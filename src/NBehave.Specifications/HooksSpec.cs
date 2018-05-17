using System;
using System.Linq;
using NBehave.Extensions;
using NBehave.Hooks;
using NBehave.Internal;
using NUnit.Framework;
using Rhino.Mocks;

namespace NBehave.Specifications
{
    [TestFixture]
    public abstract class HooksSpec
    {
        [Hooks]
        public class When_finding_hooks : HooksSpec
        {
            [Hooks.BeforeRun, Hooks.BeforeFeature, Hooks.BeforeScenario, Hooks.BeforeStep]
            public void HookMeUp()
            { }

            [Hooks.AfterRun, Hooks.AfterFeature, Hooks.AfterScenario, Hooks.AfterStep]
            private void HookMeUpToo() { }

            [TestCase(typeof(Hooks.BeforeRunAttribute))]
            [TestCase(typeof(Hooks.AfterRunAttribute))]
            [TestCase(typeof(Hooks.BeforeFeatureAttribute))]
            [TestCase(typeof(Hooks.AfterFeatureAttribute))]
            [TestCase(typeof(Hooks.BeforeScenarioAttribute))]
            [TestCase(typeof(Hooks.AfterScenarioAttribute))]
            [TestCase(typeof(Hooks.BeforeStepAttribute))]
            [TestCase(typeof(Hooks.AfterStepAttribute))]
            public void Should_find_hook(Type typeOfHook)
            {
                var hooksCatalog = new HooksCatalog();
                var h = new HooksParser(hooksCatalog);
                h.FindHooks(GetType());
                Assert.IsTrue(hooksCatalog.OfType(typeOfHook).Any());
            }

            [TestCase(typeof(Hooks.BeforeFeatureAttribute))]
            [TestCase(typeof(Hooks.AfterScenarioAttribute))]
            [TestCase(typeof(Hooks.BeforeStepAttribute))]
            public void Should_find_hook_in_nested_class(Type typeOfHook)
            {
                var c = new HooksCatalog();
                var p = new HooksParser(c);
                p.FindHooks(typeof(When_running_hooks_with_tag_filter.When_matching_tag_for_RunIfHasTags));
                Assert.IsTrue(c.OfType(typeOfHook).Any());
            }

            private static bool PublicDelegateHookCalled;
            [Hooks.BeforeRun]
            public Action PublicDelegateHook = () => PublicDelegateHookCalled = true;

            private static bool PrivateDelegateHookCalled;
            [Hooks.BeforeRun]
            private Action PrivateDelegateHook = () => PrivateDelegateHookCalled = true;

            [Test]
            public void Should_find_hook_on_public_Action_delegate()
            {
                var c = new HooksCatalog();
                var p = new HooksParser(c);
                PublicDelegateHookCalled = false;
                p.FindHooks(GetType());
                var hooks = c.OfType<BeforeRunAttribute>().ToList();
                hooks.Each(_ => _.Invoke());
                Assert.IsTrue(PublicDelegateHookCalled);
            }

            [Test]
            public void Should_find_hook_on_private_Action_delegate()
            {
                var c = new HooksCatalog();
                var p = new HooksParser(c);
                PrivateDelegateHookCalled = false;
                p.FindHooks(GetType());
                var hooks = c.OfType<BeforeRunAttribute>().ToList();
                hooks.Each(_ => _.Invoke());
                Assert.IsTrue(PrivateDelegateHookCalled);
            }
        }

        [Hooks]
        public abstract class When_running_hooks_with_tag_filter : HooksSpec
        {
            private IRunContextEvents context;

            [SetUp]
            public void Init()
            {
                var catalog = new HooksCatalog();
                var loader = new HooksParser(catalog);
                loader.FindHooks(GetType());

                var handler = new HooksHandler(catalog);
                context = MockRepository.GenerateStub<IRunContextEvents>();
                handler.SubscribeToHubEvents(context);

                SetupContextTags();
            }

            protected abstract void SetupContextTags();

            public class When_matching_tag_for_RunIfHasTags : When_running_hooks_with_tag_filter
            {
                private static bool HookExecuted;

                [Hooks.BeforeFeature(RunIfHasTags = new[] { "tag1" })]
                public void BeforeFeature()
                {
                    HookExecuted = true;
                }

                [Hooks.AfterScenario(RunIfHasTags = new[] { "tag2" })]
                public void AfterScenario()
                {
                    HookExecuted = true;
                }

                [Hooks.BeforeStep(RunIfHasTags = new[] { "tag1" })]
                public void BeforeStep()
                {
                    HookExecuted = true;
                }

                protected override void SetupContextTags()
                {
                    var f = new FeatureContext(null, new[] { "tag1" });
                    TinyIoC.TinyIoCContainer.Current.Register(f);
                    var c = new ScenarioContext(f, null, new[] { "tag2", "tag5" });
                    TinyIoC.TinyIoCContainer.Current.Register(c);
                }

                [Test]
                public void Should_run_before_feature_hook()
                {
                    HookExecuted = false;
                    context.Raise(_ => _.OnFeatureStarted += null, this, new EventArgs<Feature>(null));
                    Assert.IsTrue(HookExecuted);
                }

                [Test]
                public void Should_run_after_scenario_hook()
                {
                    HookExecuted = false;
                    context.Raise(_ => _.OnScenarioFinished += null, this, new EventArgs<ScenarioResult>(null));
                    Assert.IsTrue(HookExecuted);
                }

                [Test]
                public void Should_run_before_step_hook()
                {
                    HookExecuted = false;
                    context.Raise(_ => _.OnStepStarted += null, this, new EventArgs<StringStep>(null));
                    Assert.IsTrue(HookExecuted);
                }
            }

            public class When_no_matching_tags_for_RunIfHasTags : When_running_hooks_with_tag_filter
            {
                private static bool HookExecuted;

                [Hooks.BeforeFeature(RunIfHasTags = new[] { "DontRun" })]
                public void BeforeFeature()
                {
                    HookExecuted = true;
                }

                protected override void SetupContextTags()
                {
                    var f = new FeatureContext(null, new[] { "Foo" });
                    TinyIoC.TinyIoCContainer.Current.Register(f);
                    var c = new ScenarioContext(f, null, new[] { "Bar" });
                    TinyIoC.TinyIoCContainer.Current.Register(c);
                }

                [Test]
                public void Should_not_run_hook()
                {
                    HookExecuted = false;
                    context.Raise(_ => _.OnFeatureStarted += null, this, new EventArgs<Feature>(null));
                    Assert.IsFalse(HookExecuted);
                }
            }

            public class When_matching_tags_for_DontRunIfHasTags : When_running_hooks_with_tag_filter
            {
                private static bool HookExecuted;

                [Hooks.BeforeStep(DontRunIfHasTags = new[] { "DontRun" })]
                public void BeforeStep()
                {
                    HookExecuted = true;
                }

                protected override void SetupContextTags()
                {
                    var f = new FeatureContext(null, new string[0]);
                    TinyIoC.TinyIoCContainer.Current.Register(f);
                    var c = new ScenarioContext(f, null, new[] { "DontRun" });
                    TinyIoC.TinyIoCContainer.Current.Register(c);
                }

                [Test]
                public void Should_not_run_hook()
                {
                    HookExecuted = false;
                    context.Raise(_ => _.OnStepStarted += null, this, new EventArgs<StringStep>(null));
                    Assert.IsFalse(HookExecuted);
                }
            }

            public class When_not_matching_tag_for_DontRunIfHasTags : When_running_hooks_with_tag_filter
            {
                private static bool HookExecuted;

                [Hooks.BeforeStep(DontRunIfHasTags = new[] { "ShouldRun" })]
                public void BeforeStep()
                {
                    HookExecuted = true;
                }

                protected override void SetupContextTags()
                {
                    var f = new FeatureContext(null, new string[0]);
                    TinyIoC.TinyIoCContainer.Current.Register(f);
                    var c = new ScenarioContext(f, null, new[] { "DontRun" });
                    TinyIoC.TinyIoCContainer.Current.Register(c);
                }

                [Test]
                public void Should_run_hook()
                {
                    HookExecuted = false;
                    context.Raise(_ => _.OnStepStarted += null, this, new EventArgs<StringStep>(null));
                    Assert.IsTrue(HookExecuted);
                }
            }
        }
    }
}
