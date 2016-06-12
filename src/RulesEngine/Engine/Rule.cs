using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace RulesEngine.Engine
{
    internal class Rule<TA, TB> : IRule<TA, TB>, IDefaultRule<TA, TB>
    {
        /*
         * each rule consists of three parts (only part three is mandatory):
         * (1) prerequisites
         * (2) condition
         * (3) action
         */


        private readonly List<MemberInfo> preIs = new List<MemberInfo>();
        private readonly List<MemberInfo> preNot = new List<MemberInfo>();
        private readonly List<Expression<Func<TA, TB, bool>>> conditions = new List<Expression<Func<TA, TB, bool>>>();
        private bool throwErrorFlag;
        private bool nextRuleFlag;
        private readonly List<Tuple<MemberInfo, MemberInfo, bool>> applies = new List<Tuple<MemberInfo, MemberInfo, bool>>();
        private readonly List<Action<TA, TB, string>> customActions = new List<Action<TA, TB, string>>();
        private readonly List<Func<TA, TB, bool>> customConditions = new List<Func<TA, TB, bool>>();
        private bool singleFlag;
        private Action<TA, TB, string> singleFailAction;
        private bool anyFlag;
        private Action<TA, string> anyFailAction;
        public bool SkipToActionPart { get; set; }
        private string ruleName;


        public IRule<TA, TB> Name(string name)
        {
            ruleName = name;
            return this;
        }

        public IRule<TA, TB> Is<TKey>(Expression<Func<TA, TKey>> propertySelector)
        {
            var z = propertySelector.Body as MemberExpression;
            if (z == null)
                throw new ArgumentException(nameof(propertySelector));
            preIs.Add(z.Member);
            return this;
        }

        public IRule<TA, TB> Not<TKey>(Expression<Func<TA, TKey>> propertySelector)
        {
            var z = propertySelector.Body as MemberExpression;
            if (z == null)
                throw new ArgumentException(nameof(propertySelector));
            preNot.Add(z.Member);
            return this;
        }

        public IRule<TA, TB> If(Expression<Func<TA, TB, bool>> predicate)
        {
            conditions.Add(predicate);
            return this;
        }

        public IRule<TA, TB> CustomCondition(Func<TA, TB, bool> func)
        {
            customConditions.Add(func);
            return this;
        }

        public IActionRule<TA, TB> ThrowError()
        {
            throwErrorFlag = true;
            return this;
        }

        public IActionRule<TA, TB> NextRule()
        {
            nextRuleFlag = true;
            return this;
        }

        public IActionRule<TA, TB> Apply<TKeyA, TKeyB>(Expression<Func<TA, TB, IMapField<TKeyA, TKeyB>>> propertySelectors)
        {
            AddApply(propertySelectors, false);
            return this;
        }

        public IActionRule<TA, TB> ApplyForEmpty<TKeyA, TKeyB>(Expression<Func<TA, TB, IMapField<TKeyA, TKeyB>>> propertySelectors)
        {
            AddApply(propertySelectors, true);
            return this;
        }

        private void AddApply<TKeyA, TKeyB>(Expression<Func<TA, TB, IMapField<TKeyA, TKeyB>>> propertySelectors, bool forEmpty)
        {
            var mc = propertySelectors.Body as MethodCallExpression;
            if (mc == null)
                throw new ArgumentException(nameof(propertySelectors));
            var a = mc.Arguments[0] as MemberExpression;
            var b = mc.Arguments[1] as MemberExpression;
            if (a == null || b == null)
                throw new ArgumentException(nameof(propertySelectors));
            applies.Add(Tuple.Create(a.Member, b.Member, forEmpty));
        }

        public IActionRule<TA, TB> CustomAction(Action<TA, TB, string> action)
        {
            customActions.Add(action);
            return this;
        }

        public IRule<TA, TB> Single(Action<TA, TB, string> failAction = null)
        {
            singleFlag = true;
            singleFailAction = failAction;
            return this;
        }

        public IRule<TA, TB> Any(Action<TA, string> failAction = null)
        {
            anyFlag = true;
            anyFailAction = failAction;
            return this;
        }

        public bool ProcessRule(TA a, IDataAdapter<TB> dataAdapter)
        {
            if (preIs.Any(v => !CheckPositivePrerequisite(v, a)))
                return false;

            if (preNot.Any(v => !CheckNegativePrerequisite(v, a)))
                return false;

            var query = dataAdapter.GetData();

            var resultSet = SkipToActionPart ? Enumerable.Empty<TB>() : TestForConditions(query, a);

            var enumerable = resultSet as IList<TB> ?? resultSet.ToList();

            if (enumerable.Any())
            {

                bool ok = true;

                if (singleFlag && enumerable.Count() > 1)
                {
                    if (singleFailAction != null)
                        singleFailAction(a, enumerable.First(), ruleName);
                    return false;
                }

                foreach (var b in enumerable)
                {
                    
                    if (customConditions.Any())
                        if (customConditions.Any(customCondition => !customCondition(a, b)))
                            ok = false;

                    if (!ok) continue;

                    foreach (var apply in applies)
                    {
                        object val = Helper.GetMemberValue(apply.Item1, a);
                        if (apply.Item3)
                        {
                            if (!Helper.CheckIsNotNullOrDefault(apply.Item2, b))
                                Helper.SetMemberValue(apply.Item2, b, val);
                        }
                        else
                            Helper.SetMemberValue(apply.Item2, b, val);
                    }

                    dataAdapter.SaveChanges();

                    foreach (var customAction in customActions)
                    {
                        customAction(a, b, ruleName);
                    }
                }

                if (ok && throwErrorFlag)
                {
                    throw new UserDefinedRuleException(ruleName + "; throw error triggered");
                }

                return !nextRuleFlag && ok;
            }

            if (SkipToActionPart)
            {
                foreach (var customAction in customActions)
                {
                    customAction(a, default(TB), ruleName);
                }
            }

            if (anyFlag)
            {
                if (anyFailAction != null)
                    anyFailAction(a, ruleName);
            }
            
            return false;
        }

        private IEnumerable<TB> TestForConditions(IQueryable<TB> query, TA a)
        {
            if (!conditions.Any())
                return query.AsEnumerable();
            var whereClause = MakeWhereClause(a);
            return query.Where(whereClause).AsEnumerable();
        }

        private Expression<Func<TB, bool>> MakeWhereClause(TA a)
        {
            ParameterExpression paramExpr = Expression.Parameter(typeof(TB), "y");
            var conditionParser = new ConditionParser<TA, TB>(paramExpr);
            var expressions = conditions.Select(x => conditionParser.InjectValues(a, x));
            var finalExpression = conditionParser.Join(expressions);
            Expression<Func<TB, bool>> t = Expression.Lambda<Func<TB, bool>>(finalExpression, paramExpr);
            return t;
        }


        private bool CheckPositivePrerequisite(MemberInfo mi, TA a)
        {
            return Helper.CheckIsNotNullOrDefault(mi, a);
        }

        private bool CheckNegativePrerequisite(MemberInfo mi, TA a)
        {
            return !Helper.CheckIsNotNullOrDefault(mi, a);
        }
    }
}
