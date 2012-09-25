using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text;
using NBehave.VS2010.Plugin.Editor;
using NBehave.VS2010.Plugin.Editor.Glyphs;

namespace NBehave.VS2010.Plugin.Tagging
{
    [Export(typeof(TokenFactory))]
    public class TokenFactory
    {
        [Import]
        internal ServiceRegistrar ServiceRegistrar = null;

        public TokenParser BuildTokenParser(ITextBuffer buffer)
        {
            ServiceRegistrar.Initialise(buffer);

            TokenParser parser;
            if (buffer.Properties.TryGetProperty(typeof(TokenParser), out parser))
                return parser;
            parser = new TokenParser(buffer);
            buffer.Properties.AddProperty(typeof(TokenParser), parser);
            return parser;
        }

        public PlayTagger BuildPlayTagger(ITextBuffer buffer)
        {
            PlayTagger tagger;
            if (buffer.Properties.TryGetProperty(typeof(PlayTagger), out tagger))
                return tagger;
            tagger = new PlayTagger(BuildTokenParser(buffer));
            buffer.Properties.AddProperty(typeof(PlayTagger), tagger);
            return tagger;
        }
    }
}