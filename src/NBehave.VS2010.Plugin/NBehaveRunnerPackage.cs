using System;
using System.ComponentModel.Design;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;
using NBehave.VS2010.Plugin.Contracts;

namespace NBehave.VS2010.Plugin
{
    using Castle.MicroKernel.Registration;
    using Castle.Windsor;

    using NBehave.VS2010.Plugin.Configuration;
    using NBehave.VS2010.Plugin.Domain;

    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", "0.5.0.0", IconResourceID = 400)]
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

            var container = new WindsorContainer();
            container.Register(Component.For<IServiceProvider, IServiceContainer>().Instance(this).LifeStyle.Singleton);
            container.Register(Component.For<IVisualStudioService>().ImplementedBy<VisualStudioService>().LifeStyle.Singleton);
            
            /*
             * This is an ordered list.
             */
            container.Install(
                new OutputWindowTask(),
                new LoggingTask(),
                new MenuCommandTask(),
                new PublishCrossPackageServicesTask()
                );
        }
    }
}