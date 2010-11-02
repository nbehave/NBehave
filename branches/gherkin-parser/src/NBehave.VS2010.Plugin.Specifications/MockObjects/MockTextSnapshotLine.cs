using System;
using System.Linq;
using Microsoft.VisualStudio.Text;

namespace NBehave.VS2010.Plugin.Specs
{
    public class MockTextSnapshotLine : ITextSnapshotLine
    {
        private readonly int _lineNumber;
        private readonly MockTextSnapshot _snapshot;
        private string _text;

        public MockTextSnapshotLine(string text, int lineNumber, MockTextSnapshot _snapshot)
        {
            _lineNumber = lineNumber;
            this._snapshot = _snapshot;
            _text = text.Split(new[] {Environment.NewLine}, StringSplitOptions.None)[lineNumber] + Environment.NewLine;
        }

        public string GetText()
        {
            return _text;
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
            get { return _snapshot; }
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
            get { return _lineNumber; }
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
            get { return _text.Length; }
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