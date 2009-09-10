using System;

namespace NBehave.Narrator.Framework
{
	public class ActionStepText
	{
		public string Text {get; private set;}
		public string FromFile {get; private set;}
		
		public ActionStepText(string text, string fromFile)
		{
			Text = text;
			FromFile = fromFile;
		}
		
		public override string ToString()
		{
			return Text;
		}
	}	
}
