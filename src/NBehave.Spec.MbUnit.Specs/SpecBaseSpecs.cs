using MbUnit.Framework;
using Rhino.Mocks;

namespace NBehave.Spec.MbUnit.Specs
{
    [TestFixture]
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
            Sut.ShouldNotBeNull();
        }
    }

    [TestFixture]
    public class When_using_the_setup_methods_in_non_generic_specs : SpecBase
    {
        private static int _estContextCount;
        private static int _becauseOfCount;
        private static int _cleanupCount;

        protected override void Establish_context()
        {
            _estContextCount++;
        }

        protected override void Because_of()
        {
            _becauseOfCount++;
        }

        protected override void Cleanup()
        {
            _cleanupCount++;
            _estContextCount.ShouldEqual(1);
            _becauseOfCount.ShouldEqual(1);
            _cleanupCount.ShouldEqual(1);
        }

        [Test]
        public void dummy_test_1()
        {
        }

        [Test]
        public void dummy_test_2()
        {
        }
    }

    [TestFixture]
    public class When_using_the_setup_methods_in_generic_specs : SpecBase<object>
    {
        private static int _estContextCount;
        private static int _becauseOfCount;
        private static int _cleanupCount;

        protected override object Establish_context()
        {
            _estContextCount++;
            return null;
        }

        protected override void Because_of()
        {
            _becauseOfCount++;
        }

        protected override void Cleanup()
        {
            _cleanupCount++;
            _estContextCount.ShouldEqual(1);
            _becauseOfCount.ShouldEqual(1);
            _cleanupCount.ShouldEqual(1);
        }

        [Test]
        public void dummy_test_1()
        {
        }

        [Test]
        public void dummy_test_2()
        {
        }
    }

    [TestFixture]
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