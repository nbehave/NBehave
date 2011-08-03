using System;

namespace NBehave.Spec.Xunit
{
	public abstract class SpecBase : Fluent.SpecBase, IDisposable
	{
		protected SpecBase()
        {
            base.MainSetup();
        }

	    public void Dispose()
	    {
	        base.MainTeardown();
	    }
	}
}
