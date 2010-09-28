using System;

namespace NBehave.Narrator.Framework.Specifications.EventListeners
{
    //TODO: Remove
    [ActionSteps]
    public class XmlOutputEventListenerTestData
    {
        [Given(@"a string $str")]
        public void a_string(string str)
        { }

        [When(@"string is ecco'ed")]
        public void Eccho_string()
        { }

        [Then(@"you should see $strOut")]
        public void StringOut(string strOut)
        { }

        [Given(@"something$")]
        [Given(@"something x$")]
        [Given(@"something two$")]
        public void A_given()
        {
        }

        [When(@"some event occurs$")]
        [When(@"some event y occurs$")]
        [When(@"some event #2 occurs$")]
        public void a_when()
        {
        }

        [Then(@"there is some outcome$")]
        [Then(@"there is some outcome #2$")]
        public void a_then()
        {
        }

        [Then(@"there is some failing outcome$")]
        public void a_then_failing()
        {
            throw new Exception("outcome failed");
        }
    }
}