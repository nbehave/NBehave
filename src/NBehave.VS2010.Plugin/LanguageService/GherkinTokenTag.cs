using Microsoft.VisualStudio.Text.Tagging;

namespace NBehave.VS2010.Plugin.LanguageService
{
    public class GherkinTokenTag : ITag
    {
        public GherkinTokenType Type { get; private set; }

        public GherkinTokenTag(GherkinTokenType type)
        {
            this.Type = type;
        }
    }
}
