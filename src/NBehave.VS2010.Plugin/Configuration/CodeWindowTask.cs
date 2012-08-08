using System;
using System.ComponentModel.Design;
using Microsoft.VisualStudio.Shell;
using NBehave.Narrator.Framework.Tiny;
using NBehave.VS2010.Plugin.Contracts;
using NBehave.VS2010.Plugin.Domain;
using NBehave.VS2010.Plugin.Tiny;

namespace NBehave.VS2010.Plugin.Configuration
{
    public class CodeWindowTask : ITinyIocInstaller
    {
        private IOutputWindow outputWindow;
        private IServiceProvider serviceProvider;
        private IScenarioRunner scenarioRunner;

        public void Install(TinyIoCContainer container)
        {
            serviceProvider = container.Resolve<IServiceProvider>();
            outputWindow = container.Resolve<IOutputWindow>();
            scenarioRunner = container.Resolve<IScenarioRunner>();

            var mcs = serviceProvider.GetService(typeof (IMenuCommandService)) as OleMenuCommandService;
            if (mcs == null) return;

            var menuCommandId = new CommandID((Identifiers.CommandGroupSet), (int) Identifiers.RunCommandId);
            var menuItem = new MenuCommand(RunCommandOnClick, menuCommandId);
            mcs.AddCommand(menuItem);

            var debugCommandId = new CommandID((Identifiers.CommandGroupSet), (int) Identifiers.DebugCommandId);
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
                scenarioRunner.Run(debug);
            }
            catch (Exception exception)
            {
                outputWindow.WriteLine(exception.ToString());
            }
        }   
    }
}