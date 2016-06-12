using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using RulesEngine.Injectors;

namespace RulesEngine.Engine
{
    public class ConditionParser<TA, TB>
    {

        private readonly ParameterExpression pe;

        public ConditionParser()
        {
            pe = Expression.Parameter(typeof (TB), "y");
        }

        public ConditionParser(ParameterExpression expression)
        {
            pe = expression;
        }

        public Expression InjectValues(TA a, Expression<Func<TA, TB, bool>>  expression)
        {
            return InjectorFactory.GetInstance().GetInjector<TA, TB>(expression.Body, pe).Inject(a, expression.Body);
        }

        public Expression Join(IEnumerable<Expression<Func<TA, TB, bool>>> expressions)
        {
            var enumerable = expressions as IList<Expression<Func<TA, TB, bool>>> ?? expressions.ToList();
            if (enumerable.Count() == 1) return enumerable.First().Body;
            return enumerable.Aggregate<Expression<Func<TA, TB, bool>>, Expression>(null, (a, e) => a == null ? e.Body : Expression.AndAlso(a, e.Body));
        }

        public Expression Join(IEnumerable<Expression> expressions)
        {
            var enumerable = expressions as IList<Expression> ?? expressions.ToList();
            if (enumerable.Count() == 1) return enumerable.First();
            return enumerable.Aggregate<Expression, Expression>(null, (a, e) => a == null ? e : Expression.AndAlso(a, e));
        }
    }
}
