﻿using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NBehave.Spec.MSTest
{
	[TestClass]
	public abstract class SpecBase<TContext> : Spec.SpecBase<TContext>
	{
		[ClassInitialize]
		public override void MainSetup()
		{
			base.MainSetup();
		}

		[ClassCleanup]
		public override void MainTeardown()
		{
			base.MainTeardown();
		}
	}
}
