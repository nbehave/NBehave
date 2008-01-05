using System;

namespace NBehave.Spec.Framework.Specification.Mocks
{
    public interface IMockable
    {
        int Foo();
        string Bar();
        void FooBar();
    }

    public abstract class MockableBase : IMockable
    {
        #region IMockable Members

        int IMockable.Foo()
        {
            return 5;
        }

        public abstract string Bar();
        public abstract void FooBar();

        #endregion

        public abstract int MethodWithOutParam(out int someInt);
    }

    [Context]
    public class StrictMockSpec
    {
        private int i;
        private string s;

        protected StrictMockSpec(int i, string s)
        {
            //For mock testing.
            this.i = i;
            this.s = s;
        }

        public StrictMockSpec()
        {
        }

        [Specification]
        public void ShouldReturnStrictMockOfCorrectType()
        {
            Interaction i = new Interaction();
            object mock = i.CreateStrictMock<StrictMockSpec>();

            Specify.That(mock.GetType().FullName).ShouldEqual("NBehave.Spec.Framework.Specification.Mocks.__Proxy__StrictMockSpec");
        }

        [Specification]
        public void ShouldReturnStrictMockWhenSupplyingCtorArgs()
        {
            Interaction i = new Interaction();
            StrictMockSpec mock = i.CreateStrictMock<StrictMockSpec>(5, "Test");

            Specify.That(mock.i).ShouldEqual(5);
            Specify.That(mock.s).ShouldEqual("Test");
            Specify.That(mock.GetType().FullName).ShouldEqual("NBehave.Spec.Framework.Specification.Mocks.__Proxy__StrictMockSpec");
        }

        [Specification]
        public void ShouldCreateStrictInstanceOfInterfaceWithDefaultCtor()
        {
            Interaction i = new Interaction();
            IMockable fooMock = i.CreateStrictMock<IMockable>();

            Specify.That(fooMock).ShouldNotBeNull();
        }

        [Specification]
        public void ShouldCreateStrictInstanceOfAbstractType()
        {
            Interaction i = new Interaction();
            MockableBase fooBaseMock = i.CreateStrictMock<MockableBase>();

            Specify.That(fooBaseMock).ShouldNotBeNull();
        }

        [Specification]
        public void ShouldThrowExceptionWhenNoMatchingConstructorCanBeFoundForStrictMock()
        {
            MethodThatThrows mtt = delegate()
                                       {
                                           Interaction i = new Interaction();
                                           MockableBase fooBase = i.CreateStrictMock<MockableBase>(1, 2, 3);
                                       };

            Specify.ThrownBy(mtt).ShouldBeOfType(typeof (NoMatchingConstructorException));
        }

        [Specification]
        public void ShouldRegisterFailureWithUnexpectedMethodCall()
        {
        }
    }
}