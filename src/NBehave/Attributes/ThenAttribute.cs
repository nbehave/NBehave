namespace NBehave
{
    public class ThenAttribute : ActionStepAttribute
    {
        public ThenAttribute()
        {
            Type = "Then";
        }

        public ThenAttribute(string regexOrTokenString) : base(regexOrTokenString)
        {
            Type = "Then";            
        }
    }
}