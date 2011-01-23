using System;
using System.ComponentModel.Design;
using Microsoft.VisualStudio.Shell;
using NBehave.VS2010.Plugin.Contracts;
using NBehave.VS2010.Plugin.Domain;

namespace NBehave.VS2010.Plugin.Configuration
{
    using Castle.MicroKernel.Registration;
    using Castle.MicroKernel.SubSystems.Configuration;
    using Castle.Windsor;

    internal class MenuCommandTask : IWindsorInstaller
    {
        private IServiceProvider _serviceProvider;
        private IVisualStudioService _visualStudioService;
        private IOutputWindow _outputWindow;

        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            this._serviceProvider = container.Resolve<IServiceProvider>();
            this._visualStudioService = container.Resolve<IVisualStudioService>();
            this._outputWindow = container.Resolve<IOutputWindow>();

            var mcs = this._serviceProvider.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (mcs == null) return;

            var menuCommandId = new CommandID((Identifiers.CommandGroupGuid), (int)Identifiers.RunCommandId);
            var menuItem = new MenuCommand(RunCommandOnClick, menuCommandId);
            mcs.AddCommand(menuItem);

            var debugCommandId = new CommandID((Identifiers.CommandGroupGuid), (int)Identifiers.DebugCommandId);
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
                var runner = new ScenarioRunner(this._outputWindow, this._visualStudioService);
                runner.Run(debug);
            }
            catch (Exception exception)
            {
                this._outputWindow.WriteLine(exception.ToString());
            }
        }
    }
}