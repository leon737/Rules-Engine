using System;
using System.Linq.Expressions;

namespace RulesEngine.Engine
{
    public interface IActionRule<TA, TB>
    {
        IActionRule<TA, TB> ThrowError();
    
        IActionRule<TA, TB> Apply<TKeyA, TKeyB>(Expression<Func<TA, TB, IMapField<TKeyA, TKeyB>>> propertySelectors);

        IActionRule<TA, TB> ApplyForEmpty<TKeyA, TKeyB>(Expression<Func<TA, TB, IMapField<TKeyA, TKeyB>>> propertySelectors);
    
        IActionRule<TA, TB> CustomAction(Action<TA, TB, string> action);

        IActionRule<TA, TB> NextRule();
    }
}
