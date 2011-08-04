using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NBehave.Fluent.Framework.MSTest
{
    [TestClass]
    public abstract class ScenarioDrivenSpecBase : Framework.ScenarioDrivenSpecBase
    {
        [TestInitialize]
        public override void MainSetup()
        {
            base.MainSetup();
        }

        [TestCleanup]
        public override void MainTeardown()
        {
            base.MainTeardown();
        }
    }
}
