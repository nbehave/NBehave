using System;

namespace NBehave.Fluent.Framework
{
    public interface IGivenFragment : IScenarioFragmentBase<IGivenFragment>
    {
        IWhenFragment When(string step);
        IWhenFragment When(string step, Action implementation);
    }
}
