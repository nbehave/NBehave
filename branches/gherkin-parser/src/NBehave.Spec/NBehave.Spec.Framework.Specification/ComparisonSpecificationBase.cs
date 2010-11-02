using System;

namespace NBehave.Spec.Framework.Specification
{
    public class ComparisonSpecificationBase
    {
        private int _failures = 0;
        private int _successes = 0;
        protected SpecRunner Runner;

        [SetUp]
        public void SetUp()
        {
            Runner = new SpecRunner(false);
            _successes = 0;
            _failures = 0;

            Runner.SpecificationPassed += new SpecficationPassedHandler(Runner_SpecificationPassed);
            Runner.SpecificationFailed += new SpecificationFailedHandler(Runner_SpecificationFailed);
        }

        [TearDown]
        public void TearDown()
        {
            Runner.SpecificationPassed -= new SpecficationPassedHandler(Runner_SpecificationPassed);
            Runner.SpecificationFailed -= new SpecificationFailedHandler(Runner_SpecificationFailed);

            Runner = null;
        }

        private void Runner_SpecificationFailed(Failure failure)
        {
            _failures++;
        }

        private void Runner_SpecificationPassed()
        {
            _successes++;
        }

        protected void VerifySuccessAndFailureCount(int expectedSuccesses, int expectedFailures)
        {
            Specify.That(_successes).ShouldEqual(expectedSuccesses);
            Specify.That(_failures).ShouldEqual(expectedFailures);
        }

        protected void VerifySingleSuccess()
        {
            VerifySuccessAndFailureCount(1, 0);
        }

        protected void VerifySingleFailure()
        {
            VerifySuccessAndFailureCount(0, 1);
        }
    }
}