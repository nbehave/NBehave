using System;

namespace NBehave.Narrator.Framework.Specifications.EventListeners
{
    //TODO: Remove
    [ActionSteps]
    public class XmlOutputEventListenerSteps
    {
        [Given(@"a string $str")]
        public void AString(string str)
        { }

        [When(@"string is ecco'ed")]
        public void EcchoString()
        { }

        [Then(@"you should see $strOut")]
        public void StringOut(string strOut)
        { }

        [Given(@"something$")]
        [Given(@"something x$")]
        [Given(@"something two$")]
        public void AGiven()
        {
        }

        [When(@"some event occurs$")]
        [When(@"some event y occurs$")]
        [When(@"some event #2 occurs$")]
        public void AWhen()
        {
        }

        [Then(@"there is some outcome$")]
        [Then(@"there is some outcome #2$")]
        public void AThen()
        {
        }

        [Then(@"there is some failing outcome$")]
        public void AThenFailing()
        {
            throw new Exception("outcome failed");
        }
    }
}