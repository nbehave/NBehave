using MbUnit.Framework;

namespace NBehave.Spec.MbUnit
{
	public abstract class MbUnitSpecBase : SpecBase
	{
		[SetUp]
		public override void MainSetup()
		{
			base.MainSetup();
		}

		[TearDown]
		public override void MainTeardown()
		{
			base.MainTeardown();
		}
	}
}
