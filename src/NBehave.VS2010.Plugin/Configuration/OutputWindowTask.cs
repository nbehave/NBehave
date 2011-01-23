using System;
using Microsoft.VisualStudio.Shell.Interop;
using NBehave.VS2010.Plugin.Contracts;
using NBehave.VS2010.Plugin.Domain;

namespace NBehave.VS2010.Plugin.Configuration
{
    using Castle.MicroKernel.Registration;
    using Castle.MicroKernel.SubSystems.Configuration;
    using Castle.Windsor;

    internal class OutputWindowTask : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            var serviceProvider = container.Resolve<IServiceProvider>();
            var pane = (IVsOutputWindow)serviceProvider.GetService(typeof(SVsOutputWindow));

            var myGuidList = Guid.NewGuid();
            pane.CreatePane(myGuidList, "Scenario Results", 1, 1);

            IVsOutputWindowPane outputWindowPane;
            pane.GetPane(myGuidList, out outputWindowPane);

            var outputWindow = new OutputWindow(outputWindowPane);
            container.Register(Component.For<IOutputWindow>().Instance(outputWindow));
        }
    }
}