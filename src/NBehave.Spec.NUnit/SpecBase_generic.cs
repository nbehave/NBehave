using NUnit.Framework;

namespace NBehave.Spec.NUnit
{
	[TestFixture]
    public abstract class SpecBase<TContext> : Spec.SpecBase<TContext>
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
