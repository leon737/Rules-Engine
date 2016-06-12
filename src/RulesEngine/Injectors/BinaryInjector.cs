using System.Linq.Expressions;

namespace RulesEngine.Injectors
{
    public class BinaryInjector<TA, TB> : InjectorBase<TA, TB, BinaryExpression>
    {
        protected override Expression InjectCore(TA a, BinaryExpression expr) => expr.Update(
                InjectorFactory.GetInstance().GetInjector<TA, TB>(expr.Left, ParameterExpression).Inject(a, expr.Left),
                expr.Conversion,
                InjectorFactory.GetInstance().GetInjector<TA, TB>(expr.Right, ParameterExpression).Inject(a, expr.Right)
                );
    }
}
