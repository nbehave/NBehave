using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using Gherkin;
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
    public class GherkinFileEditorParser : IListener, IDisposable
    {
        private Subject<ParserEvent> _parserEvents;

        private IDisposable _inputListener;

        private ITextSnapshot _snapshot;
        private Subject<bool> _isParsing;

        public IObservable<ParserEvent> ParserEvents
        {
            get { return _parserEvents; }
        }

        public IObservable<bool> IsParsing
        {
            get { return _isParsing; }
        }

        public void InitialiseWithBuffer(ITextBuffer textBuffer)
        {
            _parserEvents = new Subject<ParserEvent>();
            _isParsing = new Subject<bool>();

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
            _isParsing.OnNext(true);
            _snapshot = snapshot;

            try
            {
                var languageService = new LanguageService();
                ILexer lexer = languageService.GetLexer(snapshot.GetText(), this);
                lexer.Scan(new StringReader(snapshot.GetText()));
            }
            catch (LexingException) { }
            finally
            {
                _isParsing.OnNext(false);
            }
        }

        public void FirstParse()
        {
            Parse(_snapshot);
        }

        public void Feature(Token keyword, Token title)
        {
            string featureTitle, description;
            if (title.Content.Contains(Environment.NewLine))
            {
                var lineBreakPosn = title.Content.IndexOf(Environment.NewLine);
                featureTitle = title.Content.Substring(0, lineBreakPosn);
                description = title.Content.Substring(lineBreakPosn + Environment.NewLine.Length);
            }
            else
            {
                featureTitle = title.Content;
                description = String.Empty;
            }

            _parserEvents.OnNext(new ParserEvent(ParserEventType.Feature)
            {
                Keyword = keyword.Content,
                Title = featureTitle,
                Description = description,
                Line = keyword.Position.Line,
                Snapshot = _snapshot
            });
        }

        public void Scenario(Token keyword, Token name)
        {
            _parserEvents.OnNext(new ParserEvent(ParserEventType.Scenario)
            {
                Keyword = keyword.Content,
                Title = name.Content,
                Line = keyword.Position.Line,
                Snapshot = _snapshot
            });
        }


        public void Examples(Token keyword, Token name)
        {
            _parserEvents.OnNext(new ParserEvent(ParserEventType.Examples)
            {
                Keyword = keyword.Content,
                Name = name.Content,
                Line = keyword.Position.Line,
                Snapshot = _snapshot
            });
        }

        public void Step(Token keyword, Token name, StepKind stepKind)
        {
            _parserEvents.OnNext(new ParserEvent(ParserEventType.Step)
            {
                Keyword = keyword.Content,
                Text = name.Content,
                Line = keyword.Position.Line,
                Snapshot = _snapshot
            });
        }

        public void Table(IList<IList<Token>> rows, Position tablePosition)
        {
            _parserEvents.OnNext(new ParserEvent(ParserEventType.Row)
            {
                List = rows.Cast<string>(),
                Line = tablePosition.Line,
                Snapshot = _snapshot
            });
        }

        public void Background(Token keyword, Token name)
        {
            _parserEvents.OnNext(new ParserEvent(ParserEventType.Background)
            {
                Keyword = keyword.Content,
                Name = name.Content,
                Line = keyword.Position.Line,
                Snapshot = _snapshot
            });
        }

        public void ScenarioOutline(Token keyword, Token name)
        {
            _parserEvents.OnNext(new ParserEvent(ParserEventType.ScenarioOutline)
            {
                Keyword = keyword.Content,
                Name = name.Content,
                Line = keyword.Position.Line,
                Snapshot = _snapshot
            });
        }

        public void Comment(Token content)
        {
            _parserEvents.OnNext(new ParserEvent(ParserEventType.Comment)
            {
                Comment = content.Content,
                Line = content.Position.Line,
                Snapshot = _snapshot
            });
        }

        public void Tag(Token name)
        {
            _parserEvents.OnNext(new ParserEvent(ParserEventType.Tag)
            {
                Name = name.Content,
                Line = name.Position.Line,
                Snapshot = _snapshot
            });
        }

        public void PythonString(Token pyString)
        {
            _parserEvents.OnNext(new ParserEvent(ParserEventType.PyString)
            {
                Content = pyString.Content,
                Line = pyString.Position.Line,
                Snapshot = _snapshot
            });
        }

        public void SyntaxError(string state, string @event, IEnumerable<string> legalEvents, Position position)
        {
        }

        public void Dispose()
        {
            _inputListener.Dispose();
        }
    }
}