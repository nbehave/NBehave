using System.Diagnostics;
using System.Reflection;
using NBehave.Spec.NUnit;
using Rhino.Mocks;
using Context = NUnit.Framework.TestFixtureAttribute;

namespace NBehave.Narrator.Framework.Specifications
{
    [Context]
    public class StoryRunnerFilterSpecs
    {
        private readonly MockRepository mocks = new MockRepository();

        private delegate void Action();

        private void Given(Action given)
        {
            using (mocks.Record())
            {
                given.Invoke();
            }
        }

        private void When(Action when)
        {
            using (mocks.Playback())
            {
                when.Invoke();
            }
        }

        private void Then(Action then)
        {
            mocks.ReplayAll();
            then.Invoke();
        }

        [Specification]
        public void Should_set_memberfilter_for_MethodInfo_of_type_Method()
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
                        filter.MethodNameFiler.ToString().ShouldEqual("^Should_set_memberfilter_for_MethodInfo_of_type_Method$");
                        filter.ClassNameFilter.ToString().ShouldEqual("^" + typeof (StoryRunnerFilterSpecs).Name + "$");
                        filter.NamespaceFilter.ToString().ShouldEqual("^" + typeof (StoryRunnerFilterSpecs).Namespace + "$");
                    }
                );
        }
    }
}