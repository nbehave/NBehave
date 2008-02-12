using System;
using Castle.MicroKernel;
using Rhino.Mocks;

namespace NBehave.Specs.AutoMocking
{
	public interface IAutoMockingRepository
	{
		MockRepository MockRepository { get; }
		IKernel Kernel { get; }
		bool CanResolve(Type type);
		object Get(Type type);
		IMockingStrategy GetMockingStrategy(Type type);
		void SetMockingStrategy(Type type, IMockingStrategy strategy);
		void MarkMissing(Type type);
		void AddService(Type type, object service);
		TypeMarker Mark(Type type);
		bool CanResolveFromMockRepository(Type service);
	}

	public interface IGenericMockingRepository
	{
		IMockingStrategy GetMockingStrategy<T>();
		void SetMockingStrategy<T>(IMockingStrategy strategy);
		void AddService<T>(T service);
		T Get<T>() where T : class;
		TypeMarker Mark<T>();
		void MarkMissing<T>();
	}
}