namespace NBehave.Fluent.Framework
{
    public interface IScenarioBuilderStartWithHelperObject : IScenarioBuilderStart
    {
        IScenarioBuilderStart WithHelperObject(object stepHelper);
        IScenarioBuilderStart WithHelperObject<T>() where T : new();
    }
}
