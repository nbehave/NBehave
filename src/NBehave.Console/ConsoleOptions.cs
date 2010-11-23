using Codeblast;

namespace NBehave.Console
{
    public class ConsoleOptions : CommandLineOptions
    {
        private bool _isInvalid;

        [Option(Short = "sf", Description = "Scenario files(s) to run. Ex: scenarioFiles:scenario1.txt;scenario2.txt")]
        public string scenarioFiles;

        [Option(Short = "x", Description = "Xml file to receive story output")]
        public string xml;

        [Option(Short = "o", Description = "File to receive story output")]
        public string storyOutput;

        [Option(Short = "?", Description = "Display help")]
        public bool help;
        
        [Option(Short = "p", Description = "Pause console after running")]
        public bool pause;

        [Option(Short = "d", Description = "Do not execute actions and output all story text to <storyOutput>")]
        public bool dryRun;

        [Option(Description = "Do not display the logo")]
        public bool nologo;

        [Option(Short = "c", Description = "Generate stub methods for pending steps")] 
        public bool codegen;

        [Option(Short = "wd", Description = "Block the process until a debugger is attached")]
        public bool waitForDebugger;

        public ConsoleOptions(params string[] args)
            : base(args)
        {
            _isInvalid = false;
        }

        public bool HasStoryOutput
        {
            get
            {
                return !string.IsNullOrEmpty(storyOutput);
            }
        }

        public bool HasStoryXmlOutput
        {
            get
            {
                return !string.IsNullOrEmpty(xml);
            }
        }

        public bool IsInvalid
        {
            get { return _isInvalid; }
        }

        public override void Help()
        {

            System.Console.WriteLine();
            System.Console.WriteLine("NBehave-Console [inputfiles] [options]");
            System.Console.WriteLine();
            System.Console.WriteLine("Runs a set of NBehave stories from the console.");
            System.Console.WriteLine();
            System.Console.WriteLine("You may specify one or more assemblies.");
            System.Console.WriteLine();
            System.Console.WriteLine("Options:");
            base.Help();
            System.Console.WriteLine();
            System.Console.WriteLine("Options that take values may use an equal sign, a colon");
            System.Console.WriteLine("or a space to separate the option from its value.");
            System.Console.WriteLine();
        }

        public bool Validate()
        {
            if (_isInvalid)
                return false;

            if (scenarioFiles == null)
                return false;

            if (dryRun && !(HasStoryOutput || HasStoryXmlOutput))
                return false;

            return ParameterCount >= 1;
        }

        protected override void InvalidOption(string name)
        {
            _isInvalid = true;
        }
    }
}
