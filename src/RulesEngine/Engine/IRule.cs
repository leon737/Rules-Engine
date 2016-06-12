namespace RulesEngine.Engine
{
    public interface IRule<TA, TB> : IPrerequisitesRule<TA, TB>, IConditionRule<TA, TB>, IActionRule<TA, TB>
    {
        IRule<TA, TB> Name(string name);
    }
}
