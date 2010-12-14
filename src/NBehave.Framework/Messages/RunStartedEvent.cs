using System.Collections.Generic;

namespace NBehave.Narrator.Framework.Messages
{
    using NBehave.Narrator.Framework.Tiny;

    public class RunStartedEvent : TinyMessageBase
    {
        public RunStartedEvent(object sender)
            : base(sender)
        {
        }
    }
}
