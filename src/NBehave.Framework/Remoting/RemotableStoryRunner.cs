using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using NBehave.Narrator.Framework.Text;

namespace NBehave.Narrator.Framework.Remoting
{
    public class RemotableStoryRunner : MarshalByRefObject, IRunner
    {
        private string[] _assemblyProbe;
        private readonly List<string> _scenarioFiles = new List<string>();
        private readonly List<string> _assemblyLocations = new List<string>();
        private readonly List<Stream> _streams = new List<Stream>();

        public void InitializeDomain(string[] assemblyProbe)
        {
            _assemblyProbe = assemblyProbe;
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
        }

        Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            foreach (string basePath in _assemblyProbe)
            {
                string assemblyFileName = args.Name;
                if (args.Name.Contains(","))
                    assemblyFileName = args.Name.Split(',')[0];
                string assemblyLocation = Path.Combine(basePath, assemblyFileName);
                if (File.Exists(assemblyLocation + ".dll"))
                    return Assembly.LoadFile(assemblyLocation + ".dll");
            }
            return null;
        }

        public IEventListener Listener { get; set; }

        public void Load(IEnumerable<string> scenarioFiles)
        {
            _scenarioFiles.AddRange(scenarioFiles);
        }

        public bool IsDryRun { get; set; }

        public StoryRunnerFilter StoryRunnerFilter { get; set; }

        public void LoadAssembly(string path)
        {
            _assemblyLocations.Add(path);
        }

        public FeatureResults Run()
        {
            return Run(null);
        }

        public void Load(Stream stream)
        {
            _streams.Add(stream);
        }

        public FeatureResults Run(PlainTextOutput output)
        {
            var runner = new TextRunner(Listener);
            runner.Load(_scenarioFiles);
            runner.IsDryRun = IsDryRun;

            foreach (var stream in _streams)
            {
                runner.Load(stream);
            }

            foreach (string path in _assemblyLocations)
            {
                try
                {
                    runner.LoadAssembly(path);
                }
                catch (FileNotFoundException)
                {
                    if(output != null)
                        output.WriteLine(string.Format("File not found: {0}", path));
                }
            }

            return runner.Run();
        }
    }
}
