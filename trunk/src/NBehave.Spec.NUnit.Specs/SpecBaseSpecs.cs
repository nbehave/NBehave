using System.Diagnostics;
using NBehave.Spec.NUnit;
using NUnit.Framework;
using Rhino.Mocks;
using Context = NUnit.Framework.TestFixtureAttribute;
using Specification = NUnit.Framework.TestAttribute;

namespace NUnit.SpecBase_Specifications
{
    [Context]
    public class When_initializing_the_SpecBase : SpecBase<StopWatch>
    {
    	protected override StopWatch Given_these_conditions()
    	{
    		return new StopWatch();
    	}

    	protected override void Because()
    	{
    	}

        [Specification]
        public void should_populate_the_SUT_before_starting_the_specification()
        {
            Sut.ShouldNotBeNull();
        }
    }

	[Context]
	public class When_using_the_setup_methods_in_non_generic_specs : SpecBase
	{
		private static int _beforeEachCount;
		private static int _beforeAllCount;
		private static int _afterEachCount;
		private static int _afterAllCount;

		protected override void Before_each_spec()
		{
			_beforeEachCount++;
		}

		protected override void Before_all_specs()
		{
			_beforeAllCount++;
		}

		protected override void After_each_spec()
		{
			_afterEachCount++;
		}

		protected override void After_all_specs()
		{
			_afterAllCount++;
			_beforeEachCount.ShouldEqual(2);
			_beforeAllCount.ShouldEqual(1);
			_afterEachCount.ShouldEqual(2);
			_afterAllCount.ShouldEqual(1);
		}

		[Specification]
		public void dummy_test_1()
		{
		}

		[Specification]
		public void dummy_test_2()
		{
		}
	}

	[Context]
	public class When_using_the_setup_methods_in_generic_specs : SpecBase<object>
	{
		private static int _beforeEachCount;
		private static int _beforeAllCount;
		private static int _afterEachCount;
		private static int _afterAllCount;

		protected override void Because()
		{
		}

		protected override object Given_these_conditions()
		{
			return null;
		}

		protected override void Before_each_spec()
		{
			_beforeEachCount++;
		}

		protected override void Before_all_specs()
		{
			_beforeAllCount++;
		}

		protected override void After_each_spec()
		{
			_afterEachCount++;
		}

		protected override void After_all_specs()
		{
			_afterAllCount++;
			_beforeEachCount.ShouldEqual(2);
			_beforeAllCount.ShouldEqual(1);
			_afterEachCount.ShouldEqual(2);
			_afterAllCount.ShouldEqual(1);
		}

		[Specification]
		public void dummy_test_1()
		{
		}

		[Specification]
		public void dummy_test_2()
		{
		}
	}

    [Context]
    public class When_initializing_the_SpecBase_with_mocks : SpecBase<StopWatch>
    {
        private ITimer _timer;

    	protected override StopWatch Given_these_conditions()
    	{
			_timer = CreateDependency<ITimer>();

    		_timer.Stub(x => x.Start(null)).IgnoreArguments().Return(true);

			return new StopWatch(_timer);
		}

    	protected override void Because()
    	{
			Sut.Start();
    	}

        [Specification]
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
