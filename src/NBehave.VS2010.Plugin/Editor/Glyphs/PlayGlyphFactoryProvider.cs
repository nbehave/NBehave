using System.ComponentModel.Composition;
using System.Windows;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Formatting;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;
using NBehave.VS2010.Plugin.Editor.Glyphs.Views;

namespace NBehave.VS2010.Plugin.Editor.Glyphs
{
    [Export(typeof(IGlyphFactoryProvider))]
    [Name("PlayGlyph")]
    [Order(After = "VsTextMarker")]
    [ContentType("gherkin")]
    [TagType(typeof(PlayGlyphTag))]
    internal sealed class PlayGlyphFactoryProvider : IGlyphFactoryProvider
    {
        /// <summary>
        /// This method creates an instance of our custom glyph factory for a given text view.
        /// </summary>
        /// <param name="view">The text view we are creating a glyph factory for, we don't use this.</param>
        /// <param name="margin">The glyph margin for the text view, we don't use this.</param>
        /// <returns>An instance of our custom glyph factory.</returns>
        public IGlyphFactory GetGlyphFactory(IWpfTextView view, IWpfTextViewMargin margin)
        {
            return new PlayGlyphFactory();
        }
    }

    internal class PlayGlyphFactory : IGlyphFactory
    {
        public UIElement GenerateGlyph(IWpfTextViewLine line, IGlyphTag tag)
        {
            if (tag == null || !(tag is PlayGlyphTag))
            {
                return null;
            }

            return new PlayGlyph();
        }
    }
}