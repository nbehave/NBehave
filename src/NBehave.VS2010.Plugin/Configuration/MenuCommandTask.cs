using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text.Editor;
using NBehave.Narrator.Framework.Tiny;
using NBehave.VS2010.Plugin.Contracts;
using NBehave.VS2010.Plugin.Domain;
using NBehave.VS2010.Plugin.Tiny;

namespace NBehave.VS2010.Plugin.Configuration
{
    public class MenuCommandTask : ITinyIocInstaller, IDisposable
    {
        private IServiceProvider serviceProvider;
        private OleMenuCommandService menuCommandService;
        private IPluginConfiguration pluginConfiguration;
        private ISolutionEventsListener solutionEvents;

        private bool disposed;
        private MenuCommand toggleCreateHtmlReportButton;

        public void Install(TinyIoCContainer container)
        {
            serviceProvider = container.Resolve<IServiceProvider>();

            menuCommandService = serviceProvider.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (menuCommandService == null) return;
            pluginConfiguration = container.Resolve<IPluginConfiguration>();
            pluginConfiguration.PropertyChanged += SetButtonState;
            solutionEvents = container.Resolve<ISolutionEventsListener>();
            solutionEvents.AfterSolutionLoaded += SolutionLoaded;
            solutionEvents.BeforeSolutionClosed += SolutionClosed;
            var vs = container.Resolve<IVisualStudioService>();
            InstallMenuCommands(vs.IsSolutionOpen);
        }

        private void SetButtonState(object sender, PropertyChangedEventArgs e)
        {
            toggleCreateHtmlReportButton.Checked = pluginConfiguration.CreateHtmlReport;
        }

        private void InstallMenuCommands(bool enabled)
        {
            var menuCmdId = new CommandID(Identifiers.TopLevelMenuCmdSet, Identifiers.MenuCommandHtmlReportToggleId);
            toggleCreateHtmlReportButton = new MenuCommand(ToggleCreateHtmlReport, menuCmdId)
                {
                    Checked = pluginConfiguration.CreateHtmlReport && enabled,
                    Enabled = enabled
                };
            menuCommandService.AddCommand(toggleCreateHtmlReportButton);
        }

        private void ToggleCreateHtmlReport(object sender, EventArgs e)
        {
            var cmd = (MenuCommand)sender;
            pluginConfiguration.CreateHtmlReport = !pluginConfiguration.CreateHtmlReport;
            cmd.Checked = pluginConfiguration.CreateHtmlReport;
        }


        private void SolutionLoaded()
        {
            toggleCreateHtmlReportButton.Checked = pluginConfiguration.CreateHtmlReport;
            toggleCreateHtmlReportButton.Enabled = true;
        }

        private void SolutionClosed()
        {
            toggleCreateHtmlReportButton.Checked = false;
            toggleCreateHtmlReportButton.Enabled = false;
        }


        public void Dispose()
        {
            if (!disposed)
            {
                disposed = true;
                if (solutionEvents != null)
                {
                    solutionEvents.AfterSolutionLoaded -= SolutionLoaded;
                    solutionEvents.BeforeSolutionClosed -= SolutionClosed;
                }
            }
            GC.SuppressFinalize(this);
        }
    }
}