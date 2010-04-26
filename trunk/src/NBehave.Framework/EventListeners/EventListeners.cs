using System.IO;
using System.Text;
using System.Xml;
using NBehave.Narrator.Framework.EventListeners.Xml;

namespace NBehave.Narrator.Framework.EventListeners
{
    public static class EventListeners
    {
        public static IEventListener CreateEventListenerUsing(TextWriter writer, string textWriterFile, string xmlWriterFile)
        {
            bool useTextWriter = textWriterFile.NotBlank();
            bool useXmlWriter = xmlWriterFile.NotBlank();

            if (useTextWriter && useXmlWriter)
                return new MultiOutputEventListener(FileOutputEventListener(textWriterFile),
                                                    XmlWriterEventListener(xmlWriterFile),
                                                    TextWriterEventListener(writer));
            if (useTextWriter)
                return new MultiOutputEventListener(FileOutputEventListener(textWriterFile),
                                                    TextWriterEventListener(writer));

            if (useXmlWriter)
                return
                    new MultiOutputEventListener(
                        XmlWriterEventListener(xmlWriterFile),
                        TextWriterEventListener(writer));

            return NullEventListener();
        }

        private static IEventListener NullEventListener()
        {
            return new NullEventListener();
        }

        public static IEventListener FileOutputEventListener(string outputPath)
        {
            return new TextWriterEventListener(File.CreateText(outputPath));
        }

        private static IEventListener TextWriterEventListener(TextWriter writer)
        {
            return new TextWriterEventListener(writer);
        }

        public static IEventListener XmlWriterEventListener(string xmlWriterFile)
        {
            return XmlWriterEventListener(new FileStream(xmlWriterFile, FileMode.Create));
        }

        public static IEventListener XmlWriterEventListener(Stream stream)
        {
            var settings = new XmlWriterSettings();
            settings.Encoding = Encoding.UTF8;
            settings.Indent = true;
            XmlWriter writer = XmlWriter.Create(stream, settings);
            return new XmlOutputEventListener(writer);
        }

        private static bool Blank(this string value)
        {
            return value == null ? true : string.IsNullOrEmpty(value.Trim());
        }

        private static bool NotBlank(this string value)
        {
            return value.Blank() == false;
        }

        public static IEventListener CodeGenEventListener(TextWriter writer)
        {
            return new CodeGenEventListener(writer);
        }
    }
}