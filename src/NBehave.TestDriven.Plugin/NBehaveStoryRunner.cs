using System;
using System.IO;
using System.Reflection;
using NBehave.Configuration;
using NBehave.Domain;
using NBehave.Extensions;
using NBehave.Internal;

using TestDriven.Framework;

//IF you change the namespace or the name of the class dont forget to update the installer.

namespace NBehave.TestDriven.Plugin
{
    public class NBehaveStoryRunner : ITestRunner
    {
        TestRunState ITestRunner.RunAssembly(ITestListener tddNetListener, Assembly assembly)
        {
            return TestRunState.NoTests;
            //return Run(assembly, null, tddNetListener);            
        }

        TestRunState ITestRunner.RunMember(ITestListener tddNetListener, Assembly assembly, MemberInfo member)
        {
            //return Run(assembly, member, tddNetListener);      
            return TestRunState.NoTests;
        }

        TestRunState ITestRunner.RunNamespace(ITestListener testListener, Assembly assembly, string ns)
        {
            return TestRunState.NoTests;

            //TODO: Fix filter. ns is probably equal to NamespaceFilter in StoryRunnerFilter
            //return Run(assembly, null, testListener);      
        }

        private TestRunState Run(Assembly assembly, MemberInfo member, ITestListener tddNetListener)
        {
            var locator = new StoryLocator
                              {
                                  RootLocation = Path.GetDirectoryName(assembly.Location)
                              };

            var type = member as Type;
            var stories = (type == null)
                              ? locator.LocateAllStories()
                              : locator.LocateStoriesMatching(type);

            var results = NBehaveConfiguration
                .New
                .DontIsolateInAppDomain()
                .SetEventListener(new StoryRunnerEventListenerProxy(tddNetListener))
                .SetScenarioFiles(stories)
                .SetAssemblies(new[] {assembly.Location})
                .SetFilter(StoryRunnerFilter.GetFilter(member))
                .Build()
                .Run();

            return GetTestRunState(results);
        }

        private static TestRunState GetTestRunState(FeatureResults results)
        {
            var state = TestRunState.Success;
            if (results.NumberOfFailingScenarios > 0)
                state = TestRunState.Failure;
            if (results.NumberOfScenariosFound == 0)
                state = TestRunState.NoTests;
            return state;
        }
    }
}