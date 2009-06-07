using System;
using System.Collections.Generic;
using System.Diagnostics;
using NAnt.Core;
using NUnit.Framework;

namespace NBehave.NAnt.Tests
{
    [TestFixture]
    public class NBehaveTaskTest
    {
        [Test]
        public void Execute_tests_in_test_build_script()
        {
            var project = new Project("NBehaveTestScript.build", Level.Debug, 1);
            Assert.IsTrue(project.Run(), "Something went wrong executing the test script.  Check log.");
        }

        [Test]
        public void Should_execute_scenariotext_scenario()
        {
            var project = new Project("GreetingSystem.build", Level.Debug, 1);
            Assert.IsTrue(project.Run(), "Something went wrong executing the test script.  Check log.");
        }
}
}
