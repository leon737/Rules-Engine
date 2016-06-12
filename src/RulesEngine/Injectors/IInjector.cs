using System.Linq.Expressions;

namespace RulesEngine.Injectors
{
    public interface IInjector<TA, TB>
    {
        IInjector<TA, TB> Init(ParameterExpression pe);
        Expression Inject(TA a, Expression expr);
    }
}