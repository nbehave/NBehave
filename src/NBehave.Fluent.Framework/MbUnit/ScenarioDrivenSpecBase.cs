using MbUnit.Framework;

namespace NBehave.Fluent.Framework.MbUnit
{
    [TestFixture]
    public abstract class ScenarioDrivenSpecBase : Framework.ScenarioDrivenSpecBase
    {
        [FixtureSetUp]
        public override void MainSetup()
        {
            base.MainSetup();
        }

        [FixtureTearDown]
        public override void MainTeardown()
        {
            base.MainTeardown();
        }
    }
}
