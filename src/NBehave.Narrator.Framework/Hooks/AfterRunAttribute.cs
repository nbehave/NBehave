using System;

namespace NBehave.Narrator.Framework.Hooks
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Field)]
    public class AfterRunAttribute : HookAttribute { }
}