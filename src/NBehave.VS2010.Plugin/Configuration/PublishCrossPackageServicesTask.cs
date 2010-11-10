using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using NBehave.VS2010.Plugin.Contracts;

namespace NBehave.VS2010.Plugin.Configuration
{
    [Export(typeof(IStartUpTask))]
    public class PublishCrossPackageServicesTask : IStartUpTask
    {
        [Import(typeof(IServiceContainer))]
        public IServiceContainer ServiceContainer { get; set; }

        [Import(typeof(CompositionContainer))]
        public CompositionContainer CompositionContainer { get; set; }

        public void Initialise()
        {
            ServiceCreatorCallback registerOutputWindow = RegisterOutputWindow;
            ServiceContainer.AddService(typeof(IOutputWindow), registerOutputWindow, true);

            ServiceCreatorCallback registerVisualStudioService = RegisterVisualStudioService;
            ServiceContainer.AddService(typeof(IVisualStudioService), registerVisualStudioService, true);

        }

        private object RegisterOutputWindow(IServiceContainer container, Type serviceType)
        {
            return CompositionContainer.GetExport<IOutputWindow>().Value;
        }


        private object RegisterVisualStudioService(IServiceContainer container, Type servicetype)
        {
            return CompositionContainer.GetExport<IVisualStudioService>().Value;
        }
    }
}
