using System;
using Castle.MicroKernel;
using NBehave.Specs.AutoMocking;

namespace NBehave.Specs.AutoMocking
{
	public class NonMockedStrategy : AbstractMockingStrategy
	{
		#region NonMockedStrategy()

		public NonMockedStrategy(IAutoMockingRepository autoMock) : base(autoMock)
		{
		}

		#endregion

		public override object Create(CreationContext context, Type type)
		{
			return AutoMock.Kernel[type];
		}
	}
}