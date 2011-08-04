using System;

namespace NBehave.Fluent.Framework
{
    public interface IScenarioBuilderStart
    {
        IGivenFragment Given(string step);
        IGivenFragment Given(string step, Action implementation);
    }
}
