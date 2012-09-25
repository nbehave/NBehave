using System.ComponentModel.Composition;
using System.Windows.Media;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace NBehave.VS2010.Plugin.Editor.Domain
{
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "gherkin.syntaxError")]
    [Name("gherkin.syntaxError")]
    [UserVisible(false)]
    [Order(Before = Priority.Default)]
    internal sealed class GherkinSyntaxErrorClassificationFormat : ClassificationFormatDefinition
    {
        public GherkinSyntaxErrorClassificationFormat()
        {
            DisplayName = "Gherkin syntax error";
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "gherkin.feature")]
    [Name("gherkin.feature")]
    [UserVisible(true)]
    [Order(Before = Priority.Default)]
    internal sealed class GherkinFeatureTitleClassificationFormat : ClassificationFormatDefinition
    {
        public GherkinFeatureTitleClassificationFormat()
        {
            DisplayName = "Gherkin Feature";
            IsBold = true;
            FontRenderingSize = 14;
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "gherkin.featureTitle")]
    [Name("gherkin.featureTitle")]
    [UserVisible(true)]
    [Order(Before = Priority.Default)]
    internal sealed class GherkinFeatureTitleFormat : ClassificationFormatDefinition
    {
        public GherkinFeatureTitleFormat()
        {
            DisplayName = "Gherkin Feature title";
            FontRenderingSize = 13;
            ForegroundColor = Colors.DarkBlue;
            IsBold = true;
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "gherkin.featureDescription")]
    [Name("gherkin.featureDescription")]
    [UserVisible(true)]
    [Order(Before = Priority.Default)]
    internal sealed class GherkinDescriptionClassificationFormat : ClassificationFormatDefinition
    {
        public GherkinDescriptionClassificationFormat()
        {
            DisplayName = "Gherkin Feature Description";
            IsItalic = true;
            ForegroundColor = Colors.Chocolate;
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "gherkin.scenario")]
    [Name("gherkin.scenario")]
    [UserVisible(true)]
    [Order(Before = Priority.Default)]
    internal sealed class GherkinScenarioTitleClassificationFormat : ClassificationFormatDefinition
    {
        public GherkinScenarioTitleClassificationFormat()
        {
            DisplayName = "Gherkin Scenario";
            FontRenderingSize = 13;
            IsBold = true;
            ForegroundColor = Colors.Black;
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "gherkin.scenarioTitle")]
    [Name("gherkin.scenarioTitle")]
    [UserVisible(true)]
    [Order(Before = Priority.Default)]
    internal sealed class GherkinScenarioTitleFormat : ClassificationFormatDefinition
    {
        public GherkinScenarioTitleFormat()
        {
            DisplayName = "Gherkin Scenario title";
            ForegroundColor = Colors.DarkBlue;
            IsBold = true;
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "gherkin.background")]
    [Name("gherkin.background")]
    [UserVisible(true)]
    [Order(Before = Priority.Default)]
    internal sealed class GherkinBackgroundTitleClassificationFormat : ClassificationFormatDefinition
    {
        public GherkinBackgroundTitleClassificationFormat()
        {
            DisplayName = "Gherkin Background";
            FontRenderingSize = 13;
            IsBold = true;
            ForegroundColor = Colors.Black;
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "gherkin.backgroundTitle")]
    [Name("gherkin.backgroundTitle")]
    [UserVisible(true)]
    [Order(Before = Priority.Default)]
    internal sealed class GherkinBackgroundTitleFormat : ClassificationFormatDefinition
    {
        public GherkinBackgroundTitleFormat()
        {
            DisplayName = "Gherkin Background title";
            ForegroundColor = Colors.DarkBlue;
            IsBold = true;
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "gherkin.comment")]
    [Name("gherkin.comment")]
    [UserVisible(true)]
    [Order(Before = Priority.Default)]
    internal sealed class GherkinCommentClassificationFormat : ClassificationFormatDefinition
    {
        public GherkinCommentClassificationFormat()
        {
            DisplayName = "Gherkin Comment";
            ForegroundColor = Colors.Green;
            IsItalic = true;
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "gherkin.tag")]
    [Name("gherkin.tag")]
    [UserVisible(true)]
    [Order(Before = Priority.Default)]
    internal sealed class GherkinTagClassificationFormat : ClassificationFormatDefinition
    {
        public GherkinTagClassificationFormat()
        {
            DisplayName = "Gherkin Tag";
            ForegroundColor = Colors.Black;
            BackgroundColor = Colors.LightGoldenrodYellow;
            IsItalic = true;
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "gherkin.docstring")]
    [Name("gherkin.docstring")]
    [UserVisible(true)]
    [Order(Before = Priority.Default)]
    internal sealed class GherkinDocstringClassificationFormat : ClassificationFormatDefinition
    {
        public GherkinDocstringClassificationFormat()
        {
            DisplayName = "Gherkin Docstring (Multi-line Text Argument)";
            ForegroundColor = Colors.DarkSlateBlue;
            BackgroundColor = Color.FromArgb(255, 232, 232, 236);
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "gherkin.examples")]
    [Name("gherkin.examples")]
    [UserVisible(true)]
    [Order(Before = Priority.Default)]
    internal sealed class GherkinExamplesClassificationFormat : ClassificationFormatDefinition
    {
        public GherkinExamplesClassificationFormat()
        {
            DisplayName = "Gherkin Examples";
            ForegroundColor = Colors.Blue;
            IsBold = true;
            IsItalic = true;
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "gherkin.step")]
    [Name("gherkin.step")]
    [UserVisible(true)]
    [Order(Before = Priority.Default)]
    internal sealed class GherkinStepClassificationFormat : ClassificationFormatDefinition
    {
        public GherkinStepClassificationFormat()
        {
            DisplayName = "Gherkin Step";
            ForegroundColor = Colors.DarkSlateBlue;
            IsBold = true;
            FontRenderingSize = 13;
            IsItalic = true;
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "gherkin.stepText")]
    [Name("gherkin.stepText")]
    [UserVisible(true)]
    [Order(Before = Priority.Default)]
    internal sealed class GherkinStepTextClassificationFormat : ClassificationFormatDefinition
    {
        public GherkinStepTextClassificationFormat()
        {
            DisplayName = "Gherkin step text";
            ForegroundColor = Colors.Blue;
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "gherkin.table")]
    [Name("gherkin.table")]
    [UserVisible(true)]
    [Order(Before = Priority.Default)]
    internal sealed class GherkinTableClassificationFormat : ClassificationFormatDefinition
    {
        public GherkinTableClassificationFormat()
        {
            DisplayName = "Gherkin Table";
            ForegroundColor = Colors.Black;
            BackgroundColor = Colors.AliceBlue;
            IsBold = true;
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "gherkin.tableheader")]
    [Name("gherkin.tableheader")]
    [UserVisible(true)]
    [Order(Before = Priority.Default)]
    internal sealed class GherkinTableHeaderClassificationFormat : ClassificationFormatDefinition
    {
        public GherkinTableHeaderClassificationFormat()
        {
            DisplayName = "Gherkin Table Header";
            IsBold = true;
            ForegroundColor = Colors.Black;
            BackgroundColor = Colors.Silver;
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "gherkin.tablecell")]
    [Name("gherkin.tablecell")]
    [UserVisible(true)]
    [Order(Before = Priority.Default)]
    internal sealed class GherkinTableCellClassificationFormat : ClassificationFormatDefinition
    {
        public GherkinTableCellClassificationFormat()
        {
            DisplayName = "Gherkin Table Cell";
            ForegroundColor = Colors.Black;
            BackgroundColor = Color.FromArgb(255, 232, 232, 236);
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "gherkin.tablecellalt")]
    [Name("gherkin.tablecellalt")]
    [UserVisible(true)]
    [Order(Before = Priority.Default)]
    internal sealed class GherkinTableCellAltClassificationFormat : ClassificationFormatDefinition
    {
        public GherkinTableCellAltClassificationFormat()
        {
            DisplayName = "Gherkin Table Cell (alt)";
            ForegroundColor = Colors.Black;
        }
    }
}