using System;
using System.Collections;
using System.IO;
using System.Reflection;
using NBehave.Narrator.Framework;

namespace NBehave.Console
{
    public class RemotableStoryRunner : MarshalByRefObject
    {

        public FeatureResults SetupAndRunStories(IEventListener listener, string scenarioFiles, ArrayList assemblyList, bool isDryRun, PlainTextOutput output)
        {
            if (string.IsNullOrEmpty(scenarioFiles))
                throw new ArgumentNullException("scenarioFiles");

            var runner = new TextRunner(listener);
            runner.Load(scenarioFiles.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries));
            runner.IsDryRun = isDryRun;

            foreach (string path in assemblyList)
            {
                try
                {
                    runner.LoadAssembly(path);
                }
                catch (FileNotFoundException)
                {
                    output.WriteLine(string.Format("File not found: {0}", path));
                }
            }

            return runner.Run();
        }

        private string[] _assemblyProbe;

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

    }
}
