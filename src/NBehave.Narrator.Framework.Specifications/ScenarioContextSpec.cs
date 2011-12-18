using NUnit.Framework;

namespace NBehave.Narrator.Framework.Specifications
{
    [TestFixture]
    public class ScenarioContextSpec
    {
        [SetUp]
        public void Initialize()
        {
            NBehaveInitialiser.Initialise(ConfigurationNoAppDomain.New);
        }

        [Test]
        public void Should_retrieve_set_value_with_type()
        {
            var context = ScenarioContext.Current;
            context["Foo2"] = "Hello2";
            object value = context.Get<string>("Foo2");
            Assert.AreEqual("Hello2", value);
        }

        [Test]
        public void Should_TryGet_value_when_item_exists_returns_true_and_out_param_has_value()
        {
            ScenarioContext.Current["hasValue"] = 42;
            int value;
            var result = ScenarioContext.Current.TryGet("hasValue", out value);
            Assert.IsTrue(result);
            Assert.AreEqual(42, value);
        }

        [Test]
        public void Should_get_same_instance_of_context_when_using_Current()
        {
            var context1 = ScenarioContext.Current;
            context1["Foo3"] = "same";
            var context2 = ScenarioContext.Current;
            var value = context2["Foo3"];
            Assert.AreEqual("same", value);
        }
    }
}