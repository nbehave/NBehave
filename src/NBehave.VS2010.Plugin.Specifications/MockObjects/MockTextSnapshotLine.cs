using System;
using System.Linq;
using Microsoft.VisualStudio.Text;

namespace NBehave.VS2010.Plugin.Specifications.MockObjects
{
    public class MockTextSnapshotLine : ITextSnapshotLine
    {
        private readonly int lineNumber;
        private readonly MockTextSnapshot snapshot;
        private readonly string text;

        public MockTextSnapshotLine(string text, int lineNumber, MockTextSnapshot snapshot)
        {
            this.lineNumber = lineNumber;
            this.snapshot = snapshot;
            this.text = text.Split(new[] { Environment.NewLine }, StringSplitOptions.None)[lineNumber] + Environment.NewLine;
        }

        public string GetText()
        {
            return text;
        }

        public string GetTextIncludingLineBreak()
        {
            throw new NotImplementedException();
        }

        public string GetLineBreakText()
        {
            throw new NotImplementedException();
        }

        public ITextSnapshot Snapshot
        {
            get { return snapshot; }
        }

        public SnapshotSpan Extent
        {
            get { throw new NotImplementedException(); }
        }

        public SnapshotSpan ExtentIncludingLineBreak
        {
            get { throw new NotImplementedException(); }
        }

        public int LineNumber
        {
            get { return lineNumber; }
        }

        public SnapshotPoint Start
        {
            get
            {
                if (LineNumber == 0)
                {
                    return new SnapshotPoint(Snapshot, 0);
                }

                string aggregate = Snapshot.Lines.Take(LineNumber).Select(line => line.GetText()).Aggregate((s, s1) => s + s1);
                int numberOfCharactersSoFar = aggregate.ToCharArray().Length;
                return new SnapshotPoint(Snapshot, numberOfCharactersSoFar);
            }
        }

        public int Length
        {
            get { return text.Length; }
        }

        public int LengthIncludingLineBreak
        {
            get { throw new NotImplementedException(); }
        }

        public SnapshotPoint End
        {
            get { throw new NotImplementedException(); }
        }

        public SnapshotPoint EndIncludingLineBreak
        {
            get { throw new NotImplementedException(); }
        }

        public int LineBreakLength
        {
            get { throw new NotImplementedException(); }
        }
    }
}