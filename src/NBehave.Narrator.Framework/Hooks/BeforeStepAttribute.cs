using System;

namespace NBehave.Narrator.Framework.Hooks
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class HookAttribute : Attribute
    { }

    [AttributeUsage(AttributeTargets.Class)]
    public class HooksAttribute : Attribute { }
    [AttributeUsage(AttributeTargets.Method)]
    public class BeforeRunAttribute : HookAttribute { }
    [AttributeUsage(AttributeTargets.Method)]
    public class AfterRunAttribute : HookAttribute { }
    [AttributeUsage(AttributeTargets.Method)]
    public class BeforeFeatureAttribute : HookAttribute { }
    [AttributeUsage(AttributeTargets.Method)]
    public class AfterFeatureAttribute : HookAttribute { }
    [AttributeUsage(AttributeTargets.Method)]
    public class BeforeScenarioAttribute : HookAttribute { }
    [AttributeUsage(AttributeTargets.Method)]
    public class AfterScenarioAttribute : HookAttribute { }
    [AttributeUsage(AttributeTargets.Method)]
    public class BeforeStepAttribute : HookAttribute { }
    [AttributeUsage(AttributeTargets.Method)]
    public class AfterStepAttribute : HookAttribute { }
}