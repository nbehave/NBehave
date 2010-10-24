using NBehave.Spec.Xunit;
using NUnit.Framework;
using Rhino.Mocks;

namespace Xunit.SpecBase_Specifications
{
	public class When_initializing_the_SpecBase : SpecBase<StopWatch>
	{
		protected override StopWatch Establish_context()
		{
			return new StopWatch();
		}

		protected override void Because_of()
		{
		}

		[Test]
		public void should_populate_the_SUT_before_starting_the_specification()
		{
			Assert.NotNull(Sut);
		}
	}

	public class When_initializing_the_SpecBase_with_mocks : SpecBase<StopWatch>
	{
		private ITimer _timer;

		protected override StopWatch Establish_context()
		{
			_timer = CreateDependency<ITimer>();

			_timer.Stub(x => x.Start(null)).IgnoreArguments().Return(true);

			return new StopWatch(_timer);
		}

		protected override void Because_of()
		{
			Sut.Start();
		}

		[Test]
		public void should_call_the_before_each_spec_before_starting_the_specification()
		{
			_timer.AssertWasCalled(x => x.Start(null), opt => opt.IgnoreArguments());
		}
	}

	public class StopWatch
	{
		private readonly ITimer _timer;

		public StopWatch()
		{
		}

		public StopWatch(ITimer timer)
		{
			_timer = timer;
		}

		public void Start()
		{
			_timer.Start("");
		}
	}

	public interface ITimer
	{
		bool Start(string reason);
		void Start();
	}
}
