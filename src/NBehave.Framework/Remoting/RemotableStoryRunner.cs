using System;
using System.IO;
using System.Reflection;

namespace NBehave.Narrator.Framework.Remoting
{
    public class RemotableStoryRunner : MarshalByRefObject, IRunner
    {
        private string[] _assemblyProbe;
        private NBehaveConfiguration _configuration;

        public RemotableStoryRunner(NBehaveConfiguration configuration)
        {
            _configuration = configuration;
        }

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

        public FeatureResults Run()
        {
            return Run(null);
        }

        public FeatureResults Run(PlainTextOutput output)
        {
            return new TextRunner(_configuration).Run();
        }
    }
}
