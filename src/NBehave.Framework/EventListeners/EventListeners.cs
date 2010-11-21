// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EventListeners.cs" company="NBehave">
//   Copyright (c) 2007, NBehave - http://nbehave.codeplex.com/license
// </copyright>
// <summary>
//   Defines the EventListeners type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace NBehave.Narrator.Framework.EventListeners
{
    using System.IO;
    using System.Text;
    using System.Xml;

    using NBehave.Narrator.Framework.EventListeners.Xml;

    public static class EventListeners
    {
        public static EventListener CreateEventListenerUsing(TextWriter writer, string textWriterFile, string xmlWriterFile)
        {
            var useTextWriter = textWriterFile.NotBlank();
            var useXmlWriter = xmlWriterFile.NotBlank();

            if (useTextWriter && useXmlWriter)
            {
                return new MultiOutputEventListener(
                    FileOutputEventListener(textWriterFile),
                    XmlWriterEventListener(xmlWriterFile),
                    TextWriterEventListener(writer));
            }

            if (useTextWriter)
            {
                return new MultiOutputEventListener(
                    FileOutputEventListener(textWriterFile), TextWriterEventListener(writer));
            }

            if (useXmlWriter)
            {
                return new MultiOutputEventListener(
                    XmlWriterEventListener(xmlWriterFile), TextWriterEventListener(writer));
            }

            return NullEventListener();
        }

        public static EventListener NullEventListener()
        {
            return new NullEventListener();
        }

        public static EventListener FileOutputEventListener(string outputPath)
        {
            return new TextWriterEventListener(File.CreateText(outputPath));
        }

        public static EventListener XmlWriterEventListener(string xmlWriterFile)
        {
            return XmlWriterEventListener(new FileStream(xmlWriterFile, FileMode.Create));
        }

        public static EventListener XmlWriterEventListener(Stream stream)
        {
            var settings = new XmlWriterSettings { Encoding = Encoding.UTF8, Indent = true };
            var writer = XmlWriter.Create(stream, settings);
            return new XmlOutputEventListener(writer);
        }

        public static EventListener CodeGenEventListener(TextWriter writer)
        {
            return new CodeGenEventListener(writer);
        }

        private static EventListener TextWriterEventListener(TextWriter writer)
        {
            return new TextWriterEventListener(writer);
        }

        private static bool Blank(this string value)
        {
            return value == null ? true : string.IsNullOrEmpty(value.Trim());
        }

        private static bool NotBlank(this string value)
        {
            return value.Blank() == false;
        }
    }
}