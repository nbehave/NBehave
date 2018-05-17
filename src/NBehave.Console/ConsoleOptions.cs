using System;
using System.Collections.Generic;

namespace NBehave.Console
{
    public class ConsoleOptions : CommandLineOptions
    {
        public ConsoleOptions()
        {
            Tags = new List<string>();
        }

        [Option(Short = "sf", Description = "Scenario files(s) to run. Ex: scenarioFiles:scenario1.txt;scenario2.txt")]
        public string ScenarioFiles { get; set; }

        [Option(Short = "con", Description = "story output to console")]
        public bool Console { get; private set; }

        [Option(Short = "x", Description = "Xml file to receive story output")]
        public string Xml { get; set; }

        [Option(Short = "o", Description = "File to receive story output")]
        public string StoryOutput { get; set; }

        [Option(Short = "?", Description = "Display help")]
        public bool Help { get; set; }

        [Option(Short = "p", Description = "Pause console after running")]
        public bool Pause { get; set; }

        [Option(Short = "d", Description = "Do not execute actions and output all story text to <storyOutput>")]
        public bool DryRun { get; set; }

        [Option(Description = "Do not display the logo")]
        public bool Nologo { get; set; }

        [Option(Short = "c", Description = "Generate stub methods for pending steps")]
        public bool Codegen { get; set; }

        [Option(Short = "wd", Description = "Block the process until a debugger is attached")]
        public bool WaitForDebugger { get; set; }

        [Option(Description = "Filter which scenarios to run based on what tags the scenarios have. ex: ---tags @tag1,~@tag2")]
        public List<string> Tags { get; private set; }

        public bool HasStoryOutput
        {
            get
            {
                return !string.IsNullOrEmpty(StoryOutput);
            }
        }

        public bool HasStoryXmlOutput
        {
            get
            {
                return !string.IsNullOrEmpty(Xml);
            }
        }

        public bool IsValid()
        {
            if (Exception != null)
                return false;

            if (Help)
                return true;

            if (ScenarioFiles == null)
                return false;

            if (DryRun && !HasStoryOutput)
                return false;

            return Parameters.Count >= 1;
        }

        public IEnumerable<string> Errors()
        {
            if (Help)
                yield break;
            else
            {
                if (ScenarioFiles == null)
                    yield return "* No scenario files found.";

                if (DryRun && !HasStoryOutput)
                    yield return "* Need story output file on dry run.";

                if (Parameters.Count == 0)
                    yield return String.Format("* No assemblies specified.");

                if (Exception != null)
                    yield return String.Format("* An error occurred when parsing the command line arguments: '{0}'", Exception.Message);

                yield break;
            }
        }

        public override void ShowHelp()
        {
            System.Console.WriteLine();
            System.Console.WriteLine("NBehave-Console [inputfiles] [options]");
            System.Console.WriteLine();
            System.Console.WriteLine("Runs a set of NBehave stories from the console.");
            System.Console.WriteLine();
            System.Console.WriteLine("You may specify one or more assemblies.");
            System.Console.WriteLine();
            System.Console.WriteLine("Options:");
            base.ShowHelp();
            System.Console.WriteLine();
            System.Console.WriteLine("Options that take values may use an equal sign, a colon");
            System.Console.WriteLine("or a space to separate the option from its value.");
            System.Console.WriteLine();
        }
    }
}
