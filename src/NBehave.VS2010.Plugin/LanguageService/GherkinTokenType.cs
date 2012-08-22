using System;

namespace NBehave.VS2010.Plugin.LanguageService
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

        //NewElementHere,
        Eof = Int32.MaxValue,
    }
}