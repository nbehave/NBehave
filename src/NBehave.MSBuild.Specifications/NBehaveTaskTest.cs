using System;
using System.Diagnostics;
using System.IO;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Microsoft.Win32;
using NUnit.Framework;
using Rhino.Mocks;
using TestPlainTextAssembly;

namespace NBehave.MSBuild.Specifications
{
    [TestFixture]
    public class NBehaveTaskTest
    {
        [Test]
        public void ShouldExecuteStorySuccessfullyViaMsbuildEXE()
        {
            // This is a hacky way of getting the MSBuild location  :-(
            var buildTaskAssemblyName = typeof (Task).Assembly.GetName().Name;
            var clrVersion = buildTaskAssemblyName.Substring(buildTaskAssemblyName.Length - 4);
            var msbuildFolder = clrVersion == "v3.5"
                                    ? clrVersion
                                    : String.Format("v{0}.{1}.{2}", Environment.Version.Major, Environment.Version.Minor,
                                                    Environment.Version.Build);
            

            var msbuild = Path.Combine(
                                    Path.Combine((string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\.NETFramework", "InstallRoot", string.Empty),
                                    msbuildFolder),
                                "MSBuild.exe");

            using (var process = new Process())
            {
                process.StartInfo = new ProcessStartInfo(msbuild, "nbehaveTaskTestScript.msbuild");
                process.StartInfo.WorkingDirectory = Path.GetDirectoryName(GetType().Assembly.CodeBase.Replace(@"file:///", ""));
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.Start();
                using (var sr = process.StandardOutput)
                {
                    process.WaitForExit();

                    var result = sr.ReadToEnd();
                    StringAssert.Contains("Scenarios run: 1, Failures: 0, Pending: 0", result);
                }
            }
        }

        [Test]
        public void Should_execute_scenariotext_scenario()
        {
            var storyAssemblies = new[] { typeof(GreetingSystemActionSteps).Assembly.Location };
            var outputPath = Path.Combine(Path.GetDirectoryName(storyAssemblies[0]), "greetingresult.xml");
            var buildEngine = MockRepository.GenerateStub<IBuildEngine2>();

            var task = new NBehaveTask(buildEngine)
            {
                DryRun = false,
                FailBuild = true,
                XmlOutputFile = outputPath,
                TestAssemblies = storyAssemblies,
                ScenarioFiles = new[] { "GreetingSystem.scenario" }
            };

            task.Execute();
            Assert.AreEqual(1, task.FeatureResults.NumberOfPassingScenarios);
        }
    }
}

