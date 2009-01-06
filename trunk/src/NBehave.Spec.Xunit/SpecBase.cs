using System;

namespace NBehave.Spec.Xunit
{
	public abstract class SpecBase : Spec.SpecBase, IDisposable
	{
		protected SpecBase()
        {
            base.SpecSetup();
        }

	    public void Dispose()
	    {
	        base.SpecTeardown();
	    }
	}
}
