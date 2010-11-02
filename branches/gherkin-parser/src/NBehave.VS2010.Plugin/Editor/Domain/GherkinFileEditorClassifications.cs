using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text.Classification;

namespace NBehave.VS2010.Plugin.Editor.Domain
{
    [Export(typeof(GherkinFileEditorClassifications))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class GherkinFileEditorClassifications
    {
        [Import]
        public IClassificationTypeRegistryService ClassificationRegistry { get; set; }

        public IClassificationType Keyword
        {
            get { return ClassificationRegistry.GetClassificationType("gherkin.keyword"); }
        }

        public IClassificationType Comment
        {
            get { return ClassificationRegistry.GetClassificationType("gherkin.comment"); }
        }
        public IClassificationType Tag{
            get { return ClassificationRegistry.GetClassificationType("gherkin.tag"); }
        }
        public IClassificationType MultilineText{
            get { return ClassificationRegistry.GetClassificationType("gherkin.multilinetext"); }
        }
        public IClassificationType Placeholder{
            get { return ClassificationRegistry.GetClassificationType("gherkin.placeholder"); }
        }
        public IClassificationType ScenarioTitle{
            get { return ClassificationRegistry.GetClassificationType("gherkin.scenariotitle"); }
        }
        public IClassificationType FeatureTitle{
            get { return ClassificationRegistry.GetClassificationType("gherkin.featuretitle"); }
        }
        public IClassificationType TableCell{
            get { return ClassificationRegistry.GetClassificationType("gherkin.tablecell"); }
        }
        public IClassificationType TableHeader{
            get { return ClassificationRegistry.GetClassificationType("gherkin.tableheader"); }
        }
        public IClassificationType Description{
            get { return ClassificationRegistry.GetClassificationType("gherkin.description"); }
        }
    }
}