using System;
using System.Threading;
using NBehave.Spec.Framework;

namespace DummySpecAssembly
{
    [Context]
    public class DummySpecA
    {
        private int _value;

        [ContextSetUp]
        public void ContextSetUp()
        {
            _value = 100;
        }

        [Specification]
        public void DummyTestA()
        {
            Thread.Sleep(100);
            Specify.That(true).ShouldBeTrue();
        }

        [Specification]
        public void DummyTestB()
        {
            Thread.Sleep(100);
            Specify.That(false).ShouldBeTrue();
        }

        [Specification]
        public void DummyTestC()
        {
            Thread.Sleep(100);
            Specify.That(true).ShouldBeTrue();
        }

        [Specification]
        public void MightThrow()
        {
            Thread.Sleep(100);
            throw new InvalidCastException("Bob's your mother's brother.");
        }

        [Specification]
        public void DummyTestD()
        {
            Specify.That(_value).ShouldEqual(100);
        }
    }
}