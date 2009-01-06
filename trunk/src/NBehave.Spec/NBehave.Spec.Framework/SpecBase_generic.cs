using System;
using Rhino.Mocks;

namespace NBehave.Spec
{
	public abstract class SpecBase<TContext>
	{
		public virtual void MainSetup()
		{
			Sut = Establish_context();
			Because_of();
		}

		public virtual void MainTeardown()
		{
			Cleanup();
		}

		protected virtual void Because_of() {}
		protected abstract TContext Establish_context();
		protected virtual void Cleanup() {}

		protected virtual TContext Sut { get; private set; }

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
