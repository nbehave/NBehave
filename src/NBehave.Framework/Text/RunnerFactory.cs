using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using NBehave.Narrator.Framework.Remoting;

namespace NBehave.Narrator.Framework.Text
{
    public static class RunnerFactory
    {
        public static IRunner CreateTextRunner(IEnumerable<string> assemblyPaths, IEventListener listener)
        {
            var assemblyWithConfigFile = assemblyPaths.Where(path => File.Exists(path + ".config"))
                                                      .Select(path => path + ".config")
                                                      .FirstOrDefault();
            if (assemblyWithConfigFile == null)
            {
                // Easy case - there are no config files involved, so we can run the stories in the current AppDomain safely
                return new TextRunner(listener);
            }

            // Create the AppDomain
            var configFileInfo = new FileInfo(assemblyWithConfigFile);
            var ads = new AppDomainSetup
                          {
                              ConfigurationFile = configFileInfo.Name,
                              ApplicationBase = configFileInfo.DirectoryName,
                              ShadowCopyDirectories = configFileInfo.DirectoryName,
                              ShadowCopyFiles = "true"
                          };
            AppDomain ad = AppDomain.CreateDomain("NBehave story runner", null, ads);

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
            runner.Listener = new DelegatingListener(listener);
            return runner;
        }
    }
}