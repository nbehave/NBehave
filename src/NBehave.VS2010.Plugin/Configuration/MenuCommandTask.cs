using System;
using System.ComponentModel.Design;
using Microsoft.VisualStudio.Shell;
using NBehave.Narrator.Framework.Tiny;
using NBehave.VS2010.Plugin.Contracts;
using NBehave.VS2010.Plugin.Domain;
using NBehave.VS2010.Plugin.Tiny;

namespace NBehave.VS2010.Plugin.Configuration
{
    internal class MenuCommandTask : ITinyIocInstaller
    {
        private IOutputWindow _outputWindow;
        private IServiceProvider _serviceProvider;
        private IVisualStudioService _visualStudioService;

        public void Install(TinyIoCContainer container)
        {
            _serviceProvider = container.Resolve<IServiceProvider>();
            _visualStudioService = container.Resolve<IVisualStudioService>();
            _outputWindow = container.Resolve<IOutputWindow>();

            var mcs = _serviceProvider.GetService(typeof (IMenuCommandService)) as OleMenuCommandService;
            if (mcs == null) return;

            var menuCommandId = new CommandID((Identifiers.CommandGroupGuid), (int) Identifiers.RunCommandId);
            var menuItem = new MenuCommand(RunCommandOnClick, menuCommandId);
            mcs.AddCommand(menuItem);

            var debugCommandId = new CommandID((Identifiers.CommandGroupGuid), (int) Identifiers.DebugCommandId);
            var debugItem = new MenuCommand(DebugCommandOnClick, debugCommandId);
            mcs.AddCommand(debugItem);
        }

        private void DebugCommandOnClick(object sender, EventArgs e)
        {
            ExecuteScenario(true);
        }

        private void RunCommandOnClick(object sender, EventArgs e)
        {
            ExecuteScenario(false);
        }

        private void ExecuteScenario(bool debug)
        {
            try
            {
                var runner = new ScenarioRunner(_outputWindow, _visualStudioService);
                runner.Run(debug);
            }
            catch (Exception exception)
            {
                _outputWindow.WriteLine(exception.ToString());
            }
        }
    }
}