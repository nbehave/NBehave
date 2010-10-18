using System.ComponentModel.Composition;
using NBehave.VS2010.Plugin.Editor.Domain;

namespace NBehave.VS2010.Plugin.Editor.SyntaxHighlighting.Classifiers
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
            Register(parserEvent => GetKeywordSpan(parserEvent));
            Register(parserEvent => GetTitleSpan(parserEvent, ClassificationRegistry.ScenarioTitle));
        }
    }
}