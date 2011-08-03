using System;

namespace NBehave.Spec.Xunit
{
	public abstract class SpecBase<TContext> : Fluent.SpecBase<TContext>, IDisposable
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
