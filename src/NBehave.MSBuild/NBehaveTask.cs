// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NBehaveTask.cs" company="NBehave">
//   Copyright (c) 2007, NBehave - http://nbehave.codeplex.com/license
// </copyright>
// <summary>
//   Defines the NBehaveTask type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace NBehave.MSBuild
{
    using System;
    using System.IO;
    using System.Text;

    using Microsoft.Build.Framework;
    using Microsoft.Build.Utilities;

    using NBehave.Narrator.Framework;
    using NBehave.Narrator.Framework.EventListeners;

    public class NBehaveTask : Task
    {
        public bool DryRun { get; set; }

        public string TextOutputFile { get; set; }

        public string XmlOutputFile { get; set; }

        [Required]
        public string[] TestAssemblies { get; set; }

        public bool FailBuild { get; set; }

        [Required]
        public string[] ScenarioFiles { get; set; }


        public FeatureResults FeatureResults { get; private set; }


        public NBehaveTask()
        {
            DryRun = false;
            FailBuild = true;
        }

        public NBehaveTask(IBuildEngine buildEngine)
            : this()
        {
            BuildEngine = buildEngine;
        }

        public override bool Execute()
        {
            if (TestAssemblies.Length == 0)
                throw new ArgumentException("At least one test assembly is required");

            var logString = new StringBuilder();
            TextWriter msbuildLogWriter = new StringWriter(logString);
            var output = new PlainTextOutput(msbuildLogWriter);

            WriteHeaderInto(output);

            var config = NBehaveConfiguration.New
                .SetScenarioFiles(ScenarioFiles)
                .SetDryRun(DryRun)
                .SetAssemblies(TestAssemblies)
                .SetEventListener(EventListeners.CreateEventListenerUsing(msbuildLogWriter, TextOutputFile, XmlOutputFile));
            var runner = new TextRunner(config);

            FeatureResults = runner.Run();

            if (DryRun)
                return true;

            var message = logString.ToString();
            Log.LogMessage(message);

            if (FailBuild && FailBuildBasedOn(FeatureResults))
                return false;

            return true;
        }

        private void WriteHeaderInto(PlainTextOutput output)
        {
            output.WriteHeader();
            output.WriteSeparator();
            output.WriteRuntimeEnvironment();
            output.WriteSeparator();
        }

        private bool FailBuildBasedOn(FeatureResults results)
        {
            if (results.NumberOfFailingScenarios == 0)
                return false;

            var exceptionMessage = new StringBuilder();
            foreach (var result in results.ScenarioResults)
            {
                exceptionMessage.AppendLine(result.Message);
                exceptionMessage.AppendLine(result.StackTrace);
                exceptionMessage.AppendLine();
            }

            Log.LogError(exceptionMessage.ToString());
            return true;
        }
    }
}
