using NBehave.Narrator.Framework.EventListeners;
using NUnit.Framework;
using Rhino.Mocks;

namespace NBehave.Narrator.Framework.Specifications.EventListeners
{
    [TestFixture]
    public class MultiOutputEventListenerFixture
    {
        [Test]
        public void ShouldInvokeMethodOnAllSpecifiedListeners()
        {
            var mockFirstEventListener = MockRepository.GenerateMock<IEventListener>();
            var mockSecondEventListener = MockRepository.GenerateMock<IEventListener>();

            IEventListener listener = new MultiOutputEventListener(mockFirstEventListener, mockSecondEventListener);
            listener.RunStarted();
            mockFirstEventListener.AssertWasCalled(l => l.RunStarted());
            mockSecondEventListener.AssertWasCalled(l => l.RunStarted());
        }
    }
}
