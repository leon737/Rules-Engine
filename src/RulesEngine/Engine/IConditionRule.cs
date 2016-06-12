using System;
using System.Linq.Expressions;

namespace RulesEngine.Engine
{
    public interface IConditionRule<TA, TB>
    {
        IRule<TA, TB> If(Expression<Func<TA, TB, bool>> predicate);

        IRule<TA, TB> CustomCondition(Func<TA, TB, bool> func);

        IRule<TA, TB> Single(Action<TA, TB, string> failAction = null);

        IRule<TA, TB> Any(Action<TA, string> failAction = null);
    }
}
