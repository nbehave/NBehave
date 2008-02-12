using Castle.Core;
using Castle.MicroKernel;
using Castle.MicroKernel.SubSystems.Naming;

namespace NBehave.Specs.AutoMocking
{
	public class AutoMockingDependencyResolver : ISubDependencyResolver
	{
		private IAutoMockingRepository _autoMock;

		public AutoMockingDependencyResolver(IAutoMockingRepository autoMock)
		{
			_autoMock = autoMock;
		}

		public IAutoMockingRepository AutoMock
		{
			get { return _autoMock; }
		}

		#region ISubDependencyResolver Members

		public bool CanResolve(CreationContext context, ISubDependencyResolver parentResolver, ComponentModel model,
		                       DependencyModel dependency)
		{
			return dependency.DependencyType == DependencyType.Service &&
			       AutoMock.CanResolve(dependency.TargetType);
		}

		public object Resolve(CreationContext context, ISubDependencyResolver parentResolver, ComponentModel model,
		                      DependencyModel dependency)
		{
			return AutoMock.Get(dependency.TargetType);
		}

		#endregion
	}
}