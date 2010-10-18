using Rhino.Mocks;

namespace NBehave.Spec
{
    public abstract class SpecBase
    {
        public virtual void MainSetup()
        {
            Establish_context();
            Because_of();
        }

        public virtual void MainTeardown()
        {
            Cleanup();
        }

        protected virtual void Establish_context()
        {
        }

        protected virtual void Because_of()
        {
        }

        protected virtual void Cleanup()
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