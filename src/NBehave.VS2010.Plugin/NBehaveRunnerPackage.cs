using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;
using NBehave.VS2010.Plugin.Contracts;

namespace NBehave.VS2010.Plugin
{
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", "0.5.1.0", IconResourceID = 400)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(Identifiers.PackageGuidString)]
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
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}