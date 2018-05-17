using NUnit.Framework;

namespace NBehave.Fluent.Framework.NUnit
{
    [TestFixture]
    public abstract class ScenarioDrivenSpecBase : Framework.ScenarioDrivenSpecBase
    {
        [OneTimeSetUp]
        public override void MainSetup()
        {
            base.MainSetup();
        }

        [OneTimeTearDown]
        public override void MainTeardown()
        {
            base.MainTeardown();
        }
    }
}
