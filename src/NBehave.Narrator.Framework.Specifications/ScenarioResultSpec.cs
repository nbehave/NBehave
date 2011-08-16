using System;
using NUnit.Framework;

namespace NBehave.Narrator.Framework.Specifications
{
    [TestFixture]
    public class ScenarioResultSpec
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
            Assert.That(_results.Message, Is.EqualTo("System.Exception : Outer\r\n  ----> System.ApplicationException : Inner"));
        }

        [Test]
        public void ShouldSetMessageWithPendingReasonWhenPending()
        {
            _results.Pend("reason");

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
            _results.Pend("not done");
            var ex = new Exception("bad thing happened!");
            _results.Fail(ex);

            var expected = "not done" + Environment.NewLine + "System.Exception : bad thing happened!";
            Assert.That(_results.Message, Is.EqualTo(expected));
        }

        [Test]
        public void HasFailedSteps_should_report_failed_if_at_least_one_step_failed()
        {
            var passed = new StepResult("Foo".AsActionStepText(""), new Passed());
            _results.AddActionStepResult(passed);
            var failed = new StepResult("Foo".AsActionStepText(""), new Failed(new Exception()));
            _results.AddActionStepResult(failed);
            Assert.IsTrue(_results.HasFailedSteps());
        }

        [Test]
        public void HasFailedSteps_should_return_false_if_no_failed_steps()
        {
            var passed = new StepResult("Foo".AsActionStepText(""), new Passed());
            _results.AddActionStepResult(passed);
            var pending = new StepResult("Bar".AsActionStepText(""), new Pending("yadda"));
            _results.AddActionStepResult(pending);
            Assert.IsFalse(_results.HasFailedSteps());
        }
    }
}