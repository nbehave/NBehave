using System;
using Castle.MicroKernel;
using NBehave.Specs.AutoMocking;

namespace NBehave.Specs.AutoMocking
{
	public class StandardMockingStrategy : AbstractMockingStrategy
	{
		#region StandardMockingStrategy()

		public StandardMockingStrategy(IAutoMockingRepository autoMock) : base(autoMock)
		{
		}

		#endregion

		public override object Create(CreationContext context, Type type)
		{
			return Mocks.CreateMock(type);
		}
	}
}