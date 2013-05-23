using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Text;
using NBehave.Gherkin;
using NBehave.Narrator.Framework;
using NBehave.Narrator.Framework.TextParsing;
using NBehave.VS2010.Plugin.Editor.Glyphs;

namespace NBehave.VS2010.Plugin.Tagging
{
    public class TokenParser : IDisposable
    {
        private List<GherkinParseEvent> events = new List<GherkinParseEvent>();
        private List<Feature> features = new List<Feature>();
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

        public IEnumerable<GherkinParseEvent> Events { get { return events; } }
        public IEnumerable<Feature> Features { get { return features; } }
        public bool LastParseFailed()
        {
            return Events.Any(_ => _.GherkinTokenType == GherkinTokenType.SyntaxError);
        }

        public void ForceParse(ITextSnapshot snapshot)
        {
            isParsing.WaitOne(100);
            nextTextSnapshotToParse.Enqueue(snapshot);
            DoParse();
            NotifyChanges(snapshot, events);
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
                while (nextTextSnapshotToParse.Any())
                {
                    var textSnapshot = GetTextSnapshotToParse();
                    content = textSnapshot.GetText(0, textSnapshot.Length);
                    List<GherkinParseEvent> oldEvents;
                    Tuple<List<GherkinParseEvent>, List<Feature>> newEvents;
                    lock (lockObj)
                    {
                        oldEvents = events;
                        newEvents = new Tuple<List<GherkinParseEvent>, List<Feature>>(events, features);
                    }
                    if (content != lastParsedContent)
                        newEvents = Parse(content);
                    lock (lockObj)
                    {
                        events = newEvents.Item1;
                        features = newEvents.Item2;
                    }
                    var e = newEvents.Item1;
                    var newAndChanged = (e.Any(_ => _.GherkinTokenType == GherkinTokenType.SyntaxError) || LinesAddedOrRemoved(e, oldEvents))
                                            ? e : e.Except(oldEvents).ToList();
                    if (newAndChanged.Any())
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
            if (!linesChanged.Any())
                return;
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
            var lastLine = textSnapshot.GetLineFromLineNumber(to);
            TokenParserEvent.Invoke(this, new TokenParserEventArgs(new GherkinParseEvent(GherkinTokenType.Eof), new SnapshotSpan(textSnapshot, lastLine.Start, 0)));
        }

        private Tuple<List<GherkinParseEvent>, List<Feature>> Parse(string content)
        {
            var features = new List<Feature>();
            var nBehaveConfiguration = NBehaveConfiguration.New.DontIsolateInAppDomain().SetDryRun(true);
            var gherkinScenarioParser = new GherkinScenarioParser(nBehaveConfiguration);
            gherkinScenarioParser.FeatureEvent += (s, e) => features.Add(e.EventInfo);
            var gherkinEventListener = new GherkinEventListener();
            IListener listener = new CompositeGherkinListener(gherkinEventListener, gherkinScenarioParser);
            var newEvents = new List<GherkinParseEvent>();
            var parser = new Parser(listener);
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
                newEvents.AddRange(ToZeroBasedLines(gherkinEventListener).ToList());
            }
            return new Tuple<List<GherkinParseEvent>, List<Feature>>(newEvents, features);
        }

        private IEnumerable<GherkinParseEvent> ToZeroBasedLines(GherkinEventListener gherkinEventListener)
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