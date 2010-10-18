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
        internal static ContentTypeDefinition GherkinContentTypeDefinition;

        [Export]
        [FileExtension(".feature")]
        [ContentType("gherkin")]
        internal static FileExtensionToContentTypeDefinition GherkinFileExtensionDefinition;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name("gherkin.keyword")]
        [BaseDefinition("keyword")]
        internal static ClassificationTypeDefinition GherkinKeywordClassifierType;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name("gherkin.comment")]
        [BaseDefinition("comment")]
        internal static ClassificationTypeDefinition GherkinCommentClassifierType;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name("gherkin.tag")]
        [BaseDefinition("symbol definition")]
        internal static ClassificationTypeDefinition GherkinTagClassifierType;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name("gherkin.placeholder")]
        [BaseDefinition("string")]
        internal static ClassificationTypeDefinition GherkinPlaceholderClassifierType;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name("gherkin.multilinetext")]
        [BaseDefinition("string")]
        internal static ClassificationTypeDefinition GherkinMultilineTextClassifierType;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name("gherkin.tableheader")]
        internal static ClassificationTypeDefinition GherkinTableHeaderClassifierType;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name("gherkin.tablecell")]
        [BaseDefinition("string")]
        internal static ClassificationTypeDefinition GherkinTableCellClassifierType;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name("gherkin.scenariotitle")]
        internal static ClassificationTypeDefinition GherkinScenarioTitleClassifierType;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name("gherkin.featuretitle")]
        internal static ClassificationTypeDefinition GherkinFeatureTitleClassifierType;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name("gherkin.description")]
        internal static ClassificationTypeDefinition GherkinDescriptionClassifierType;
    }
}
