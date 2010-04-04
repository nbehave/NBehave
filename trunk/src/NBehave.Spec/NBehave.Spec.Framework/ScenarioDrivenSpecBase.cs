using NBehave.Narrator.Framework;
using Rhino.Mocks;

namespace NBehave.Spec
{
    public abstract class ScenarioDrivenSpecBase
    {
        protected Feature Feature { get; private set; }

        protected abstract Feature CreateFeature();

        public virtual void MainSetup()
        {
            Feature = CreateFeature();
        }

        public virtual void MainTeardown()
        {
        }

        protected TType CreateDependency<TType>()
            where TType : class
        {
            return MockRepository.GenerateMock<TType>();
        }

        protected TType CreateStub<TType>()
            where TType : class
        {
            return MockRepository.GenerateStub<TType>();
        }
    }
}
