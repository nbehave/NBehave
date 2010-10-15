using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text.RegularExpressions;
using gherkin.lexer;
using java.util;
using Microsoft.VisualStudio.Text;
using NBehave.Narrator.Framework;
using Observable = System.Linq.Observable;

namespace NBehave.VS2010.Plugin.GherkinFileEditor
{
    [Export(typeof(GherkinFileEditorParserFactory))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class GherkinFileEditorParserFactory
    {
        [Import]
        public GherkinFileEditorParser GherkinFileEditorParser { get; set; }

        internal GherkinFileEditorParser CreateParser(ITextBuffer buffer)
        {
            GherkinFileEditorParser.InitialiseWithBuffer(buffer);
            return buffer.Properties.GetOrCreateSingletonProperty(() => GherkinFileEditorParser);
        }
    }

    [Export(typeof(GherkinFileEditorParser))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class GherkinFileEditorParser : Listener, IDisposable
    {
        private Subject<ParserEvent> _parserEvents;
        private IDisposable _inputListener;
        private ITextSnapshot _snapshot;

        public IObservable<ParserEvent> ParserEvents
        {
            get { return _parserEvents; }
        }

        public void InitialiseWithBuffer(ITextBuffer textBuffer)
        {
            _parserEvents = new Subject<ParserEvent>();
            _snapshot = textBuffer.CurrentSnapshot;

            IObservable<IEvent<TextContentChangedEventArgs>> fromEvent = 
                Observable.FromEvent((EventHandler<TextContentChangedEventArgs> ev) => 
                    new EventHandler<TextContentChangedEventArgs>(ev),
                    handler => textBuffer.Changed += handler,
                    handler => textBuffer.Changed -= handler);

            _inputListener = fromEvent
                .Sample(TimeSpan.FromSeconds(2))
                .Select(event1 => event1.EventArgs.After)
                .Subscribe(Parse);
        }

        private void Parse(ITextSnapshot snapshot)
        {
            _snapshot = snapshot;

            try
            {
                var languageService = new LanguageService();

                Lexer lexer = languageService.GetLexer(snapshot.GetText(), this);
                lexer.scan(snapshot.GetText());
            }
            catch (Exception) { }
        }

        public void FirstParse()
        {
            Parse(_snapshot);
        }

        public void feature(string keyword, string title, string description, int line)
        {
            _parserEvents.OnNext(new ParserEvent(ParserEventType.Feature)
            {
                Keyword = keyword,
                Title = title,
                Description = description,
                Line = line,
                Snapshot = _snapshot
            });
        }

        public void scenario(string keyword, string title, string description, int line)
        {
            _parserEvents.OnNext(new ParserEvent(ParserEventType.Scenario)
            {
                Keyword = keyword,
                Title = title,
                Description = description,
                Line = line,
                Snapshot = _snapshot
            });
        }

        public void examples(string keyword, string name, string description, int line)
        {
            _parserEvents.OnNext(new ParserEvent(ParserEventType.Examples)
            {
                Keyword = keyword,
                Name = name,
                Description = description,
                Line = line,
                Snapshot = _snapshot
            });
        }

        public void step(string keyword, string text, int line)
        {
            _parserEvents.OnNext(new ParserEvent(ParserEventType.Step)
            {
                Keyword = keyword,
                Text = text,
                Line = line,
                Snapshot = _snapshot
            });
        }

        public void row(List list, int line)
        {
            _parserEvents.OnNext(new ParserEvent(ParserEventType.Row)
            {
                List = list.toArray().Cast<string>(),
                Line = line,
                Snapshot = _snapshot
            });
        }

        public void background(string keyword, string name, string description, int line)
        {
            _parserEvents.OnNext(new ParserEvent(ParserEventType.Background)
            {
                Keyword = keyword,
                Name = name,
                Description = description,
                Line = line,
                Snapshot = _snapshot
            });
        }

        public void scenarioOutline(string keyword, string name, string description, int line)
        {
            _parserEvents.OnNext(new ParserEvent(ParserEventType.ScenarioOutline)
            {
                Keyword = keyword,
                Name = name,
                Description = description,
                Line = line,
                Snapshot = _snapshot
            });
        }

        public void comment(string comment, int line)
        {
            _parserEvents.OnNext(new ParserEvent(ParserEventType.Comment)
            {
                Comment = comment,
                Line = line,
                Snapshot = _snapshot
            });
        }

        public void tag(string name, int line)
        {
            _parserEvents.OnNext(new ParserEvent(ParserEventType.Tag)
            {
                Name = name,
                Line = line,
                Snapshot = _snapshot
            });
        }

        public void pyString(string content, int line)
        {
            _parserEvents.OnNext(new ParserEvent(ParserEventType.PyString)
            {
                Content = content,
                Line = line,
                Snapshot = _snapshot
            });
        }

        public void eof()
        {
            _parserEvents.OnNext(new ParserEvent(ParserEventType.Eof));
        }

        public void Dispose()
        {
            _inputListener.Dispose();
        }
    }
}