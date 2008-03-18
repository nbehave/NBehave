using System.Reflection;

using NBehave.Narrator.Framework;
using TestDriven.Framework;


namespace NBehave.TestDriven.Plugin
{
    public class NBehaveStoryRunner : ITestRunner
    {
        #region ITestRunner Members

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
            StoryRunnerEventListenerProxy listener = new StoryRunnerEventListenerProxy(tddNetListener);
            StoryResults results = RunStories(assembly, member, listener);

            return GetTestRunState(results);
        }

        private StoryResults RunStories(Assembly assembly, MemberInfo member, StoryRunnerEventListenerProxy listener)
        {
            StoryRunner runner = new StoryRunner();
            runner.StoryRunnerFilter = StoryRunnerFilter.GetFilter(member);
            runner.LoadAssembly(assembly);

            StoryResults results = runner.Run(listener);

            return results;
        }

        private TestRunState GetTestRunState(StoryResults results)
        {
            TestRunState state = TestRunState.Success;
            if (results.NumberOfFailingScenarios > 0)
                state = TestRunState.Failure;
            if (results.NumberOfScenariosFound == 0)
                state = TestRunState.NoTests;
            return state;
        }

    
        
        #endregion
    }
}
