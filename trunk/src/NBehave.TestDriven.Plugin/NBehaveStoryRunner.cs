using System.Reflection;

using NBehave.Narrator.Framework;
using TestDriven.Framework;

//IF you change the namespace or the name of the class dont forget to update the installer.
namespace NBehave.TestDriven.Plugin
{
    public class NBehaveStoryRunner : ITestRunner
    {
        TestRunState ITestRunner.RunAssembly(ITestListener tddNetListener, Assembly assembly)
        {
            return Run(assembly, null, tddNetListener);
        }

        TestRunState ITestRunner.RunMember(ITestListener tddNetListener, Assembly assembly, MemberInfo member)
        {
            return Run(assembly, member, tddNetListener);
        }
      
        TestRunState ITestRunner.RunNamespace(ITestListener testListener, Assembly assembly, string ns)
        {
            //TODO: Fix filter. ns is probably equal to NamespaceFilter in StoryRunnerFilter
            return Run(assembly, null, testListener);
        }


        private TestRunState Run(Assembly assembly, MemberInfo member, ITestListener tddNetListener)
        {
            var listener = new StoryRunnerEventListenerProxy(tddNetListener);
            FeatureResults results = RunStories(assembly, member, listener);

            return GetTestRunState(results);
        }

        private FeatureResults RunStories(Assembly assembly, MemberInfo member, IEventListener listener)
        {
            RunnerBase runner = new TextRunner(listener);
            runner.StoryRunnerFilter = StoryRunnerFilter.GetFilter(member);
            runner.LoadAssembly(assembly);

            FeatureResults results = runner.Run();

            return results;
        }

        private TestRunState GetTestRunState(FeatureResults results)
        {
            TestRunState state = TestRunState.Success;
            if (results.NumberOfFailingScenarios > 0)
                state = TestRunState.Failure;
            if (results.NumberOfScenariosFound == 0)
                state = TestRunState.NoTests;
            return state;
        }
    }
}
