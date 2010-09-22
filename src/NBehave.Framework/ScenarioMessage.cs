namespace NBehave.Narrator.Framework
{
    public class ScenarioMessage
    {
        public ScenarioMessage(string category, string message)
        {
            Category = category;
            Message = message;
        }

        public string Category { get; private set; }
        public string Message { get; private set; }
    }
}