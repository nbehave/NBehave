using System;

namespace NBehave.Spec.Xunit
{
    public abstract class ScenarioDrivenSpecBase : Spec.ScenarioDrivenSpecBase, IDisposable
    {
        protected ScenarioDrivenSpecBase()
        {
            base.MainSetup();
        }

        public void Dispose()
        {
            base.MainTeardown();
        }
    }
}
