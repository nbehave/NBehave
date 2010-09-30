using System;
using NUnit.Framework;

namespace NBehave.Narrator.Framework.Specifications
{
    [TestFixture]
    public class ScenarioResultsFixture
    {
        private ScenarioResult _results;

        [SetUp]
        public void Establish_context()
        {
            _results = new ScenarioResult(new Feature("Feature Title"), "Scenario Title");
        }

        [Test]
        public void Should_set_message_with_failure_when_failed()
        {
            _results.Fail(new Exception("Error"));

            Assert.That(_results.Result, Is.TypeOf(typeof(Failed)));
            Assert.That(_results.Message, Is.EqualTo("System.Exception : Error"));
        }

        [Test]
        public void Should_set_message_with_inner_exception_information_when_failed()
        {
            var inner = new ApplicationException("Inner");
            var outer = new Exception("Outer", inner);
            _results.Fail(outer);

            Assert.That(_results.Result, Is.TypeOf(typeof(Failed)));
            Assert.That(_results.Message,
                        Is.EqualTo("System.Exception : Outer\r\n  ----> System.ApplicationException : Inner"));
        }

        [Test]
        public void Should_set_message_with_pending_reason_when_pending()
        {
            _results.Pend("reason", "Given step");

            Assert.That(_results.Result, Is.TypeOf(typeof(Pending)));
            Assert.That(_results.Message, Is.EqualTo("reason"));
        }

        [Test]
        public void Should_set_stack_trace_of_exception_when_failed()
        {
            Exception ex;

            try
            {
                throw new Exception("Test");
            }
            catch (Exception e)
            {
                ex = e;
            }

            _results.Fail(ex);

            Assert.That(_results.StackTrace, Is.EqualTo(ex.StackTrace));
        }

        [Test]
        public void Should_set_stack_trace_with_inner_exception_details_when_failed()
        {
            Exception ex;

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

            _results.Fail(ex);

            var expected = ex.StackTrace + "\r\n--ApplicationException\r\n" + ex.InnerException.StackTrace;

            Assert.That(_results.StackTrace, Is.EqualTo(expected));
        }

        [Test]
        public void Failed_Step_message_should_not_overwrite_existing_message()
        {
            _results.Pend("not done", "Given step");
            var ex = new Exception("bad thing happened!");
            _results.Fail(ex);

            var expected = "not done" + Environment.NewLine + "System.Exception : bad thing happened!";
            Assert.That(_results.Message, Is.EqualTo(expected));
        }
    }
}