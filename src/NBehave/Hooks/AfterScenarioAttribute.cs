using System;

namespace NBehave.Hooks
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Field)]
    public class AfterScenarioAttribute : HookAttribute { }
}