namespace RulesEngine.Engine
{
    public interface IMapField<TA, TB>
    {
        TA A { get; }

        TB B { get; }
    }
}