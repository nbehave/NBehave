using System;
using System.ComponentModel.Design;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;
using NBehave.Narrator.Framework.Tiny;
using NBehave.VS2010.Plugin.Contracts;
using NBehave.VS2010.Plugin.Configuration;
using NBehave.VS2010.Plugin.Domain;
using NBehave.VS2010.Plugin.Tiny;

namespace NBehave.VS2010.Plugin
{

    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", "0.6.0.0", IconResourceID = 400)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(Identifiers.PackageGuidString)]
    [ProvideAutoLoad("{f1536ef8-92ec-443c-9ed7-fdadf150da82}")]
    [ProvideService(typeof(IOutputWindow))]
    [ProvideService(typeof(IVisualStudioService))]
    [ProvideService(typeof(IPluginLogger))]
    public sealed class NBehaveRunnerPackage : Package
    {
        protected override void Initialize()
        {
            base.Initialize();
            var container = new TinyIoCContainer();
            container.Register<IServiceProvider>(this);
            container.Register<IServiceContainer>(this);
            container.Register<IVisualStudioService, VisualStudioService>().AsMultiInstance();

            container.Install(
                new OutputWindowTask(),
                new LoggingTask(),
                new MenuCommandTask(),
                new PublishCrossPackageServicesTask()
                );
        }
    }
}