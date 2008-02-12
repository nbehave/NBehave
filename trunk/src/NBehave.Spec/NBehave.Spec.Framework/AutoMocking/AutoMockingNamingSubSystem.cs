using System;
using System.Collections.Generic;
using Castle.Core;
using Castle.MicroKernel;
using Castle.MicroKernel.SubSystems.Naming;

namespace NBehave.Specs.AutoMocking
{
	public class AutoMockingNamingSubSystem : DefaultNamingSubSystem
	{
		private IAutoMockingRepository autoMocking;
		public AutoMockingNamingSubSystem(IAutoMockingRepository autoMocking)
		{
			this.autoMocking = autoMocking;
		}

		public override bool Contains(System.Type service)
		{
			if (base.Contains(service))
				return true;
			
			return autoMocking.CanResolveFromMockRepository(service);
		}

		public override IHandler GetHandler(Type service)
		{
			IHandler handler = base.GetHandler(service);
			if (handler != null)
				return handler;
			if (autoMocking.CanResolveFromMockRepository(service))
				return new AutoMocking(autoMocking, service);
			return handler;
		}

		public class AutoMocking : IHandler
		{
			private IAutoMockingRepository autoMocking;
			private Type service;
			private ComponentModel model;

			public AutoMocking(IAutoMockingRepository autoMocking, Type service)
			{
				this.autoMocking = autoMocking;
				this.service = service;
				model = new ComponentModel(service.FullName, service, service);
			}

			public event HandlerStateDelegate OnHandlerStateChanged
			{
				add { }
				remove { }
			}

			public void Init(IKernel kernel)
			{
			}

			public object Resolve(CreationContext context)
			{
				object o = autoMocking.Get(service);
				return o;
			}

			public void Release(object instance)
			{
			}

			public void AddCustomDependencyValue(string key, object value)
			{
			}

			public void RemoveCustomDependencyValue(string key)
			{
			}

			public bool HasCustomParameter(string key)
			{
				return false;
			}

			public HandlerState CurrentState
			{
				get { return HandlerState.Valid; }
			}

			public ComponentModel ComponentModel
			{
				get { return model; }
			}

			public object Resolve(CreationContext context, ISubDependencyResolver parentResolver, ComponentModel model, DependencyModel dependency)
			{
				throw new NotImplementedException();
			}

			public bool CanResolve(CreationContext context, ISubDependencyResolver parentResolver, ComponentModel model, DependencyModel dependency)
			{
				return false;
			}
		}
	}
}