using System;
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
            Project project = new Project("NBehaveTestScript.build", Level.Debug, 1);
            project.MessageLogged += (sender, e) => Console.Out.WriteLine(e.Message);

            Assert.IsTrue(project.Run(), "Something went wrong executing the test script.  Check log.");
        }
    }
}
