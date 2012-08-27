using System;
using Microsoft.VisualStudio.Text;

namespace NBehave.VS2010.Plugin.Tagging
{
    public class TokenParserEventArgs : EventArgs
    {
        public TokenParserEventArgs(GherkinParseEvent evt, SnapshotSpan snapshotSpan)
        {
            Event = evt;
            SnapshotSpan = snapshotSpan;
        }

        public GherkinParseEvent Event { get; private set; }
        public SnapshotSpan SnapshotSpan { get; private set; }
    }
}