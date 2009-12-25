namespace NBehave.Narrator.Framework
{
	public class ActionStepText
	{
		public string Step {get; set;}
		public string FromFile {get; private set;}
		
		public ActionStepText(string text, string fromFile)
		{
			Step = text;
			FromFile = fromFile;
		}
		
		public override string ToString()
		{
			return Step;
		}
	}	
}
