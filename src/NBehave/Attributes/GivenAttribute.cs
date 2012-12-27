namespace NBehave
{
    public class GivenAttribute : ActionStepAttribute
    {
        public GivenAttribute()
        {
            Type = "Given";            
        }

        public GivenAttribute(string regexOrTokenString)
            : base(regexOrTokenString)
        {
            Type = "Given";            
        }
    }
}