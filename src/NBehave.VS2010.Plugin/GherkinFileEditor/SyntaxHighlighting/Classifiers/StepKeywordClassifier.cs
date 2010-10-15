using System.ComponentModel.Composition;

namespace NBehave.VS2010.Plugin.GherkinFileEditor.SyntaxHighlighting.Classifiers
{
    [Export(typeof(IGherkinClassifier))]
    public class StepKeywordClassifier : GherkinClassifierBase
    {
        public override bool CanClassify(ParserEvent parserEvent)
        {
            return parserEvent.EventType == ParserEventType.Step;
        }

        public override void RegisterClassificationDefinitions()
        {
            Register(@event => GetKeywordSpan(@event));
        }
    }
}