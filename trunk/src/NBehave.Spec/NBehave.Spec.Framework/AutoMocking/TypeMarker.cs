using System;
using System.Collections.Generic;
using System.Text;
using NBehave.Specs.AutoMocking;

namespace NBehave.Specs.AutoMocking
{
	public class TypeMarker
	{
		private readonly IAutoMockingRepository _repository;
		private readonly Type _type;

		public TypeMarker(Type type, IAutoMockingRepository repository)
		{
			_repository = repository;
			_type = type;
		}

		public void Dynamic()
		{
			_repository.SetMockingStrategy(_type, new DynamicMockingStrategy(_repository));
		}

		public void Stubbed()
		{
			_repository.SetMockingStrategy(_type, new StubbedStrategy(_repository));
		}

		public void NotMocked()
		{
			_repository.SetMockingStrategy(_type, new NonMockedStrategy(_repository));
		}

		public void Missing()
		{
			_repository.MarkMissing(_type);
		}

		public void NonDynamic()
		{
			_repository.SetMockingStrategy(_type, new StandardMockingStrategy(_repository));
		}
	}
}