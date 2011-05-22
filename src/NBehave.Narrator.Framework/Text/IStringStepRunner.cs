namespace NBehave.Narrator.Framework
{
    public interface IStringStepRunner
    {
        ActionStepResult Run(ActionStepText actionStep);
        ActionStepResult Run(ActionStepText actionStep, Row row);
        void BeforeScenario();
        void AfterScenario();
    }
}