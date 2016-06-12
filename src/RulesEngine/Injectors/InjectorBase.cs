using System;
using System.Linq.Expressions;
using System.Reflection;

namespace RulesEngine.Injectors
{
    public abstract class InjectorBase<TA, TB, TE> : IInjector<TA, TB>
        where TE:Expression
    {
        protected ParameterExpression ParameterExpression { get; set; }

        public IInjector<TA, TB> Init(ParameterExpression pe)
        {
            ParameterExpression = pe;
            return this;
        }

        public Expression Inject(TA a, Expression expr) => InjectCore(a, (TE)expr);

        protected abstract Expression InjectCore(TA a, TE expr);

        protected Expression MakeConstant(Expression expr, TA a)
        {
            object val;
            Type t;
            var e = expr as MemberExpression;
            if (e == null)
                throw new ArgumentException("expr");
            if (e.Member is FieldInfo)
            {
                val = ((FieldInfo)e.Member).GetValue(a);
                t = ((FieldInfo)e.Member).FieldType;
            }
            else
            {
                val = ((PropertyInfo)e.Member).GetValue(a);
                t = ((PropertyInfo)e.Member).PropertyType;
            }
            return Expression.Constant(val, t);
        }

        protected Expression MakeParameter(Expression expr)
        {
            var e = expr as MemberExpression;
            if (e == null)
                throw new ArgumentException("expr");
            return Expression.MakeMemberAccess(ParameterExpression, e.Member);
        }
    }
}
