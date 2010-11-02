namespace NBehave.Spec
{
    public interface IScenarioBuilderStartWithHelperObject : IScenarioBuilderStart
    {
        IScenarioBuilderStart WithHelperObject(object stepHelper);
        IScenarioBuilderStart WithHelperObject<T>() where T : new();
    }
}
