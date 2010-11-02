using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace NBehave.VS2010.Plugin.Editor.Domain
{
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "gherkin.keyword")]
    [Name("gherkin.keyword")]
    [UserVisible(true)] 
    [Order(Before = Priority.Default)] 
    internal sealed class GherkinKeywordClassificationFormat : ClassificationFormatDefinition
    {
        public GherkinKeywordClassificationFormat()
        {
            this.DisplayName = "Gherkin Keyword"; 
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
            this.DisplayName = "Gherkin Comment";
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
            this.DisplayName = "Gherkin Tag"; 
            this.IsItalic = true;
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "gherkin.placeholder")]
    [Name("gherkin.placeholder")]
    [UserVisible(true)] 
    [Order(Before = Priority.Default)] 
    internal sealed class GherkinPlaceholderClassificationFormat : ClassificationFormatDefinition
    {
        public GherkinPlaceholderClassificationFormat()
        {
            this.DisplayName = "Gherkin Scenario Outline Placeholder"; 
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "gherkin.multilinetext")]
    [Name("gherkin.multilinetext")]
    [UserVisible(true)] 
    [Order(Before = Priority.Default)]
    internal sealed class GherkinMultilineTextClassificationFormat : ClassificationFormatDefinition
    {
        public GherkinMultilineTextClassificationFormat()
        {
            this.DisplayName = "Gherkin Multi-line Text Argument";
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
            this.DisplayName = "Gherkin Table Cell"; 
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
            this.DisplayName = "Gherkin Table Header";
            this.IsItalic = true;
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "gherkin.description")]
    [Name("gherkin.description")]
    [UserVisible(true)] 
    [Order(Before = Priority.Default)] 
    internal sealed class GherkinDescriptionClassificationFormat : ClassificationFormatDefinition
    {
        public GherkinDescriptionClassificationFormat()
        {
            this.DisplayName = "Gherkin Feature/Scenario Description";
            this.IsItalic = true;
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "gherkin.scenariotitle")]
    [Name("gherkin.scenariotitle")]
    [UserVisible(true)] 
    [Order(Before = Priority.Default)] 
    internal sealed class GherkinScenarioTitleClassificationFormat : ClassificationFormatDefinition
    {
        public GherkinScenarioTitleClassificationFormat()
        {
            this.DisplayName = "Gherkin Scenario Title"; 
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "gherkin.featuretitle")]
    [Name("gherkin.featuretitle")]
    [UserVisible(true)] 
    [Order(Before = Priority.Default)] 
    internal sealed class GherkinFeatureTitleClassificationFormat : ClassificationFormatDefinition
    {
        public GherkinFeatureTitleClassificationFormat()
        {
            this.DisplayName = "Gherkin Feature Title"; 
        }
    }
}
