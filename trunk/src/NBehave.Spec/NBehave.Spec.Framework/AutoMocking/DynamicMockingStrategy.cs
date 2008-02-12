using System;
using Castle.MicroKernel;
using NBehave.Specs.AutoMocking;

namespace NBehave.Specs.AutoMocking
{
	public class DynamicMockingStrategy : AbstractMockingStrategy
	{
		public DynamicMockingStrategy(IAutoMockingRepository autoMock) : base(autoMock)
		{
		}

		public override object Create(CreationContext context, Type type)
		{
			return Mocks.DynamicMock(type);
		}
	}
}