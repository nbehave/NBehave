using System;
using NBehave.Narrator.Framework;

namespace TestAssembly
{
    [Theme]
    public class InvalidActionSpecs
    {
        [Story]
        public void Invalid_action()
        {
            var story = new Story("Developer failures");
            var d = DateTime.MinValue;
            story
                .WithScenario("Developer forgets to put ()=> .. in action")
                .Given("An invalid action", d = new DateTime(2009, 1, 1));

        }
    }
}