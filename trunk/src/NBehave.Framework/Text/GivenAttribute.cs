using System.Text.RegularExpressions;

namespace NBehave.Narrator.Framework
{
    public class GivenAttribute : ActionStepAttribute
    {
        public GivenAttribute()
        {
            Type = "Given";            
        }

        private GivenAttribute(Regex actionMatch)
            : base(actionMatch)
        {
            Type = "Given";
        }
      
        public GivenAttribute(string regex)
            : this(new Regex(regex))
        { }
    }
}