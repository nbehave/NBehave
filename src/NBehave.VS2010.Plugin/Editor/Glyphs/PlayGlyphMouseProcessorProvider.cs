using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Formatting;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;

namespace NBehave.VS2010.Plugin.Editor.Glyphs
{
    [Export(typeof(IGlyphMouseProcessorProvider))]
    [ContentType("nbehave.gherkin")]
    [TextViewRole(PredefinedTextViewRoles.Editable)]
    [Name("PlayGlyphMouseProcessorProvider")]
    internal sealed class PlayGlyphMouseProcessorProvider : IGlyphMouseProcessorProvider
    {
        [Import]
        private IViewTagAggregatorFactoryService ViewTagAggregatorFactoryService { get; set; }

        public IMouseProcessor GetAssociatedMouseProcessor(IWpfTextViewHost wpfTextViewHost, IWpfTextViewMargin margin)
        {
            return new PlayMouseProcessor(wpfTextViewHost, ViewTagAggregatorFactoryService);
        }
    }

    internal class PlayMouseProcessor : MouseProcessorBase
    {
        private readonly IWpfTextViewHost _wpfTextViewHost;
        private readonly IViewTagAggregatorFactoryService _viewTagAggregatorFactoryService;
        private readonly ITagAggregator<PlayGlyphTag> _createTagAggregator;

        public PlayMouseProcessor(IWpfTextViewHost wpfTextViewHost, IViewTagAggregatorFactoryService viewTagAggregatorFactoryService)
        {
            _wpfTextViewHost = wpfTextViewHost;
            _viewTagAggregatorFactoryService = viewTagAggregatorFactoryService;

            _createTagAggregator = _viewTagAggregatorFactoryService.CreateTagAggregator<PlayGlyphTag>(_wpfTextViewHost.TextView);
        }

        public override void PreprocessMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            IWpfTextView textView = _wpfTextViewHost.TextView;
            Point position = e.GetPosition(textView.VisualElement);

            ITextViewLine textViewLine = textView.TextViewLines
                .GetTextViewLineContainingYCoordinate(position.Y + textView.ViewportTop);
            var lineText = textViewLine.Extent.GetText();

            var tagSpans = _createTagAggregator.GetTags(textViewLine.ExtentAsMappingSpan).ToList();
            var selected = tagSpans.Select(span => span.Tag).ToList();
            PlayGlyphTag tag = selected
                .FirstOrDefault(_ => _.IsScenario(lineText, textViewLine.Start.GetContainingLine().LineNumber + 1));
            if (tag != null)
                tag.Execute(position, textView.VisualElement);
        }
    }
}