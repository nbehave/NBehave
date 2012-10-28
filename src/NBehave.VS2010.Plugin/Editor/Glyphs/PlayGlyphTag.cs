using System.Windows;
using Microsoft.VisualStudio.Text.Editor;
using NBehave.VS2010.Plugin.Editor.Domain;
using NBehave.VS2010.Plugin.Editor.Glyphs.ViewModels;
using NBehave.VS2010.Plugin.Editor.Glyphs.Views;
using NBehave.VS2010.Plugin.Tagging;

namespace NBehave.VS2010.Plugin.Editor.Glyphs
{
    public class PlayGlyphTag : IGlyphTag
    {
        private readonly IGherkinText gherkinText;
        private RunOrDebugViewModel viewModel;

        public PlayGlyphTag(IGherkinText gherkinText)
        {
            this.gherkinText = gherkinText;
        }

        public void Execute(Point position, FrameworkElement visualElement)
        {
            var runOrDebugView = new RunOrDebugView();
            if (viewModel == null)
            {
                viewModel = runOrDebugView.DataContext as RunOrDebugViewModel;
            }
            viewModel.InitialiseProperties(position, visualElement, runOrDebugView, gherkinText);
            viewModel.Show();
        }

        public bool IsScenario(string scenarioTitle, int lineNumber)
        {
            var title = scenarioTitle.Trim(WhiteSpaces.Chars);
            return lineNumber == gherkinText.SourceLine && title.EndsWith(gherkinText.Title);
        }

        public string GetText()
        {
            return gherkinText.AsString;
        }
    }
}