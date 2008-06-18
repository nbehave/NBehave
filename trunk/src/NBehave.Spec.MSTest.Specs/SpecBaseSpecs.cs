using Rhino.Mocks;
using Rhino.Mocks.Exceptions;
using Context = Microsoft.VisualStudio.TestTools.UnitTesting.TestClassAttribute;
using Specification = Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute;
using NBehave.Spec.MSTest;

namespace MSTest.SpecBase_Specifications
{
	[Context]
	public class When_initializing_the_SpecBase : MSTestSpecBase
	{
		private StopWatch _stopWatch;

		protected override void Before_each_spec()
		{
			_stopWatch = new StopWatch();
		}

		[Specification]
		public void should_call_the_before_each_spec_before_starting_the_specification()
		{
			_stopWatch.ShouldNotBeNull();
		}
	}

	[Context]
	public class When_initializing_the_SpecBase_with_mocks : MSTestSpecBase
	{
		private StopWatch _stopWatch;
		private ITimer _timer;

		protected override void Before_each_spec()
		{
			_timer = Mock<ITimer>();
			_stopWatch = new StopWatch(_timer);
		}

		[Specification]
		public void should_call_the_before_each_spec_before_starting_the_specification()
		{
			using (RecordExpectedBehavior)
			{
				Expect.Call(_timer.Start(null))
					.IgnoreArguments().Return(true);
			}

			using (PlaybackBehavior)
			{
				_stopWatch.Start();
			}
		}
	}

    [Context]
    public class When_overriding_mocking_strategy_in_before_each_spec : MSTestSpecBase
    {
        protected override void Before_each_spec()
        {
            Mark<ITimer>().NonDynamic();
        }

        [Specification]
        public void should_apply_the_new_mocking_strategy()
        {
            using (RecordExpectedBehavior)
            {
                Expect.Call(Get<ITimer>().Start);
            }

            Mocks.ReplayAll();

            Get<ITimer>().Start();
            typeof(ExpectationViolationException).ShouldBeThrownBy(Get<ITimer>().Start);
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
