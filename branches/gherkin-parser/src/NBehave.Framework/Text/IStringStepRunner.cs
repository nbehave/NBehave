namespace NBehave.Narrator.Framework
{
    public interface IStringStepRunner
    {
        ActionStepResult Run(ActionStepText actionStep);
        ActionStepResult Run(ActionStepText actionStep, Row row);
        void OnCloseScenario();
        void BeforeScenario();
        void AfterScenario();
    }
}