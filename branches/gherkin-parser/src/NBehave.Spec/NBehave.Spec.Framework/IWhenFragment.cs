using System;

namespace NBehave.Spec
{
    public interface IWhenFragment : IScenarioFragmentBase<IWhenFragment>
    {
        IThenFragment Then(string step);
        IThenFragment Then(string step, Action implementation);
    }
}
