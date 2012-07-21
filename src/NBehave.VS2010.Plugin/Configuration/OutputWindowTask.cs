using System;
using Microsoft.VisualStudio.Shell.Interop;
using NBehave.Narrator.Framework.Tiny;
using NBehave.VS2010.Plugin.Contracts;
using NBehave.VS2010.Plugin.Domain;
using NBehave.VS2010.Plugin.Tiny;

namespace NBehave.VS2010.Plugin.Configuration
{
    internal class OutputWindowTask : ITinyIocInstaller
    {
        public void Install(TinyIoCContainer container)
        {
            var serviceProvider = container.Resolve<IServiceProvider>();
            var pane = (IVsOutputWindow)serviceProvider.GetService(typeof(SVsOutputWindow));

            var myGuidList = Guid.NewGuid();
            pane.CreatePane(myGuidList, "NBehave", 1, 1);

            IVsOutputWindowPane outputWindowPane;
            pane.GetPane(myGuidList, out outputWindowPane);

            var outputWindow = new OutputWindow(outputWindowPane);
            container.Register<IOutputWindow>(outputWindow);
        }
    }
}