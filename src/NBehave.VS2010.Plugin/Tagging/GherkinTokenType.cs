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
        Language,
        Tag,
        DocString,
        Examples,
        Step,
        StepText,
        Table,
        TableHeader,
        TableCell,
        TableCellAlt,
        Eof,
    }
}