using System;

namespace NBehave.VS2010.Plugin.Tagging
{
    public enum GherkinTokenType
    {
        SyntaxError,
        Feature,
        FeatureTitle,
        FeatureDescription,
        Scenario,
        ScenarioTitle,
        Background,
        BackgroundTitle,
        Comment,
        Tag,
        DocString,
        Examples,
        Step,
        StepText,
        Table,
        TableHeader,
        TableCell,
        Eof = Int32.MaxValue,
    }
}