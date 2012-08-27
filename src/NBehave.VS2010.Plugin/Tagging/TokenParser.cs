using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Text;
using NBehave.Gherkin;

namespace NBehave.VS2010.Plugin.Tagging
{
    public class TokenParser : IDisposable
    {
        private readonly GherkinEventListener gherkinEventListener = new GherkinEventListener();
        private List<GherkinParseEvent> events = new List<GherkinParseEvent>();
        private string lastParsedContent = "";
        private readonly object lockObj = new object();
        private readonly ManualResetEvent isParsing = new ManualResetEvent(true);
        private readonly Queue<ITextSnapshot> nextTextSnapshotToParse = new Queue<ITextSnapshot>();
        private readonly ITextBuffer buffer;
        private bool disposed;

        public event EventHandler<TokenParserEventArgs> TokenParserEvent;

        public TokenParser(ITextBuffer buffer)
        {
            this.buffer = buffer;
            nextTextSnapshotToParse.Enqueue(buffer.CurrentSnapshot);
            buffer.Changed += BufferChanged;
            TokenParserEvent += (s, e) => { };
            Parallel.Invoke(DoParse);
        }

        public IEnumerable<GherkinParseEvent> Events
        {
            get
            {
                return events;
            }
        }

        private void BufferChanged(object sender, TextContentChangedEventArgs e)
        {
            lock (lockObj)
                nextTextSnapshotToParse.Enqueue(e.After);
            Parallel.Invoke(DoParse);
        }

        private void DoParse()
        {
            if (!isParsing.WaitOne(0) || !nextTextSnapshotToParse.Any())
                return;
            isParsing.Reset();

            string content = string.Empty;
            try
            {
                Console.WriteLine("--> snapshots? " + nextTextSnapshotToParse.Any());
                while (nextTextSnapshotToParse.Any())
                {
                    var textSnapshot = GetTextSnapshotToParse();
                    content = textSnapshot.GetText(0, textSnapshot.Length);
                    if (content == lastParsedContent)
                        return;
                    var oldEvents = Events;
                    var newEvents = Parse(content);
                    lock (lockObj)
                    {
                        events = newEvents;
                    }
                    var newAndChanged = (newEvents.Any(_ => _.GherkinTokenType == GherkinTokenType.SyntaxError) || LinesAddedOrRemoved(newEvents, oldEvents))
                                            ? newEvents : newEvents.Except(oldEvents).ToList();
                    NotifyChanges(textSnapshot, newAndChanged);
                }
            }
            catch (Exception)
            { }
            finally
            {
                lastParsedContent = content;
                isParsing.Set();
            }
        }

        private ITextSnapshot GetTextSnapshotToParse()
        {
            ITextSnapshot textSnapshot = null;
            lock (nextTextSnapshotToParse)
            {
                while (nextTextSnapshotToParse.Any())
                    textSnapshot = nextTextSnapshotToParse.Dequeue();
            }
            return textSnapshot;
        }

        private bool LinesAddedOrRemoved(IEnumerable<GherkinParseEvent> newEvents, IEnumerable<GherkinParseEvent> oldEvents)
        {
            int a = (newEvents.Any()) ? newEvents.SelectMany(_ => _.Tokens).Max(_ => _.LineInFile.Line) : -1;
            int b = (oldEvents.Any()) ? oldEvents.SelectMany(_ => _.Tokens).Max(_ => _.LineInFile.Line) : -1;
            return a != b;
        }

        private void NotifyChanges(ITextSnapshot textSnapshot, IEnumerable<GherkinParseEvent> newAndChanged)
        {
            var linesChanged = newAndChanged
                .Where(_ => _.Tokens.Any())
                .Select(_ => _.Tokens[0].LineInFile.Line)
                .Distinct().ToArray();
            int from = linesChanged.Min();
            int to = linesChanged.Max();
            var previousEvent = new GherkinParseEvent(GherkinTokenType.Feature, new Token("", new LineInFile(0)));
            for (int i = from; i <= to; i++)
            {
                ITextSnapshotLine line = textSnapshot.GetLineFromLineNumber(i);
                var s = new SnapshotSpan(textSnapshot, line.Start, line.Length);
                GherkinParseEvent evt = newAndChanged.FirstOrDefault(_ => _.Tokens.Any() && _.Tokens[0].LineInFile.Line == i) ??
                    new GherkinParseEvent(previousEvent.GherkinTokenType, new Token("", new LineInFile(i)));
                TokenParserEvent.Invoke(this, new TokenParserEventArgs(evt, s));
                previousEvent = evt;
            }
        }

        private List<GherkinParseEvent> Parse(string content)
        {
            var newEvents = new List<GherkinParseEvent>();
            gherkinEventListener.Events.Clear();
            var parser = new Parser(gherkinEventListener);
            try
            {
                parser.Scan(content);
            }
            catch (Exception e)
            {
                var match = new Regex(@"^Line: (?<lineNumber>\d+). (\w+\s*)+ '(?<lineText>.*)'$").Match(e.Message);
                if (match.Success && (e is ParseException))
                {
                    var line = int.Parse(match.Groups["lineNumber"].Value);
                    var lineInFile = new LineInFile(line - 1);
                    var text = match.Groups["lineText"].Value;
                    var token = new Token(text, lineInFile);
                    var error = new Token(e.Message, lineInFile);
                    newEvents.Add(new GherkinParseEvent(GherkinTokenType.SyntaxError, token, error));
                }
            }
            finally
            {
                newEvents.AddRange(ToZeroBasedLines().ToList());
            }
            return newEvents;
        }

        private IEnumerable<GherkinParseEvent> ToZeroBasedLines()
        {
            return gherkinEventListener.Events
                .Select(_ => new GherkinParseEvent(_.GherkinTokenType, _.Tokens.Select(t => new Token(t.Content, new LineInFile(t.LineInFile.Line - 1))).ToArray()));
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            if (disposed)
                return;
            disposed = true;
            buffer.Changed -= BufferChanged;
        }
    }
}