using NUnit.Framework;

namespace NBehave.Spec.NUnit
{
	[TestFixture]
	public abstract class SpecBase<TContext> : Fluent.SpecBase<TContext>
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
