using System;

namespace RulesEngineTests
{
    public class SampleEntity
    {
        public string StringValue { get; set; }
       
        public int IntValue { get; set; }

        public decimal DecimalValue { get; set; }

        public DateTime DateTimeValue { get; set; }

        public bool BoolValue { get; set; }

        public int? IntNullableValue { get; set; }

        public decimal? DecimalNullableValue { get; set; }

        public DateTime? DateTimeNullableValue { get; set; }

        public bool? BoolNullableValue { get; set; }
    }
}
