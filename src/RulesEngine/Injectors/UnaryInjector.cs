using System.Linq.Expressions;

namespace RulesEngine.Injectors
{
    public class UnaryInjector<TA, TB> : InjectorBase<TA, TB, UnaryExpression>
    {
        protected override Expression InjectCore(TA a, UnaryExpression expr) =>expr.Update(InjectorFactory.GetInstance()
                .GetInjector<TA, TB>(expr.Operand, ParameterExpression)
                .Inject(a, expr.Operand));
    }
}
