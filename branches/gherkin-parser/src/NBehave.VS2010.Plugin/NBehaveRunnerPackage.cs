using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.ComponentModel.Design;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.ExtensibilityHosting;
using Microsoft.VisualStudio.Shell;
using NBehave.VS2010.Plugin.Contracts;
using NBehave.VS2010.Plugin.Domain;

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
        private CompositionContainer _container;

        [Export(typeof (IServiceProvider))]
        public IServiceProvider ServiceProvider { get; set; }

        [ImportMany(AllowRecomposition = true)]
        public IEnumerable<IComponentInitialiser> ComponentInitialisers { get; set; }

        protected override void Initialize()
        {
            base.Initialize();
            ServiceProvider = this;

            try
            {
                var catalog = new AssemblyCatalog(typeof (NBehaveRunnerPackage).Assembly);
                _container = new CompositionContainer(catalog);
                _container.ComposeParts(this);

                foreach (var initialiser in ComponentInitialisers)
                {
                    initialiser.Initialise();
                    _container.ComposeParts(initialiser);
                }

                IServiceContainer serviceContainer = this as IServiceContainer;
                ServiceCreatorCallback registerOutputWindow = RegisterOutputWindow;
                serviceContainer.AddService(typeof(IOutputWindow), registerOutputWindow, true);

                ServiceCreatorCallback registerVisualStudioService = RegisterVisualStudioService;
                serviceContainer.AddService(typeof(IVisualStudioService), registerVisualStudioService, true);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private object RegisterVisualStudioService(IServiceContainer container, Type servicetype)
        {
            return _container.GetExport<IVisualStudioService>().Value;
        }

        private object RegisterOutputWindow(IServiceContainer container, Type serviceType)
        {
            return _container.GetExport<IOutputWindow>().Value;
        }
    }
}