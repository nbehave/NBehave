using NBehave.EventListeners;
using NUnit.Framework;
using Rhino.Mocks;

namespace NBehave.Specifications.EventListeners
{
    [TestFixture]
    public class MultiOutputEventListenerSpec
    {
        [Test]
        public void ShouldInvokeMethodOnAllSpecifiedListeners()
        {
            var mockFirstEventListener = MockRepository.GenerateMock<IEventListener>();
            var mockSecondEventListener = MockRepository.GenerateMock<IEventListener>();

            EventListener listener = new MultiOutputEventListener(mockFirstEventListener, mockSecondEventListener);
            listener.RunStarted();
            mockFirstEventListener.AssertWasCalled(l => l.RunStarted());
            mockSecondEventListener.AssertWasCalled(l => l.RunStarted());
        }
    }
}
