namespace NBehave.Narrator.Framework
{
    public class WhenAttribute : ActionStepAttribute
    {
        public WhenAttribute()
        {
            Type = "When";            
        }

        public WhenAttribute(string regexOrTokenString)
            : base(regexOrTokenString)
        {
            Type = "When";            
        }
    }
}