using System.Linq.Expressions;

namespace RulesEngine.Injectors
{
    public class LambdaInjector<TA, TB> : InjectorBase<TA, TB, LambdaExpression>
    {
        protected override Expression InjectCore(TA a, LambdaExpression expr) => Expression.Lambda(InjectorFactory.GetInstance()
                .GetInjector<TA, TB>(expr.Body, ParameterExpression)
                .Inject(a, expr.Body), expr.Parameters);
    }
}
