using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RulesEngine.Engine;

namespace RulesEngineTests
{
    [TestClass]
    public class RulesEngineTests
    {

        [TestMethod]
        public void TestAddNewRule()
        {
            var engine = new RulesEngine<SampleEntity, SampleEntity>();
            var rule = engine.NewRule();
            Assert.IsNotNull(rule);
        }

        [TestMethod]
        public void TestGetDefaultRule()
        {
            var engine = new RulesEngine<SampleEntity, SampleEntity>();
            var rule = engine.DefaultRule;
            Assert.IsNotNull(rule);
        }

        [TestMethod]
        public void TestAlwaysRule()
        {
            var engine = new RulesEngine<SampleEntity, SampleEntity>();
            bool ok = false;
            engine.NewRule().CustomAction((a, b, r) => ok = true);
            engine.ApplyRules(new SampleEntity(), new SampleDataAdapter<SampleEntity>());
            Assert.IsTrue(ok);
        }

        [TestMethod]
        [ExpectedException(typeof(UserDefinedRuleException))]
        public void TestThrowErrorRule()
        {
            var engine = new RulesEngine<SampleEntity, SampleEntity>();
            engine.NewRule().ThrowError();
            engine.ApplyRules(new SampleEntity(), new SampleDataAdapter<SampleEntity>());
        }

        [TestMethod]
        public void TestIsMethodForValue()
        {
            var myModel = new SampleEntity
            {
                StringValue = "test"
            };
            var engine = new RulesEngine<SampleEntity, SampleEntity>();
            bool ok = false;
            engine.NewRule().Is(x => x.StringValue).CustomAction((a, b, r) => ok = true);
            engine.ApplyRules(myModel, new SampleDataAdapter<SampleEntity>());
            Assert.IsTrue(ok);
        }

        [TestMethod]
        public void TestNotMethodForValue()
        {
            var myModel = new SampleEntity
            {
                StringValue = "test"
            };
            var engine = new RulesEngine<SampleEntity, SampleEntity>();
            bool ok = false;
            engine.NewRule().Not(x => x.StringValue).CustomAction((a, b, r) => ok = true);
            engine.ApplyRules(myModel, new SampleDataAdapter<SampleEntity>());
            Assert.IsFalse(ok);
        }

        [TestMethod]
        public void TestIsMethodForNull()
        {
            var myModel = new SampleEntity
            {
                StringValue = null
            };
            var engine = new RulesEngine<SampleEntity, SampleEntity>();
            bool ok = false;
            engine.NewRule().Is(x => x.StringValue).CustomAction((a, b, r) => ok = true);
            engine.ApplyRules(myModel, new SampleDataAdapter<SampleEntity>());
            Assert.IsFalse(ok);
        }
        
        [TestMethod]
        public void TestNotMethodForNull()
        {
            var myModel = new SampleEntity
            {
                StringValue = null
            };
            var engine = new RulesEngine<SampleEntity, SampleEntity>();
            bool ok = false;
            engine.NewRule().Not(x => x.StringValue).CustomAction((a, b, r) => ok = true);
            engine.ApplyRules(myModel, new SampleDataAdapter<SampleEntity>());
            Assert.IsTrue(ok);
        }

        [TestMethod]
        public void TestNotMethodForEmptyString()
        {
            var myModel = new SampleEntity
            {
                StringValue = ""
            };
            var engine = new RulesEngine<SampleEntity, SampleEntity>();
            bool ok = false;
            engine.NewRule().Not(x => x.StringValue).CustomAction((a, b, r) => ok = true);
            engine.ApplyRules(myModel, new SampleDataAdapter<SampleEntity>());
            Assert.IsTrue(ok);
        }


        [TestMethod]
        public void TestIfMethod1()
        {
            var myModel = new SampleEntity
            {
                IntValue = 5
            };
            var engine = new RulesEngine<SampleEntity, SampleEntity2>();
            int cnt = 0;
            engine.NewRule().If((x, y) => x.IntValue < y.IntValue).CustomAction((a, b, r) => cnt++ );
            engine.ApplyRules(myModel, new SampleDataAdapter2());
            Assert.AreEqual(1, cnt);
        }


        [TestMethod]
        public void TestIfMethod2()
        {
            var myModel = new SampleEntity
            {
                StringValue = "Hello"
            };
            var engine = new RulesEngine<SampleEntity, SampleEntity2>();
            int cnt = 0;
            engine.NewRule().If((x, y) => y.StringValue.StartsWith(x.StringValue)).CustomAction((a, b, r) => cnt++);
            engine.ApplyRules(myModel, new SampleDataAdapter2());
            Assert.AreEqual(1, cnt);
        }

        [TestMethod]
        public void TestIfMethod3()
        {
            var myModel = new SampleEntity
            {
                StringValue = "Hello world! Bye!"
            };
            var engine = new RulesEngine<SampleEntity, SampleEntity2>();
            int cnt = 0;
            engine.NewRule().If((x, y) => x.StringValue.StartsWith(y.StringValue)).CustomAction((a, b, r) => cnt++);
            engine.ApplyRules(myModel, new SampleDataAdapter2());
            Assert.AreEqual(2, cnt);
        }

        [TestMethod]
        public void TestIfMethod4()
        {
            var myModel = new SampleEntity();
            var engine = new RulesEngine<SampleEntity, SampleEntity2>();
            int cnt = 0;
            var defaultValue = DateTime.Now;
            engine.NewRule().If((x, y) => x.DateTimeNullableValue.GetValueOrDefault(defaultValue) > y.DateTimeValue).CustomAction((a, b, r) => cnt++);
            engine.ApplyRules(myModel, new SampleDataAdapter2());
            Assert.AreEqual(1, cnt);
        }

        [TestMethod]
        public void TestIfMethod5()
        {
            var myModel = new SampleEntity();
            var engine = new RulesEngine<SampleEntity, SampleEntity2>();
            int cnt = 0;
            engine.NewRule().If((x, y) => x.DateTimeNullableValue.GetValueOrDefault(DateTime.Now) < y.DateTimeValue).CustomAction((a, b, r) => cnt++);
            engine.ApplyRules(myModel, new SampleDataAdapter2());
            Assert.AreEqual(1, cnt);
        }

        [TestMethod]
        public void TestApplyRule()
        {
            var engine = new RulesEngine<SampleEntity, SampleEntity2>();
            engine.NewRule().Apply((a, b) => MapField.Create(a.IntValue, b.IntValue));
            var adapter = new SampleDataAdapter2();
            engine.ApplyRules(new SampleEntity { IntValue = 101 }, adapter);
            var data = adapter.GetData();
            Assert.AreEqual(101, data.First().IntValue);
        }

        [TestMethod]
        public void TestApplyForEmptyRule()
        {
            var engine = new RulesEngine<SampleEntity, SampleEntity2>();
            engine.NewRule().ApplyForEmpty((a, b) => MapField.Create(a.IntNullableValue, b.IntNullableValue));
            var adapter = new SampleDataAdapter2();
            engine.ApplyRules(new SampleEntity { IntNullableValue = 101 }, adapter);
            var data = adapter.GetData();
            Assert.AreEqual(5, data.First().IntNullableValue.GetValueOrDefault(0));
            Assert.AreEqual(101, data.Last().IntNullableValue.GetValueOrDefault(0));
        }


    }
}
