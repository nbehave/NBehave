using System;
using Castle.MicroKernel;

namespace NBehave.Specs.AutoMocking
{
	public interface IMockingStrategy
	{
		object Create(CreationContext context, Type type);
	}
}