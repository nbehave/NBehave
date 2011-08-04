using System;

namespace NBehave.Fluent.Framework
{
    public interface IWhenFragment : IScenarioFragmentBase<IWhenFragment>
    {
        IThenFragment Then(string step);
        IThenFragment Then(string step, Action implementation);
    }
}
