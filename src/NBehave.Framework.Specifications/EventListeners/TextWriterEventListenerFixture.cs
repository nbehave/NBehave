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
            var mockTextWriter = MockRepository.GenerateMock<TextWriter>();

            IEventListener listener = new TextWriterEventListener(mockTextWriter);
            listener.RunStarted();
            mockTextWriter.AssertWasCalled(w => w.WriteLine(""), opt => opt.IgnoreArguments().Constraints(IsIt.TypeOf<string>()));
        }
    }
}
