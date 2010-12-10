using System;
using System.IO;
using System.Reflection;

namespace NBehave.Narrator.Framework.Remoting
{
    public class AppDomainBootstrapper : MarshalByRefObject
    {
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
                if (File.Exists(assemblyLocation + ".exe"))
                    return Assembly.LoadFile(assemblyLocation + ".exe");
            }
            return null;

        }

    }
}