using System;

namespace NBehave.Spec.Xunit
{
	public class XunitSpecBase : SpecBase, IDisposable
	{
        public XunitSpecBase()
        {
            base.MainSetup();
        }

	    public void Dispose()
	    {
	        base.MainTeardown();
	    }
	}
}
