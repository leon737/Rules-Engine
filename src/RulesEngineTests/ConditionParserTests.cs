using System;
using System.Linq.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RulesEngine.Engine;

namespace RulesEngineTests
{
    [TestClass]
    public class ConditionParserTests
    {
        [TestMethod]
        public void TestInjectValues()
        {
            Expression<Func<SampleEntity, SampleEntity2, bool>> condition = (x, y) => y.IntValue != x.IntValue;
            var parser = new ConditionParser<SampleEntity, SampleEntity2>();
            var result = parser.InjectValues(new SampleEntity { IntValue = 5 }, condition);
        }

        [TestMethod]
        public void TestInjectValues2()
        {
            Expression<Func<SampleEntity, SampleEntity2, bool>> condition = (x, y) => y.IntValue != x.IntValue && x.IntValue > 10;
            var parser = new ConditionParser<SampleEntity, SampleEntity2>();
            var result = parser.InjectValues(new SampleEntity { IntValue = 5 }, condition);
        }

        [TestMethod]
        public void TestInjectValues3()
        {
            Expression<Func<SampleEntity, SampleEntity2, bool>> condition = (x, y) => y.IntValue != x.IntValue 
                && (x.DecimalNullableValue > y.DecimalNullableValue || x.StringValue == y.StringValue);
            var parser = new ConditionParser<SampleEntity, SampleEntity2>();
            var result = parser.InjectValues(new SampleEntity { IntValue = 5, 
                DecimalNullableValue = 100.0m, 
                StringValue="Test" }, condition);
        }

        [TestMethod]
        public void TestJoinExpressions()
        {
            Expression<Func<SampleEntity, SampleEntity2, bool>> condition1 = (x, y) => y.IntValue != x.IntValue;
            Expression<Func<SampleEntity, SampleEntity2, bool>> condition2 = (x, y) => x.StringValue == y.StringValue;
            var parser = new ConditionParser<SampleEntity, SampleEntity2>();
            var result = parser.Join(new[] {condition1, condition2});
        }

    }
}
