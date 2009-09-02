namespace NBehave.Narrator.Framework
{
    public class MessageEventData
    {
        public MessageEventData(string type, string message)
        {
            Type = type;
            Message = message;
        }

        public string Type { get; private set; }
        public string Message { get; private set; }
    }
}