﻿using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NBehave.Spec.MSTest10
{
	[TestClass]
	public abstract class SpecBase : Spec.SpecBase
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
