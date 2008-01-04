using System;
using NBehave.Narrator.Framework;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace NBehave.Narrator.Framework.Specifications
{
    [TestFixture]
    public class ScenarioResultsFixture
    {
        [Test]
        public void Should_set_message_with_failure_when_failed()
        {
            ScenarioResults results = new ScenarioResults("Story Title", "Scenario Title", ScenarioResult.Passed);

            results.Fail(new Exception("Error"));

            Assert.That(results.ScenarioResult, Is.EqualTo(ScenarioResult.Failed));
            Assert.That(results.Message, Is.EqualTo("System.Exception : Error"));
        }

        [Test]
        public void Should_set_message_with_inner_exception_information_when_failed()
        {
            ScenarioResults results = new ScenarioResults("Story Title", "Scenario Title", ScenarioResult.Passed);

            ApplicationException inner = new ApplicationException("Inner");
            Exception outer = new Exception("Outer", inner);
            results.Fail(outer);

            Assert.That(results.ScenarioResult, Is.EqualTo(ScenarioResult.Failed));
            Assert.That(results.Message,
                        Is.EqualTo("System.Exception : Outer\r\n  ----> System.ApplicationException : Inner"));
        }

        [Test]
        public void Should_set_message_with_pending_reason_when_pending()
        {
            ScenarioResults results = new ScenarioResults("Story Title", "Scenario Title", ScenarioResult.Passed);

            results.Pend("reason");

            Assert.That(results.ScenarioResult, Is.EqualTo(ScenarioResult.Pending));
            Assert.That(results.Message, Is.EqualTo("reason"));
        }

        [Test]
        public void Should_set_stack_trace_of_exception_when_failed()
        {
            ScenarioResults results = new ScenarioResults("Story Title", "Scenario Title", ScenarioResult.Passed);

            Exception ex;

            try
            {
                throw new Exception("Test");
            }
            catch (Exception e)
            {
                ex = e;
            }

            results.Fail(ex);

            Assert.That(results.StackTrace, Is.EqualTo(ex.StackTrace));
        }

        [Test]
        public void Should_set_stack_trace_with_inner_exception_details_when_failed()
        {
            ScenarioResults results = new ScenarioResults("Story Title", "Scenario Title", ScenarioResult.Passed);

            Exception ex = null;

            try
            {
                try
                {
                    throw new ApplicationException("Inner");
                }
                catch (Exception e)
                {
                    throw new Exception("Outer", e);
                }
            }
            catch (Exception e)
            {
                ex = e;
            }

            results.Fail(ex);

            string expected = ex.StackTrace + "\r\n--ApplicationException\r\n" + ex.InnerException.StackTrace;

            Assert.That(results.StackTrace, Is.EqualTo(expected));
        }
    }
}