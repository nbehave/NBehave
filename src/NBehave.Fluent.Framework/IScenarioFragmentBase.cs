using System;

namespace NBehave.Fluent.Framework
{
    public interface IScenarioFragmentBase<T> where T : IScenarioFragmentBase<T>
    {
        T And(string step);
        T And(string step, Action implementation);
    }
}
