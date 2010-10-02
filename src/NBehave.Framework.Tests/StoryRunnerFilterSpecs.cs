using System.Diagnostics;
using System.Reflection;
using NBehave.Spec.NUnit;
using NUnit.Framework;
using Rhino.Mocks;

namespace NBehave.Narrator.Framework.Specifications
{
    [TestFixture]
    public class StoryRunnerFilterSpecs
    {
        private readonly MockRepository _mocks = new MockRepository();

        private delegate void Action();

        private void Given(Action given)
        {
            using (_mocks.Record())
            {
                given.Invoke();
            }
        }

        private void When(Action when)
        {
            using (_mocks.Playback())
            {
                when.Invoke();
            }
        }

        private void Then(Action then)
        {
            _mocks.ReplayAll();
            then.Invoke();
        }

        [Test]
        public void ShouldSetMemberfilterForMethodInfoOfTypeMethod()
        {
            StoryRunnerFilter filter = null;
            MemberInfo member = null;

            Given(
                () =>
                    {
                        var stack = new StackFrame(2); //
                        member = stack.GetMethod();
                    }
                );

            When(
                () => { filter = StoryRunnerFilter.GetFilter(member); }
                );

            Then(
                () =>
                    {
                        filter.MethodNameFiler.ToString().ShouldEqual("^ShouldSetMemberfilterForMethodInfoOfTypeMethod$");
                        filter.ClassNameFilter.ToString().ShouldEqual("^" + typeof (StoryRunnerFilterSpecs).Name + "$");
                        filter.NamespaceFilter.ToString().ShouldEqual("^" + typeof (StoryRunnerFilterSpecs).Namespace + "$");
                    }
                );
        }
    }
}