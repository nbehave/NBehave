using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace NBehave.VS2010.Plugin.Editor.Domain
{
    internal static class GherkinFileClassificationDefinitions
    {
        [Export]
        [Name("gherkin")]
        [BaseDefinition("text")]
        internal static ContentTypeDefinition GherkinContentTypeDefinition = null;

        [Export]
        [FileExtension(".feature")]
        [ContentType("gherkin")]
        internal static FileExtensionToContentTypeDefinition GherkinFileExtensionDefinition = null;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name("gherkin.keyword")]
        [BaseDefinition("keyword")]
        internal static ClassificationTypeDefinition GherkinKeywordClassifierType = null;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name("gherkin.comment")]
        [BaseDefinition("comment")]
        internal static ClassificationTypeDefinition GherkinCommentClassifierType = null;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name("gherkin.tag")]
        [BaseDefinition("symbol definition")]
        internal static ClassificationTypeDefinition GherkinTagClassifierType = null;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name("gherkin.placeholder")]
        [BaseDefinition("string")]
        internal static ClassificationTypeDefinition GherkinPlaceholderClassifierType = null;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name("gherkin.multilinetext")]
        [BaseDefinition("string")]
        internal static ClassificationTypeDefinition GherkinMultilineTextClassifierType = null;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name("gherkin.tableheader")]
        internal static ClassificationTypeDefinition GherkinTableHeaderClassifierType = null;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name("gherkin.tablecell")]
        [BaseDefinition("string")]
        internal static ClassificationTypeDefinition GherkinTableCellClassifierType = null;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name("gherkin.scenariotitle")]
        internal static ClassificationTypeDefinition GherkinScenarioTitleClassifierType = null;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name("gherkin.featuretitle")]
        internal static ClassificationTypeDefinition GherkinFeatureTitleClassifierType = null;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name("gherkin.description")]
        internal static ClassificationTypeDefinition GherkinDescriptionClassifierType = null;
    }
}
