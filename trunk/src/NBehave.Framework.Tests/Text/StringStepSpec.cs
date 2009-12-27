using System;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using NUnit.Framework;
using Rhino.Mocks;

namespace NBehave.Narrator.Framework.Specifications.Text
{
    [TestFixture]
    public class StringStepSpec
    {
        private StringStepRunner _stringStepRunner;
        private ActionCatalog _catalog;

        [SetUp]
        public void Establish_context()
        {
            _catalog = new ActionCatalog();
            _stringStepRunner = new StringStepRunner(_catalog);
        }

        private StringStep CreateInstance(string step)
        {
            return CreateInstance(step, MockRepository.GenerateStub<IEventListener>());
        }

        private StringStep CreateInstance(string step, IEventListener listener)
        {
            var stringStep = new StringStep(step, "fileName", _stringStepRunner);
            StringStep.MessageAdded += (s, e) => listener.ScenarioMessageAdded(e.EventData.Message);
            return stringStep;
        }

        public class When_comparing_StringSteps : StringStepSpec
        {
            [Test]
            public void same_ref_should_be_equal()
            {
                StringStep s = CreateInstance("Foo");
                Assert.That(s.Equals(s), Is.True);
            }

            [Test]
            public void instance_is_not_equal_to_null()
            {
                var s = CreateInstance("Foo");
                Assert.That(s.Equals(null), Is.False);
            }

            [Test]
            public void instance_is_not_equal_to_int()
            {
                var s = CreateInstance("1");
                Assert.That(s.Equals(1), Is.False);
            }

            [Test]
            public void same_steps_should_be_equal()
            {
                var s1 = CreateInstance("Foo");
                var s2 = CreateInstance("Foo");
                Assert.That(s1.Equals(s2), Is.True);
            }

            [Test]
            public void different_text_in_steps_should_not_be_equal()
            {
                var s1 = CreateInstance("Foo");
                var s2 = CreateInstance("Bar");
                Assert.That(s1.Equals(s2), Is.False);
            }
        }

        public class When_running_step : StringStepSpec
        {
            [Test]
            public void Should_raise_event_for_passed_step()
            {
                Action action = () => { };
                MethodInfo methodInfo = action.Method;
                var actionMethodInfo = new ActionMethodInfo(new Regex("Foo"), action, methodInfo, "Given");
                _catalog.Add(actionMethodInfo);

                var listener = MockRepository.GenerateMock<IEventListener>();
                var step = CreateInstance("Foo", listener);
                step.Run();
                listener.AssertWasCalled(l => l.ScenarioMessageAdded("Foo"));
            }

            [Test]
            public void Should_raise_event_for_pending_step()
            {
                var listener = MockRepository.GenerateMock<IEventListener>();
                var step = CreateInstance("Foo", listener);
                step.Run();
                listener.AssertWasCalled(l => l.ScenarioMessageAdded("Foo - PENDING"));
            }

            [Test]
            public void Should_raise_event_for_failed_step()
            {
                Action action = () => { throw new AbandonedMutexException("fail"); };
                MethodInfo methodInfo = action.Method;
                var actionMethodInfo = new ActionMethodInfo(new Regex("Foo"), action, methodInfo, "Given");
                _catalog.Add(actionMethodInfo);

                var listener = MockRepository.GenerateMock<IEventListener>();
                var step = CreateInstance("Foo", listener);
                step.Run();
                listener.AssertWasCalled(l => l.ScenarioMessageAdded("Foo - FAILED"));
            }
        }
    }
}