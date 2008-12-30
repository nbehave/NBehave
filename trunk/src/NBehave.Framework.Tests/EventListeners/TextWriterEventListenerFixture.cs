using System.IO;
using NBehave.Narrator.Framework.EventListeners;
using NUnit.Framework;
using Rhino.Mocks;
using Rhino.Mocks.Constraints;

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
            MockRepository mockery = new MockRepository();

            TextWriter mockTextWriter = mockery.StrictMock<TextWriter>();

            using (mockery.Record())
            {
                Expect.Call(delegate { mockTextWriter.WriteLine(string.Empty); }).IgnoreArguments().Constraints(Is.TypeOf<string>());
            }

            TextWriterEventListener listener = new TextWriterEventListener(mockTextWriter);
            using (mockery.Playback())
            {
                listener.RunStarted();
            }

            mockery.VerifyAll();
        }
    }
}
