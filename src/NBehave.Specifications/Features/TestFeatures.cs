using System.IO;

namespace NBehave.Specifications.Features
{
    public static class TestFeatures
    {
        public static readonly string FeatureWithScenarioBackground = Path.Combine("System.Specs", "Backgrounds", "Background.feature");
        public static readonly string FeatureWithPendingStep = Path.Combine("Features", "FeatureWithPendingStep.feature");
        public static readonly string FeatureWithCommentOnLastRow = Path.Combine("Features", "FeatureWithCommentOnLastRow.feature");
        public static readonly string FeatureWithLowerCaseSteps = Path.Combine("Features", "FeatureWithLowerCaseSteps.feature");
        public static readonly string FeatureWithNewLineInGivenClause = Path.Combine("Features", "FeatureWithNewLineInGivenClause.feature");
        public static readonly string FeatureWithManyScenarios = Path.Combine("Features", "FeatureWithManyScenarios.feature");
        public static readonly string FeatureWithFailingStep = Path.Combine("Features", "FeatureWithFailingStep.feature");
        public static readonly string FeaturesAndScenarios = Path.Combine("Features", "FeaturesAndScenarios.feature");
        public static readonly string FeatureNamedStory = Path.Combine("Features", "FeatureNamedStory.feature");
        public static readonly string FeatureWithTags = Path.Combine("Features", "FeatureWithTags.feature");
        public static readonly string FeatureInSwedish = Path.Combine("Features", "FeatureInSwedish.feature");
        public static readonly string ScenariosWithoutFeature = Path.Combine("Features", "ScenariosWithoutFeature.feature");
        public static readonly string ScenarioWithExamples = Path.Combine("Features", "ScenarioWithExamples.feature");
        public static readonly string ScenarioWithTables = Path.Combine("Features", "ScenarioWithTables.feature");
        public static readonly string ScenarioWithNoActionSteps = Path.Combine("Features", "ScenarioWithNoActionSteps.feature");
    }
}
