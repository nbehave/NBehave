using System;

namespace NBehave.Narrator.Framework.Hooks
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class HookAttribute : Attribute
    {
        public string[] RunIfHasTags { get; set; }
        public string[] DontRunIfHasTags { get; set; }

        protected HookAttribute()
            : this(new string[0], new string[0])
        { }

        private HookAttribute(string[] runIfHasTags, string[] dontRunIfHasTags)
        {
            RunIfHasTags = runIfHasTags;
            DontRunIfHasTags = dontRunIfHasTags;
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class HooksAttribute : Attribute { }
    
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Field)]
    public class BeforeRunAttribute : HookAttribute { }
    
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Field)]
    public class AfterRunAttribute : HookAttribute { }
    
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Field)]
    public class BeforeFeatureAttribute : HookAttribute { }
    
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Field)]
    public class AfterFeatureAttribute : HookAttribute { }
    
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Field)]
    public class BeforeScenarioAttribute : HookAttribute { }
    
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Field)]
    public class AfterScenarioAttribute : HookAttribute { }
    
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Field)]
    public class BeforeStepAttribute : HookAttribute { }
    
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Field)]
    public class AfterStepAttribute : HookAttribute { }
}