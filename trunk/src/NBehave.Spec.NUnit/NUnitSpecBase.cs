using NUnit.Framework;

namespace NBehave.Spec.NUnit
{
	public abstract class NUnitSpecBase : SpecBase
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
