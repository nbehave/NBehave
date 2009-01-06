using NBehave.Narrator.Framework.EventListeners;
using NUnit.Framework;
using Rhino.Mocks;

namespace NBehave.Narrator.Framework.Specifications.EventListeners
{
    [TestFixture]
    public class MultiOutputEventListenerFixture
    {
        [Test]
        public void Should_invoke_method_on_all_specified_listeners()
        {
            MockRepository mockery = new MockRepository();

            IEventListener mockFirstEventListener = mockery.CreateMock<IEventListener>();
            IEventListener mockSecondEventListener = mockery.CreateMock<IEventListener>();

            using (mockery.Record())
            {
                Expect.Call(delegate{mockFirstEventListener.RunStarted();});
                Expect.Call(delegate{mockSecondEventListener.RunStarted();});
            }

            IEventListener listener = new MultiOutputEventListener(mockFirstEventListener, mockSecondEventListener);
            using (mockery.Playback())
            {
                listener.RunStarted();
            }

            mockery.VerifyAll();
        }
    }
}
