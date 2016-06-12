using System.Linq.Expressions;

namespace RulesEngine.Injectors
{
    public class MemberInjector<TA, TB> : InjectorBase<TA, TB, MemberExpression>
    {
        protected override Expression InjectCore(TA a, MemberExpression expr)
        {
            // TODO: replace with type pattern matching (Functional.Fluent)
            if (expr.Member.DeclaringType == typeof (TA))
                return MakeConstant(expr, a);
            if (expr.Member.DeclaringType == typeof (TB))
                return MakeParameter(expr);
            return expr;
        }
    }
}
