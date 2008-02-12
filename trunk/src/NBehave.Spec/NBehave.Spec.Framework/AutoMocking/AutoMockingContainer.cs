using System;
using System.Collections;
using System.Collections.Generic;
using Castle.MicroKernel;
using Castle.MicroKernel.SubSystems.Naming;
using Castle.Windsor;
using Rhino.Mocks;

namespace NBehave.Specs.AutoMocking
{
	public class AutoMockingContainer : WindsorContainer, IAutoMockingRepository, IGenericMockingRepository
	{
		private readonly IList<Type> _markMissing = new List<Type>();
		private readonly MockRepository _mocks;
		private readonly Dictionary<Type, object> _services = new Dictionary<Type, object>();
		private readonly Dictionary<Type, IMockingStrategy> _strategies = new Dictionary<Type, IMockingStrategy>();

		public AutoMockingContainer(MockRepository mocks)
		{
			_mocks = mocks;
		}

		#region IAutoMockingRepository Members
		public virtual MockRepository MockRepository
		{
			get { return _mocks; }
		}

		private object GetService(Type type) 
		{
			if (_services.ContainsKey(type))
				return _services[type];
			return null;
		}

		public virtual IMockingStrategy GetMockingStrategy(Type type)
		{
			if (_strategies.ContainsKey(type))
			{
				return _strategies[type];
			}
			return new DynamicMockingStrategy(this);
		}

		public virtual IMockingStrategy GetMockingStrategy<T>()
		{
			return GetMockingStrategy(typeof (T));
		}

		public bool CanResolve(Type type)
		{
			return _markMissing.Contains(type) == false;
		}
		#endregion

		public void Initialize()
		{
			Kernel.AddSubSystem(SubSystemConstants.NamingKey,new AutoMockingNamingSubSystem(this));
			Kernel.AddFacility("AutoMockingFacility", new AutoMockingFacility(this));
		}


		private void AddComponentIfMissing<T>()
		{
			Type targetType = typeof(T);
			if (!Kernel.HasComponent(targetType.FullName))
				AddComponent(targetType.FullName, targetType);
		}

		public T Create<T>()
		{
			AddComponentIfMissing<T>();
			return Resolve<T>();
		}

		public T Create<T>(IDictionary parameters)
		{
			AddComponentIfMissing<T>();
			return Resolve<T>(parameters);
		}

		public object Get(Type type)
		{
			if (type == typeof(IKernel))
				return Kernel;
			object t = GetService(type);
			if (t != null)
				return t;

			object instance = GetMockingStrategy(type).Create(CreationContext.Empty, type);
			AddService(type, instance);
			return instance;
		}

		public T Get<T>() where T : class
		{
			return (T) Get(typeof (T));
		}

		public void SetMockingStrategy(Type type, IMockingStrategy strategy)
		{
			_strategies[type] = strategy;
		}

		public void SetMockingStrategy<T>(IMockingStrategy strategy)
		{
			SetMockingStrategy(typeof(T), strategy);
		}

		public void AddService(Type type, object service)
		{
			_services[type] = service;
		}
        
		public void AddService<T>(T service)
		{
			AddService(typeof (T), service);
		}

		public TypeMarker Mark(Type type)
		{
			return new TypeMarker(type, this);
		}

		public bool CanResolveFromMockRepository(Type service)
		{
			return _markMissing.Contains(service) == false && 
			       GetMockingStrategy(service).GetType() != typeof (NonMockedStrategy);
		}

		public TypeMarker Mark<T>()
		{
			return Mark(typeof (T));
		}

		public void MarkMissing<T>()
		{
			MarkMissing(typeof (T));
		}

		public void MarkMissing(Type type)
		{
			_markMissing.Add(type);
		}
	}
}