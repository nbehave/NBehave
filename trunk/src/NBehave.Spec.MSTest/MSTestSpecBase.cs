using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NBehave.Spec.MSTest
{
	[TestClass]
	public class MSTestSpecBase : SpecBase
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
