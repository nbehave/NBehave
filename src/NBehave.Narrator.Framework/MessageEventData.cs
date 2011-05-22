namespace NBehave.Narrator.Framework
{
    public class MessageEventData
    {
        public MessageEventData(MessageEventType type, string message)
        {
            Type = type;
            Message = message;
        }

        public MessageEventType Type { get; private set; }
        public string Message { get; private set; }
    }

    public enum MessageEventType
    {
        Uknown,
        StringStep, Pending,
        Scenario,
        Feature
    }
}