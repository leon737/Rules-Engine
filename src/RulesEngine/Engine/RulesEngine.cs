using System.Collections.Generic;
using System.Linq;

namespace RulesEngine.Engine
{
    public class RulesEngine<TA, TB>
    {
        private readonly List<Rule<TA, TB>> rulesCollection = new List<Rule<TA, TB>>();

        private readonly Rule<TA, TB> defaultRule = new Rule<TA, TB> {SkipToActionPart = true};

        private int skipRules = 0;

        public IRule<TA, TB> NewRule()
        {
            var rule = new Rule<TA, TB>();
            rulesCollection.Add(rule);
            return rule;
        }

        public IDefaultRule<TA, TB> DefaultRule => defaultRule;

        public void ApplyRules(TA a, IDataAdapter<TB> dataAdapter)
        {
            if (rulesCollection.Skip(skipRules).Select(rule => rule.ProcessRule(a, dataAdapter)).Any(result => result)) return;
            defaultRule.ProcessRule(a, dataAdapter);
        }

        public RulesEngine<TA, TB> SkipRules(int numberOfRulesToSkip)
        {
            skipRules = numberOfRulesToSkip;
            return this;
        }
    }
}
