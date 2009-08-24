using System.Reflection;

namespace NBehave.Narrator.Framework
{
    public class ActionMethodInfo : ActionMatch
    {
        public string ActionType { get; set; } //Given, When Then etc..
        public MethodInfo MethodInfo { get; set; }
    }
}
