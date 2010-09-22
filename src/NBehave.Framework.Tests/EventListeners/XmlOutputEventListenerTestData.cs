using System;

namespace NBehave.Narrator.Framework.Specifications.EventListeners
{
    [ActionSteps]
    public class XmlOutputEventListenerTestData
    {
        public static string Feature = "Feature: S1" + Environment.NewLine +
                                       "  As a X1" + Environment.NewLine +
                                       "  I want Y1" + Environment.NewLine +
                                       "  So that Z1" + Environment.NewLine +
                                       "Scenario: SC1" + Environment.NewLine +
                                       "  Given something" + Environment.NewLine +
                                       "  When some event occurs" + Environment.NewLine +
                                       "  Then there is some outcome" + Environment.NewLine +
                                       "Scenario: SC2" + Environment.NewLine +
                                       "  Given something two" + Environment.NewLine +
                                       "  When some event #2 occurs" + Environment.NewLine +
                                       "  Then there is some outcome #2" + Environment.NewLine +
                                       "Scenario: Pending scenario" + Environment.NewLine +
                                       "  Given something pending" + Environment.NewLine +
                                       "  And something pending" + Environment.NewLine +
                                       "  And something more pending" + Environment.NewLine +
                                       "  When some pending event occurs" + Environment.NewLine +
                                       "  And some more pending event occurs" + Environment.NewLine +
                                       "  Then this text should still show up in xml output" +
                                       "  " + Environment.NewLine +
                                       "Feature: S2" + Environment.NewLine +
                                       "  As a X2" + Environment.NewLine +
                                       "  I want Y2" + Environment.NewLine +
                                       "  So that Z2" + Environment.NewLine +
                                       "Scenario: SC1" + Environment.NewLine +
                                       "  Given something" + Environment.NewLine +
                                       "  When some event occurs" + Environment.NewLine +
                                       "  Then there is some outcome" + Environment.NewLine +
                                       "  " + Environment.NewLine +
                                       "Feature: S3" + Environment.NewLine +
                                       "  As a X3" + Environment.NewLine +
                                       "  I want Y3" + Environment.NewLine +
                                       "  So that Z3" + Environment.NewLine +
                                       "Scenario: SC3" + Environment.NewLine +
                                       "  Given something" + Environment.NewLine +
                                       "  When some event occurs" + Environment.NewLine +
                                       "  Then there is some outcome" + Environment.NewLine +
                                       "Scenario: FailingScenario" + Environment.NewLine +
                                       "  Given something x" + Environment.NewLine +
                                       "  When some event y occurs" + Environment.NewLine +
                                       "  Then there is some failing outcome";

        public static string Feature_With_Examples = "Feature: Example" + Environment.NewLine +
                                                     "  As a X1" + Environment.NewLine +
                                                     "  I want Y1" + Environment.NewLine +
                                                     "  So that Z1" + Environment.NewLine +
                                                     "Scenario: SC1" + Environment.NewLine +
                                                     "  Given a string [str]" + Environment.NewLine +
                                                     "  When string is ecco'ed" + Environment.NewLine +
                                                     "  Then you should see [strOut]" + Environment.NewLine +
                                                     "Examples:" + Environment.NewLine +
                                                     "|str|strOut|" + Environment.NewLine +
                                                     "|xyz|xyz   |" + Environment.NewLine +
                                                     "|abc|abc   |" + Environment.NewLine;

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