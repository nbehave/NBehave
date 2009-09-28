using System;
using System.Text.RegularExpressions;

namespace NBehave.Narrator.Framework
{
    public class GivenAttribute : ActionStepAttribute
    {
        protected GivenAttribute(Regex actionMatch)
            : base(actionMatch)
        {
            Type = "Given";
        }
        public GivenAttribute(string regex)
            : this(new Regex(regex))
        { }
    }

    public class WhenAttribute : ActionStepAttribute
    {
        protected WhenAttribute(Regex actionMatch)
            : base(actionMatch)
        {
            Type = "When";
        }

        public WhenAttribute(string regex)
            : this(new Regex(regex))
        { }
    }

    public class ThenAttribute : ActionStepAttribute
    {
        protected ThenAttribute(Regex actionMatch)
            : base(actionMatch)
        {
            Type = "Then";
        }

        public ThenAttribute(string regex)
            : this(new Regex(regex))
        { }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class ActionStepAttribute : Attribute
    {
        public Regex ActionMatch { get; protected set; }
        public string Type { get; protected set; }

        public ActionStepAttribute()
        { }

        public ActionStepAttribute(string tokenString)
            : this(tokenString.RemoveFirstWord().AsRegex())
        {
            Type = tokenString.GetFirstWord();
        }

        protected ActionStepAttribute(Regex actionMatch)
        {
            ActionMatch = actionMatch;
            Type = actionMatch.ToString().GetFirstWord();
        }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class NotificationMethodAttribute : Attribute { }

    public class BeforeStepAttribute : NotificationMethodAttribute
    {
    }
    
    public class AfterStepAttribute : NotificationMethodAttribute
    {
    }
    
    public class BeforeScenarioAttribute : NotificationMethodAttribute
    {
    }

    public class AfterScenarioAttribute : NotificationMethodAttribute
    {
    }
}