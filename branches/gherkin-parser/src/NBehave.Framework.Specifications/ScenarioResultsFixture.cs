using System;
using NUnit.Framework;

namespace NBehave.Narrator.Framework.Specifications
{
    [TestFixture]
    public class ScenarioResultsFixture
    {
        private ScenarioResult _results;

        [SetUp]
        public void EstablishContext()
        {
            _results = new ScenarioResult(new Feature("Feature Title"), "Scenario Title");
        }

        [Test]
        public void ShouldSetMessageWithFailureWhenFailed()
        {
            _results.Fail(new Exception("Error"));

            Assert.That(_results.Result, Is.TypeOf(typeof(Failed)));
            Assert.That(_results.Message, Is.EqualTo("System.Exception : Error"));
        }

        [Test]
        public void ShouldSetMessageWithInnerExceptionInformationWhenFailed()
        {
            var inner = new ApplicationException("Inner");
            var outer = new Exception("Outer", inner);
            _results.Fail(outer);

            Assert.That(_results.Result, Is.TypeOf(typeof(Failed)));
            Assert.That(_results.Message,
                        Is.EqualTo("System.Exception : Outer\r\n  ----> System.ApplicationException : Inner"));
        }

        [Test]
        public void ShouldSetMessageWithPendingReasonWhenPending()
        {
            _results.Pend("reason", "Given step");

            Assert.That(_results.Result, Is.TypeOf(typeof(Pending)));
            Assert.That(_results.Message, Is.EqualTo("reason"));
        }

        [Test]
        public void ShouldSetStackTraceOfExceptionWhenFailed()
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
        public void ShouldSetStackTraceWithInnerExceptionDetailsWhenFailed()
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
        public void FailedStepMessageShouldNotOverwriteExistingMessage()
        {
            _results.Pend("not done", "Given step");
            var ex = new Exception("bad thing happened!");
            _results.Fail(ex);

            var expected = "not done" + Environment.NewLine + "System.Exception : bad thing happened!";
            Assert.That(_results.Message, Is.EqualTo(expected));
        }
    }
}