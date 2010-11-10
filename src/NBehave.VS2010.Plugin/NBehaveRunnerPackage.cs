using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;
using NBehave.VS2010.Plugin.Contracts;
using NLog;

namespace NBehave.VS2010.Plugin
{
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", "0.5.0.0", IconResourceID = 400)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(Identifiers.PackageGuidString)]
    [ProvideAutoLoad("{f1536ef8-92ec-443c-9ed7-fdadf150da82}")]
    [ProvideService(typeof(IOutputWindow))] 
    [ProvideService(typeof(IVisualStudioService))] 
    public sealed class NBehaveRunnerPackage : Package
    {
        [Export(typeof (IServiceProvider))]
        public IServiceProvider ServiceProvider { get; set; }

        [Export(typeof(IServiceContainer))]
        public IServiceContainer ServiceContainer { get; set; }

        [Export(typeof(CompositionContainer))]
        public CompositionContainer Container { get; set; }

        [ImportMany(AllowRecomposition = true)]
        public IEnumerable<IStartUpTask> ComponentInitialisers { get; set; }

        [Import(AllowRecomposition = true)]
        public Logger Logger { get; set; }

        protected override void Initialize()
        {
            base.Initialize();
            ServiceProvider = this;
            ServiceContainer = this;

            AppDomain.CurrentDomain.UnhandledException += (sender, unhandledExceptionEventArgs) 
                => this.Logger.FatalException(("unhandled"), (Exception) unhandledExceptionEventArgs.ExceptionObject);

            var catalog = new AssemblyCatalog(typeof (NBehaveRunnerPackage).Assembly);
            Container = new CompositionContainer(catalog);
            Container.ComposeParts(this);

            foreach (var initialiser in ComponentInitialisers)
            {
                initialiser.Initialise();
                Container.ComposeParts(initialiser);
            }
        }
    }
}