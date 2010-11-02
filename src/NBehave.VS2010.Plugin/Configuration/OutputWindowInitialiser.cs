using System;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Shell.Interop;
using NBehave.VS2010.Plugin.Contracts;
using NBehave.VS2010.Plugin.Domain;

namespace NBehave.VS2010.Plugin.Configuration
{
    [Export(typeof (IComponentInitialiser))]
    internal class OutputWindowInitialiser : IComponentInitialiser
    {
        [Import]
        public IServiceProvider ServiceProvider { get; set; }

        [Export(typeof (IOutputWindow))]
        public IOutputWindow OutputWindow { get; set; }


        public void Initialise()
        {
            var pane = (IVsOutputWindow) ServiceProvider.GetService(typeof (SVsOutputWindow));

            var myGuidList = Guid.NewGuid();
            pane.CreatePane(myGuidList, "Scenario Results", 1, 1);

            IVsOutputWindowPane outputWindowPane;
            pane.GetPane(myGuidList, out outputWindowPane);

            OutputWindow = new OutputWindow(outputWindowPane);

//            var serviceContainer = _serviceProvider as IServiceContainer;
//            serviceContainer.AddService(typeof(IOutputWindow), OutputWindow);
        }

    }
}