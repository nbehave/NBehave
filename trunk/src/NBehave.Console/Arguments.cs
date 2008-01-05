using System;
using System.Collections.Generic;

namespace NBehave.Console
{
    internal class Arguments
    {
        private bool _showHelp = false;
        private List<string> _assemblyPaths = new List<string>();
        private string _storyOutput;

        public Arguments(string[] args)
        {
            ParseArguments(args);
        }

        public bool ShowHelp
        {
            get { return _showHelp; }
        }

        public IEnumerable<string> AssemblyPaths
        {
            get
            {
                foreach (string path in _assemblyPaths)
                {
                    yield return path;
                }
            }
        }

        public string StoryOutput
        {
            get { return _storyOutput; }
        }

        private void ParseArguments(string[] args)
        {
            if ((args == null) || (args.Length == 0))
            {
                _showHelp = true;
                return;
            }

            foreach (string arg in args)
            {
                if (arg.StartsWith("/"))
                {
                    string[] split = arg.Split(':');

                    if (split.Length < 2)
                        throw new ArgumentException(string.Format("Argument not specified for '{0}'", split[0]), "args");

                    string command = split[0];
                    string argument = split[1];

                    if (command.Equals("/storyOutput", StringComparison.CurrentCultureIgnoreCase))
                    {
                        _storyOutput = argument;
                    }
                }
                else
                {
                    _assemblyPaths.Add(arg);
                }
            }
        }
    }
}
