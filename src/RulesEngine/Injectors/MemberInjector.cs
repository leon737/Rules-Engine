using Functional.Fluent;
using System.Linq.Expressions;

namespace RulesEngine.Injectors
{
    public class MemberInjector<TA, TB> : InjectorBase<TA, TB, MemberExpression>
    {
        protected override Expression InjectCore(TA a, MemberExpression expr) => expr.Member.DeclaringType.ToMaybe().Match()
                .With(typeof(TA), _ => MakeConstant(expr, a))
                .With(typeof(TB), _ => MakeParameter(expr))
                .Else(_ => expr)
                .Do();            
    }
}
