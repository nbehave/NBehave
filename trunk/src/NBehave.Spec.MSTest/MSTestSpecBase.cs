﻿using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NBehave.Spec.MSTest
{
	public abstract class MSTestSpecBase : SpecBase
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
