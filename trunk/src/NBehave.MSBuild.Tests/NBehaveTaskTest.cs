using System;
using System.Diagnostics;
using Microsoft.Build.Framework;
using Microsoft.Win32;
using NUnit.Framework;
using System.IO;
using Rhino.Mocks;
using TestPlainTextAssembly;


namespace NBehave.MSBuild.Tests
{
    [TestFixture]
    public class NBehaveTaskTest
    {
        [Test]
        public void ShouldExecuteStorySuccessfullyViaMsbuildEXE()
        {
            string msbuild = Path.Combine(
                                    Path.Combine((string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\.NETFramework", "InstallRoot", string.Empty),
                                    "v3.5"),
                                "MSBuild.exe");

            using (var process = new Process())
            {
                process.StartInfo = new ProcessStartInfo(msbuild, "nbehaveTaskTestScript.msbuild");
                process.StartInfo.WorkingDirectory = Path.GetDirectoryName(GetType().Assembly.CodeBase.Replace(@"file:///", ""));
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.Start();
                using (StreamReader sr = process.StandardOutput)
                {
                    process.WaitForExit();

                    string result = sr.ReadToEnd();
                    string[] results = result.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
                    // Fails if language on computer isnt english: Assert.Contains("Build succeeded.", results);
                    Assert.Contains("  Scenarios run: 1, Failures: 0, Pending: 0", results);
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

