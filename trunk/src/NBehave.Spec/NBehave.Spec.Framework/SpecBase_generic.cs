using System;
using Rhino.Mocks;

namespace NBehave.Spec
{
	public abstract class SpecBase<T>
	{
		private MockRepository _mocks;

		public virtual void MainSetup()
		{
			Before_all_specs();
		}

		public virtual void MainTeardown()
		{
			After_all_specs();
		}

		public virtual void SpecSetup()
		{
			_mocks = new MockRepository();

			Before_each_spec();
			Sut = Given_these_conditions();
			Because();
		}

		public virtual void SpecTeardown()
		{
			After_each_spec();
		}

		protected abstract void Because();

		protected abstract T Given_these_conditions();

		protected virtual void Before_each_spec() {}

		protected virtual void After_each_spec() {}

		protected virtual void Before_all_specs() {}

		protected virtual void After_all_specs() {}

		protected T Sut { get; private set; }

		protected MockRepository Mocks
		{
			get { return _mocks; }
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

		protected void VerifyAll()
		{
			_mocks.VerifyAll();
		}

		protected void Spec_not_implemented()
		{
			Console.WriteLine("Not implemented");
		}
	}
}
