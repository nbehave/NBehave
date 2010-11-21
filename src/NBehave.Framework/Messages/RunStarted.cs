using System.Collections.Generic;

namespace NBehave.Narrator.Framework.Messages
{
    using NBehave.Narrator.Framework.Tiny;

    public class RunStarted : TinyMessageBase
    {
        public RunStarted(object sender)
            : base(sender)
        {
        }
    }
}
