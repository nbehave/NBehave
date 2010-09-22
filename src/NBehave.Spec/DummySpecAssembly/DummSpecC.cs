using System;
using System.Threading;
using NBehave.Spec.Framework;

namespace DummySpecAssembly
{
    [Context]
    public class DummSpecC
    {
        [Specification]
        public void DummyTestE()
        {
            Thread.Sleep(100);
            Specify.That(true).ShouldBeTrue();
        }
    }
}