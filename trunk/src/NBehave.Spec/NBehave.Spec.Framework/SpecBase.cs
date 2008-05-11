using System;
using System.Collections.Generic;
using System.Text;
using NBehave.Specs.AutoMocking;
using Rhino.Mocks;

namespace NBehave.Spec
{
	public abstract class SpecBase
	{
		private MockRepository _mocks;
		private AutoMockingContainer _autoMockingContainer;

		
		public virtual void MainSetup()
		{
			_mocks = new MockRepository();
			_autoMockingContainer = new AutoMockingContainer(_mocks);
			_autoMockingContainer.Initialize();

			Before_each_spec();
		}

		
		public virtual void MainTeardown()
		{
			After_each_spec();
		}

		public MockRepository Mocks
		{
			get { return _mocks; }
		}

		protected virtual void Before_each_spec()
		{

		}

		protected virtual void After_each_spec()
		{

		}

		protected IDisposable RecordExpectedBehavior
		{
			get { return _mocks.Record(); }
		}

		protected IDisposable PlaybackBehavior
		{
			get { return _mocks.Playback(); }
		}

		protected TType Mock<TType>()
		{
			return _mocks.DynamicMock<TType>();
		}

		protected TType Mock<TType>(object[] prams)
		{
			return _mocks.DynamicMock<TType>(prams);
		}

		protected TType Partial<TType>()
		   where TType : class
		{
			return _mocks.PartialMock<TType>();
		}

		protected TType Get<TType>() where TType : class
		{
			return _autoMockingContainer.Get<TType>();
		}

		protected TType Create<TType>() where TType : class
		{
			return _autoMockingContainer.Create<TType>();
		}

		protected TType Stub<TType>()
		{
			return _mocks.Stub<TType>();
		}

		protected void Verify(object mock)
		{
			_mocks.Verify(mock);
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
