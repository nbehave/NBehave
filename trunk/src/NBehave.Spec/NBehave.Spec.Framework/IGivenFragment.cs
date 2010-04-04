using System;

namespace NBehave.Spec
{
    public interface IGivenFragment : IScenarioFragmentBase<IGivenFragment>
    {
        IWhenFragment When(string step);
        IWhenFragment When(string step, Action implementation);
    }
}
