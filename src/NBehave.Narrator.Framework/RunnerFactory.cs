using System;
using System.IO;
using System.Linq;
using System.Reflection;
using NBehave.Narrator.Framework.Remoting;

namespace NBehave.Narrator.Framework
{
    public static class RunnerFactory
    {
        public const string AppDomainName = "NBehave story runner";

        public static IRunner CreateTextRunner(NBehaveConfiguration configuration)
        {
            var assemblyWithConfigFile = configuration.Assemblies
                                                      .Where(path => File.Exists(path + ".config"))
                                                      .Select(path => path + ".config")
                                                      .FirstOrDefault();
            if (assemblyWithConfigFile == null)
                assemblyWithConfigFile = configuration.Assemblies.First() + ".config";

            if (configuration.CreateAppDomain==false)
                return new TextRunner(configuration);

            // Create the AppDomain
            return CreateRunnerInNewAppDomain(configuration, assemblyWithConfigFile);
        }

        private static IRunner CreateRunnerInNewAppDomain(NBehaveConfiguration configuration, string assemblyWithConfigFile)
        {
            var configFileInfo = new FileInfo(assemblyWithConfigFile);
            var ads = new AppDomainSetup
                          {
                              ConfigurationFile = configFileInfo.Name,
                              ApplicationBase = configFileInfo.DirectoryName,
                              ShadowCopyDirectories = configFileInfo.DirectoryName,
                              ShadowCopyFiles = "true"
                          };
            AppDomain ad = AppDomain.CreateDomain(AppDomainName, null, ads);

            // Load up our bootstrapper class into the remote AppDomain
            var bootstrapper = (AppDomainBootstrapper) ad.CreateInstanceFromAndUnwrap(typeof (AppDomainBootstrapper).Assembly.Location, typeof (AppDomainBootstrapper).FullName);

            var directoryName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            bootstrapper.InitializeDomain(new[]
                                              {
                                                  directoryName,
                                                  configFileInfo.DirectoryName
                                              });

            // And now we can create a remote story runner
            var runner = (RemotableStoryRunner) ad.CreateInstanceAndUnwrap(Assembly.GetExecutingAssembly().FullName, typeof (RemotableStoryRunner).FullName);
            runner.Initialise(configuration);
            return runner;
        }
    }
}