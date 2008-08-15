using System;

namespace NBehave.Spec.Xunit
{
	public abstract class SpecBase<T> : Spec.SpecBase<T>, IDisposable
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
