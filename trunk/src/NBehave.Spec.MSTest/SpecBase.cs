﻿using System;
﻿using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NBehave.Spec.MSTest
{
	public abstract class SpecBase : Spec.SpecBase
	{
		[TestInitialize]
		public override void SpecSetup()
		{
			base.SpecSetup();
		}

		[TestCleanup]
		public override void SpecTeardown()
		{
			base.SpecTeardown();
		}
	}
}
