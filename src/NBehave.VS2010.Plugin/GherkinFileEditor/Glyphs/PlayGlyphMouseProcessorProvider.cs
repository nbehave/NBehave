using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;

namespace NBehave.VS2010.Plugin.GherkinFileEditor.Glyphs
{
    [Export(typeof(IGlyphMouseProcessorProvider))]
    [ContentType("gherkin")]
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
        private ITagAggregator<PlayTag> _createTagAggregator;

        public PlayMouseProcessor(IWpfTextViewHost wpfTextViewHost, IViewTagAggregatorFactoryService viewTagAggregatorFactoryService)
        {
            _wpfTextViewHost = wpfTextViewHost;
            _viewTagAggregatorFactoryService = viewTagAggregatorFactoryService;

            _createTagAggregator = _viewTagAggregatorFactoryService.CreateTagAggregator<PlayTag>(_wpfTextViewHost.TextView);
        }

        public override void PreprocessMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            IWpfTextView textView = this._wpfTextViewHost.TextView;
            Point position = e.GetPosition(textView.VisualElement);

            var textViewLine = 
                textView.TextViewLines.GetTextViewLineContainingYCoordinate(position.Y + textView.ViewportTop);

            var tags = this._createTagAggregator.GetTags(textViewLine.ExtentAsMappingSpan);
            var glyphs = tags.Select(span => span.Tag);
            glyphs.First().Execute();
        }
    }
}