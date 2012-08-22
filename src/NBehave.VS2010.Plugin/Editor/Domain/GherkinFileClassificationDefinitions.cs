using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace NBehave.VS2010.Plugin.Editor.Domain
{
    internal static class GherkinFileClassificationDefinitions
    {
        [Export(typeof(ClassificationTypeDefinition))]
        [Name("gherkin.syntaxError")]
        internal static ClassificationTypeDefinition GherkinSyntaxErrorClassifierType = null;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name("gherkin.feature")]
        internal static ClassificationTypeDefinition GherkinFeatureClassifierType = null;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name("gherkin.featureTitle")]
        internal static ClassificationTypeDefinition GherkinFeatureTitleClassifierType = null;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name("gherkin.featureDescription")]
        internal static ClassificationTypeDefinition GherkinFeatureDescriptionClassifierType = null;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name("gherkin.scenario")]
        internal static ClassificationTypeDefinition GherkinScenarioClassifierType = null;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name("gherkin.scenarioTitle")]
        internal static ClassificationTypeDefinition GherkinScenarioTitleClassifierType = null;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name("gherkin.background")]
        internal static ClassificationTypeDefinition GherkinBackgroundClassifierType = null;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name("gherkin.backgroundTitle")]
        internal static ClassificationTypeDefinition GherkinBackgroundTitleClassifierType = null;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name("gherkin.comment")]
        //[BaseDefinition("comment")]
        internal static ClassificationTypeDefinition GherkinCommentClassifierType = null;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name("gherkin.tag")]
        //[BaseDefinition("symbol definition")]
        internal static ClassificationTypeDefinition GherkinTagClassifierType = null;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name("gherkin.docstring")]
        //[BaseDefinition("string")]
        internal static ClassificationTypeDefinition GherkinDocstringClassifierType = null;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name("gherkin.examples")]
        //[BaseDefinition("string")]
        internal static ClassificationTypeDefinition GherkinExamplesClassifierType = null;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name("gherkin.step")]
        //[BaseDefinition("keyword")]
        internal static ClassificationTypeDefinition GherkinStepClassifierType = null;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name("gherkin.stepText")]
        //[BaseDefinition("keyword")]
        internal static ClassificationTypeDefinition GherkinStepTextClassifierType = null;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name("gherkin.table")]
        internal static ClassificationTypeDefinition GherkinTableClassifierType = null;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name("gherkin.tableheader")]
        internal static ClassificationTypeDefinition GherkinTableHeaderClassifierType = null;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name("gherkin.tablecell")]
        //[BaseDefinition("string")]
        internal static ClassificationTypeDefinition GherkinTableCellClassifierType = null;
    }
}
