using System.ComponentModel.Composition;

namespace NBehave.VS2010.Plugin.GherkinFileEditor.SyntaxHighlighting.Classifiers
{
    [Export(typeof(IGherkinClassifier))]
    public class ScenarioClassifier : GherkinClassifierBase
    {
        public override bool CanClassify(ParserEvent parserEvent)
        {
            return parserEvent.EventType == ParserEventType.Scenario;
        }

        public override void RegisterClassificationDefinitions()
        {
            Register(GetKeywordSpan);
            Register(@event => GetTitleSpan(@event, ClassificationRegistry.ScenarioTitle));
        }
    }
}