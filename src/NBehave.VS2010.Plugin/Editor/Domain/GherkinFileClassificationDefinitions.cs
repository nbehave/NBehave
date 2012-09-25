using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace NBehave.VS2010.Plugin.Editor.Domain
{
    internal static class GherkinFileClassificationDefinitions
    {
        [Export]
        [Name("gherkin.syntaxError")]
        [BaseDefinition("text")]
        internal static ClassificationTypeDefinition GherkinSyntaxErrorClassifierType = null;

        [Export]
        [Name("gherkin.feature")]
        [BaseDefinition("text")]
        internal static ClassificationTypeDefinition GherkinFeatureClassifierType = null;

        [Export]
        [Name("gherkin.featureTitle")]
        [BaseDefinition("text")]
        internal static ClassificationTypeDefinition GherkinFeatureTitleClassifierType = null;

        [Export]
        [Name("gherkin.featureDescription")]
        [BaseDefinition("Plaintext")]
        internal static ClassificationTypeDefinition GherkinFeatureDescriptionClassifierType = null;

        [Export]
        [Name("gherkin.scenario")]
        [BaseDefinition("text")]
        internal static ClassificationTypeDefinition GherkinScenarioClassifierType = null;

        [Export]
        [Name("gherkin.scenarioTitle")]
        [BaseDefinition("text")]
        internal static ClassificationTypeDefinition GherkinScenarioTitleClassifierType = null;

        [Export]
        [Name("gherkin.background")]
        [BaseDefinition("text")]
        internal static ClassificationTypeDefinition GherkinBackgroundClassifierType = null;

        [Export]
        [Name("gherkin.backgroundTitle")]
        [BaseDefinition("text")]
        internal static ClassificationTypeDefinition GherkinBackgroundTitleClassifierType = null;

        [Export]
        [Name("gherkin.comment")]
        [BaseDefinition("text")]
        internal static ClassificationTypeDefinition GherkinCommentClassifierType = null;

        [Export]
        [Name("gherkin.tag")]
        [BaseDefinition("text")]
        internal static ClassificationTypeDefinition GherkinTagClassifierType = null;

        [Export]
        [Name("gherkin.docstring")]
        [BaseDefinition("text")]
        internal static ClassificationTypeDefinition GherkinDocstringClassifierType = null;

        [Export]
        [Name("gherkin.examples")]
        [BaseDefinition("text")]
        internal static ClassificationTypeDefinition GherkinExamplesClassifierType = null;

        [Export]
        [Name("gherkin.step")]
        [BaseDefinition("text")]
        internal static ClassificationTypeDefinition GherkinStepClassifierType = null;

        [Export]
        [Name("gherkin.stepText")]
        [BaseDefinition("text")]
        internal static ClassificationTypeDefinition GherkinStepTextClassifierType = null;

        [Export]
        [Name("gherkin.table")]
        [BaseDefinition("text")]
        internal static ClassificationTypeDefinition GherkinTableClassifierType = null;

        [Export]
        [Name("gherkin.tableheader")]
        [BaseDefinition("text")]
        internal static ClassificationTypeDefinition GherkinTableHeaderClassifierType = null;

        [Export]
        [Name("gherkin.tablecell")]
        [BaseDefinition("text")]
        internal static ClassificationTypeDefinition GherkinTableCellClassifierType = null;

        [Export]
        [Name("gherkin.tablecellAlt")]
        [BaseDefinition("text")]
        internal static ClassificationTypeDefinition GherkinTableCellAltClassifierType = null;
    }
}
