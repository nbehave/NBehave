using System;
using NBehave.Narrator.Framework.Hooks;
using NUnit.Framework;

namespace NBehave.Narrator.Framework.Specifications
{
    [TestFixture]
    public abstract class HooksParserSpec
    {
        [Hooks]
        public class When_finding_hooks : HooksParserSpec
        {
            [BeforeRun, AfterRun, BeforeFeature, AfterFeature, BeforeScenario, AfterScenario, BeforeStep, AfterStep]
            public void HookMeUp()
            { }

            [TestCase(typeof(BeforeRunAttribute))]
            [TestCase(typeof(AfterRunAttribute))]
            [TestCase(typeof(BeforeFeatureAttribute))]
            [TestCase(typeof(AfterFeatureAttribute))]
            [TestCase(typeof(BeforeScenarioAttribute))]
            [TestCase(typeof(AfterScenarioAttribute))]
            [TestCase(typeof(BeforeStepAttribute))]
            [TestCase(typeof(AfterStepAttribute))]
            public void Should_find_hook(Type typeOfHook)
            {
                var hooksCatalog = new HooksCatalog();
                var h = new HooksParser(hooksCatalog);
                h.FindHooks(GetType());
            }
        }
    }
}