using System.IO;
using NBehave.Narrator.Framework.EventListeners;
using NUnit.Framework;
using Rhino.Mocks;
using IsIt = Rhino.Mocks.Constraints.Is;

namespace NBehave.Narrator.Framework.Specifications.EventListeners
{
    [TestFixture]
    public class TextWriterEventListenerFixture
    {
        /// <summary>
        /// Just a smoke test really...
        /// </summary>
        [Test]
        public void Should_write_text_to_specified_text_writer()
        {
            var mockery = new MockRepository();
            var mockTextWriter = mockery.StrictMock<TextWriter>();

            using (mockery.Record())
            {
                Expect.Call(() => mockTextWriter.WriteLine(string.Empty)).IgnoreArguments().Constraints(IsIt.TypeOf<string>());
            }

            IEventListener listener = new TextWriterEventListener(mockTextWriter);
            using (mockery.Playback())
            {
                listener.RunStarted();
            }

            mockery.VerifyAll();
        }
    }
}
