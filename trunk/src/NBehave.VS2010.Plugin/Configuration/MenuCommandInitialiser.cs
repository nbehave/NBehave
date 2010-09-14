using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Design;
using Microsoft.VisualStudio.Shell;
using NBehave.VS2010.Plugin.Contracts;
using NBehave.VS2010.Plugin.Domain;

namespace NBehave.VS2010.Plugin.Configuration
{
    [Export(typeof (IComponentInitialiser))]
    internal class MenuCommandInitialiser : IComponentInitialiser
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IVisualStudioService _visualStudioService;

        [ImportingConstructor]
        public MenuCommandInitialiser(IServiceProvider serviceProvider, IVisualStudioService visualStudioService)
        {
            _serviceProvider = serviceProvider;
            _visualStudioService = visualStudioService;
        }

        [Import(AllowRecomposition = true)]
        public IOutputWindow OutputWindow { get; set; }

        #region IComponentInitialiser Members

        public void Initialise()
        {
            var mcs = _serviceProvider.GetService(typeof (IMenuCommandService)) as OleMenuCommandService;
            if (mcs == null) return;
            
            var menuCommandId = new CommandID((Identifiers.CommandGroupGuid), (int) Identifiers.RunCommandId);
            var menuItem = new MenuCommand(RunCommandOnClick, menuCommandId);
            mcs.AddCommand(menuItem);

            var debugCommandId = new CommandID((Identifiers.CommandGroupGuid), (int) Identifiers.DebugCommandId);
            var debugItem = new MenuCommand(DebugCommandOnClick, debugCommandId);
            mcs.AddCommand(debugItem);
        }

        #endregion

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
                var runner = new ScenarioRunner(OutputWindow, _visualStudioService);
                runner.Run(debug);
            }
            catch (Exception exception)
            {
                OutputWindow.WriteLine(exception.ToString());
            }
        }
    }
}