using System;
using System.Linq.Expressions;

namespace RulesEngine.Engine
{
    public interface IPrerequisitesRule<TA, TB>
    {

        IRule<TA, TB> Is<TKey>(Expression<Func<TA, TKey>> propertySelector);
        IRule<TA, TB> Not<TKey>(Expression<Func<TA, TKey>> propertySelector);
    }
}
