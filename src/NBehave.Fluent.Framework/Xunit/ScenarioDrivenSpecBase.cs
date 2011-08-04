using System;

namespace NBehave.Fluent.Framework.Xunit
{
    public abstract class ScenarioDrivenSpecBase : Framework.ScenarioDrivenSpecBase, IDisposable
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
