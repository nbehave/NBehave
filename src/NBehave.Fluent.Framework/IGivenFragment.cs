using System;

namespace NBehave.Fluent
{
    public interface IGivenFragment : IScenarioFragmentBase<IGivenFragment>
    {
        IWhenFragment When(string step);
        IWhenFragment When(string step, Action implementation);
    }
}
