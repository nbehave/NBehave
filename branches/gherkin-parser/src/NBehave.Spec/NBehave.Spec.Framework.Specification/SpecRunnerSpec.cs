using System;

namespace NBehave.Spec.Framework.Specification
{
    [Context]
    public class SpecRunnerSpec
    {
        private int _failures = 0;
        private int _specClassesFound = 0;
        private int _successes = 0;
        private SpecRunner runner;

        [SetUp]
        public void SetUp()
        {
            _specClassesFound = 0;
            _successes = 0;
            _failures = 0;

            runner = new SpecRunner();

            runner.SpecClassFound += new SpecClassFoundHandler(runner_SpecClassFound);
            runner.SpecificationPassed += new SpecficationPassedHandler(runner_SpecificationPassed);
            runner.SpecificationFailed += new SpecificationFailedHandler(runner_SpecificationFailed);

            runner.LoadAssembly("DummySpecAssembly.dll");
        }

        private void runner_SpecificationFailed(Failure failure)
        {
            _failures++;
        }

        private void runner_SpecificationPassed()
        {
            _successes++;
        }

        private void runner_SpecClassFound(Type type)
        {
            _specClassesFound++;
        }

        [Specification]
        public void ShouldFindAllSpecClassesInAssembly()
        {
            Specify.That(_specClassesFound).ShouldEqual(3);
        }

        [Specification]
        public void ShouldRunAllSpecMethods()
        {
            Specify.RunnersStopListening();
            runner.Run();
            Specify.RunnersStartListening();

            Specify.That(_successes).ShouldEqual(5);
            Specify.That(_failures).ShouldEqual(3);
        }
    }
}