using System.Windows;
using Microsoft.VisualStudio.Text.Editor;
using NBehave.VS2010.Plugin.Editor.Glyphs.ViewModels;
using NBehave.VS2010.Plugin.Editor.Glyphs.Views;

namespace NBehave.VS2010.Plugin.Editor.Glyphs
{
    public class PlayGlyphTag : IGlyphTag
    {
        public string ScenarioText { get; set; }
        private RunOrDebugViewModel viewModel;

        public PlayGlyphTag(string scenarioText)
        {
            ScenarioText = scenarioText;
        }

        public void Execute(Point position, FrameworkElement visualElement)
        {
            if (viewModel == null)
            {
                var runOrDebugView = new RunOrDebugView();
                viewModel = runOrDebugView.DataContext as RunOrDebugViewModel;
                viewModel.InitialiseProperties(position, visualElement, runOrDebugView, ScenarioText);
            }

            viewModel.Show();
        }
    }
}