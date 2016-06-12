using System;
using System.Linq;
using RulesEngine.Engine;

namespace RulesEngineTests
{
    public class SampleDataAdapter2 : IDataAdapter<SampleEntity2>
    {

        private IQueryable<SampleEntity2> data;

        public SampleDataAdapter2()
        {
            data = new[]
            {
                new SampleEntity2()
                {
                    IntNullableValue = 5,
                    IntValue = 1,
                    StringValue = "",
                    DateTimeValue = new DateTime(2000, 01, 01)
                },
                new SampleEntity2()
                {
                    IntValue = 6,
                    StringValue = "Hello world!",
                    DateTimeValue = new DateTime(2100, 01, 01)
                }
            }.AsQueryable();
        }


        public IQueryable<SampleEntity2> GetData() => data;

        public void SaveChanges()
        {
               
        }
    }
}
