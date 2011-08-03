using MbUnit.Framework;

namespace NBehave.Spec.MbUnit
{
	[TestFixture]
	public abstract class SpecBase<TContext> : Spec.SpecBase<TContext>
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
