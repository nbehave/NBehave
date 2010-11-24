using System;

namespace NBehave.Spec
{
    public interface IScenarioBuilderStart
    {
        IGivenFragment Given(string step);
        IGivenFragment Given(string step, Action implementation);
    }
}
