using System;

namespace NBehave.Spec.Xunit
{
    public abstract class SpecBase : Spec.SpecBase, IDisposable
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
