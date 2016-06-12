using System.Linq.Expressions;

namespace RulesEngine.Injectors
{
    public class DefaultInjector<TA, TB> : InjectorBase<TA, TB, Expression>
    {
        protected override Expression InjectCore(TA a, Expression expr) => expr;
    }
}
