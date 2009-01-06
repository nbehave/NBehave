using System;
using System.Text;
using NBehave.Narrator.Framework;

namespace NBehave.Narrator.Framework
{
    public class DebugMessageProvider : IMessageProvider
    {
        public void AddMessage(string message)
        {
            System.Diagnostics.Debug.WriteLine(message);
        }
    }

    public class StringBuilderMessageProvider : IMessageProvider
    {
        private StringBuilder _sb = new StringBuilder();

        public void AddMessage(string message)
        {
            _sb.AppendLine(message);
        }

        public override string ToString()
        {
            return _sb.ToString();
        }
    }

    public class MessageProviderRegistry
    {
        [ThreadStatic]
        private static IMessageProvider _messageProvider;

        public static IMessageProvider GetInstance()
        {
            if (_messageProvider == null)
                RegisterInstance(new DebugMessageProvider());

            return _messageProvider;
        }

        public static void RegisterInstance(IMessageProvider messageProvider)
        {
            if (messageProvider == null)
                throw new ArgumentNullException("messageProvider");

            _messageProvider = messageProvider;
        }

        public static IDisposable RegisterScopedInstance(IMessageProvider messageProvider)
        {
            _messageProvider = messageProvider;

            return new DisposableAction(delegate() { _messageProvider = null; });
        }

        public static void ClearRegistry()
        {
            _messageProvider = null;
        }
    }
}