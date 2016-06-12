using System;

namespace RulesEngine.Engine
{
    public class UserDefinedRuleException : Exception
    {
        public UserDefinedRuleException(string ruleName) : base($"Rule: {ruleName}") { }
    }
}