using System;
using System.Text;
using System.IO;
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
			
			if (useTextWriter && useTextWriter)
				return  new MultiOutputEventListener(EventListeners.FileOutputEventListener(textWriterFile),
				                                     EventListeners.XmlWriterEventListener(new XmlTextWriter(xmlWriterFile, Encoding.UTF8)),
				                                     EventListeners.TextWriterEventListener(writer));
			if(useTextWriter)
				return  new MultiOutputEventListener(EventListeners.FileOutputEventListener(textWriterFile),
				                                     EventListeners.TextWriterEventListener(writer));
			
			if(useXmlWriter)
				return  new MultiOutputEventListener(EventListeners.XmlWriterEventListener(new XmlTextWriter(xmlWriterFile, Encoding.UTF8)),
				                                     EventListeners.TextWriterEventListener(writer));
			
			return EventListeners.NullEventListener();
		}
		
		public static IEventListener NullEventListener()
		{
			return new NullEventListener();
		}

		public static IEventListener FileOutputEventListener(string storyOutputPath)
		{
			return new FileOutputEventListener(storyOutputPath);
		}

		public static IEventListener TextWriterEventListener(TextWriter writer)
		{
			return new TextWriterEventListener(writer);
		}
		
		public static IEventListener XmlWriterEventListener(XmlWriter writer)
		{
			return new XmlOutputEventListener(writer);
		}
		
		private static bool Blank(this string value)
        {
            return value == null ? true : string.IsNullOrEmpty(value.Trim());
        }
        
        private static bool NotBlank(this string value)
        {
        	return value.Blank()==false;
        }
	}
}
