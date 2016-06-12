namespace RulesEngine.Engine
{
    public class MapField<TA, TB> : IMapField<TA, TB>
    {
        public TA A { get; }

        public TB B { get; }

        public MapField(TA a, TB b)
        {
            A = a;
            B = b;
        }
    }

    public static class MapField
    {
        public static MapField<TA, TB> Create<TA, TB>(TA a, TB b) => new MapField<TA, TB>(a, b);
    }
    
}