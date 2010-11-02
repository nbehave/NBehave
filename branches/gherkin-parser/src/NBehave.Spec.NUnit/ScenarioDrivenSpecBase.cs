using NUnit.Framework;

namespace NBehave.Spec.NUnit
{
    [TestFixture]
    public abstract class ScenarioDrivenSpecBase : Spec.ScenarioDrivenSpecBase
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
