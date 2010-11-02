using System;
using System.Threading;
using NBehave.Spec.Framework;

namespace DummySpecAssembly
{
    [Context]
    public class DummySpecB
    {
        [Specification]
        public void DummyTestD()
        {
            Thread.Sleep(100);
            Specify.That(true).ShouldBeTrue();
        }

        [Specification]
        public void DummyTestE()
        {
            Thread.Sleep(100);
            Specify.That(false).ShouldBeTrue();
        }
    }
}