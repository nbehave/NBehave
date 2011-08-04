using NUnit.Framework;

namespace NBehave.Fluent.Framework.NUnit
{
    [TestFixture]
    public abstract class ScenarioDrivenSpecBase : Framework.ScenarioDrivenSpecBase
    {
        [TestFixtureSetUp]
        public override void MainSetup()
        {
            base.MainSetup();
        }

        [TestFixtureTearDown]
        public override void MainTeardown()
        {
            base.MainTeardown();
        }        
    }
}
