using System;
using NBehave.Fluent.Framework.Extensions;
using NBehave.Narrator.Framework;
using NUnit.Framework;

namespace NBehave.Fluent.Framework.Specifications
{
    [TestFixture]
    public abstract class ScenarioBuilderSpec
    {
        [TestFixture]
        public class When_actionSteps_is_in_helper_class : ScenarioBuilderSpec
        {
            [ActionSteps]
            public class HelperClass
            {
                public static bool GivenWasCalled;
                public static bool AndWasCalled;
                public static bool ThenWasCalled;
                public static bool WhenWasCalled;

                [Given("something to call")]
                public void Something()
                {
                    GivenWasCalled = true;
                }

                [Given("something else")]
                public void SomethingElse()
                {
                    AndWasCalled = true;
                }

                [When("method should be called")]
                public void WhenMethod()
                {
                    WhenWasCalled = true;
                }

                [Then("this should work")]
                public void ThenMethod()
                {
                    ThenWasCalled = true;
                }
            }

            [SetUp]
            public void RunScenario()
            {
                HelperClass.GivenWasCalled = false;
                var feature = new Feature("Hello");
                feature.AddScenario()
                    .WithHelperObject<HelperClass>()
                    .Given("something to call")
                    .And("something else")
                    .When("method should be called")
                    .Then("this should work");
            }

            [Test]
            public void Should_call_Given_method_with_attribute_in_ActionStepsClass()
            {
                Assert.IsTrue(HelperClass.GivenWasCalled, "Step was not invoked");
            }

            [Test]
            public void Should_call_And_method_with_attribute_in_ActionStepsClass()
            {
                Assert.IsTrue(HelperClass.AndWasCalled, "Step was not invoked");
            }

            [Test]
            public void Should_call_When_method_with_attribute_in_ActionStepsClass()
            {
                Assert.IsTrue(HelperClass.WhenWasCalled, "Step was not invoked");
            }

            [Test]
            public void Should_call_Then_method_with_attribute_in_ActionStepsClass()
            {
                Assert.IsTrue(HelperClass.ThenWasCalled, "Step was not invoked");
            }
        }
    }

    [TestFixture]
    public class When_actionsteps_are_in_same_or_base_class : ScenarioBuilderSpec
    {
        public static bool GivenWasCalled;
        public static bool AndWasCalled;
        public static bool ThenWasCalled;
        public static bool WhenWasCalled;

        [SetUp]
        public void RunScenario()
        {
            var feature = new Feature("Hello");
            feature.AddScenario()
                .WithHelperObject(this)
                    .Given("something to call")
                    .And("something else")
                    .When("method should be called")
                    .Then("this should work");
        }

        [Test]
        public void Should_call_Given_method_with_attribute_in_ActionStepsClass()
        {
            Assert.IsTrue(GivenWasCalled, "Step was not invoked");
        }

        [Test]
        public void Should_call_And_method_with_attribute_in_ActionStepsClass()
        {
            Assert.IsTrue(AndWasCalled, "Step was not invoked");
        }

        [Test]
        public void Should_call_When_method_with_attribute_in_ActionStepsClass()
        {
            Assert.IsTrue(WhenWasCalled, "Step was not invoked");
        }

        [Test]
        public void Should_call_Then_method_with_attribute_in_ActionStepsClass()
        {
            Assert.IsTrue(ThenWasCalled, "Step was not invoked");
        }

        public void Given_something_to_call()
        {
            GivenWasCalled = true;
        }

        public void Given_something_else()
        {
            AndWasCalled = true;
        }

        public void When_method_should_be_called()
        {
            WhenWasCalled = true;
        }

        public void Then_this_should_work()
        {
            ThenWasCalled = true;
        }
    }

    [TestFixture]
    public class When_step_implementation_is_inline : ScenarioBuilderSpec
    {
        [Test]
        public void Should_call_method_with_attribute_in_ActionStepsClass()
        {
            bool givenWasCalled = false;
            bool andWasCalled = false;
            bool whenWasCalled = false;
            bool thenWasCalled = false;

            var feature = new Feature("Hello");
            feature.AddScenario()
                .Given("something to call", () => givenWasCalled = true)
                .And("something else", () => andWasCalled = true)
                .When("method should be called", () => whenWasCalled = true)
                .Then("this should work", () => thenWasCalled = true);

            Assert.IsTrue(givenWasCalled, "Given step was not invoked");
            Assert.IsTrue(andWasCalled, "And step was not invoked");
            Assert.IsTrue(whenWasCalled, "When step was not invoked");
            Assert.IsTrue(thenWasCalled, "Then step was not invoked");
        }

        [Test]
        public void Should_use_And_for_second_Given()
        {
            var feature = new Feature("Hello");
            var scenarioBuilder = feature.AddScenario();
            var fragment = scenarioBuilder.Given("foo");
            fragment.And("bar");
            var scenario = feature.Scenarios[0].ToString();
            Assert.AreEqual(
                "Scenario: " + Environment.NewLine +
                "  Given foo" + Environment.NewLine +
                "  And bar", scenario);
        }

        [Test]
        public void Should_use_And_for_second_When()
        {
            var feature = new Feature("Hello");
            var scenarioBuilder = feature.AddScenario();
            var fragment = scenarioBuilder.Given("foo");
            var whenFragment = fragment.When("bar");
            whenFragment.And("baz");
            var scenario = feature.Scenarios[0].ToString();
            Assert.AreEqual(
                "Scenario: " + Environment.NewLine +
                "  Given foo" + Environment.NewLine +
                "  When bar" + Environment.NewLine +
                "  And baz", scenario);
        }

        [Test]
        public void Should_use_And_for_second_Then()
        {
            var feature = new Feature("Hello");
            var scenarioBuilder = feature.AddScenario();
            var fragment = scenarioBuilder.Given("foo");
            var whenFragment = fragment.When("bar");
            var thenFragment = whenFragment.Then("moo");
            thenFragment.And("moo-moo");
            var scenario = feature.Scenarios[0].ToString();
            Assert.AreEqual(
                "Scenario: " + Environment.NewLine +
                "  Given foo" + Environment.NewLine +
                "  When bar" + Environment.NewLine +
                "  Then moo" + Environment.NewLine +
                "  And moo-moo", scenario);
        }
    }
}