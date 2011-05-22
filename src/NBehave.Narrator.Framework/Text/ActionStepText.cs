namespace NBehave.Narrator.Framework
{
	public class ActionStepText
	{
		public string Step {get; set;}
		public string Source {get; set;}
		
		public ActionStepText(string text, string source)
		{
			Step = text;
			Source = source;
		}
		
		public override string ToString()
		{
			return Step;
		}
	}	
}
