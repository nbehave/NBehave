using System;
using System.IO;
using System.Reflection;
using NBehave.Configuration;
using NBehave.Internal;

namespace NBehave.Remoting
{
    public class AppDomainRunner : IRunner
    {
        private readonly NBehaveConfiguration configuration;
        private readonly string assemblyWithConfigFile;

        public AppDomainRunner(NBehaveConfiguration configuration, string assemblyWithConfigFile)
        {
            this.configuration = configuration;
            this.assemblyWithConfigFile = assemblyWithConfigFile;
        }

        public const string AppDomainName = "NBehave story runner";

        public FeatureResults Run()
        {
            var appDomain = CreateAppDomain();
            var runner = CreateRunnerInNewAppDomain(appDomain);
            FeatureResults results;
            try
            {
                results = runner.Run();
            }
            finally
            {
                AppDomain.Unload(appDomain);
            }
            return results;
        }

        private IRunner CreateRunnerInNewAppDomain(AppDomain appDomain)
        {
            var runner = (RemotableStoryRunner)appDomain.CreateInstanceAndUnwrap(Assembly.GetExecutingAssembly().FullName, typeof(RemotableStoryRunner).FullName);
            runner.Initialise(configuration);
            return runner;
        }

        private AppDomain CreateAppDomain()
        {
            var configFileInfo = new FileInfo(assemblyWithConfigFile);
            var ads = new AppDomainSetup
                          {
                              ConfigurationFile = configFileInfo.Name,
                              ApplicationBase = configFileInfo.DirectoryName,
                              ShadowCopyDirectories = configFileInfo.DirectoryName,
                              ShadowCopyFiles = "true"
                          };
            var appDomain = AppDomain.CreateDomain(AppDomainName, null, ads);

            // Load up our bootstrapper class into the remote AppDomain
            var bootstrapper = (AppDomainBootstrapper)appDomain.CreateInstanceFromAndUnwrap(typeof(AppDomainBootstrapper).Assembly.Location, typeof(AppDomainBootstrapper).FullName);

            var directoryName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            bootstrapper.InitializeDomain(new[]
                                              {
                                                  directoryName,
                                                  configFileInfo.DirectoryName
                                              });
            return appDomain;
        }
    }
}