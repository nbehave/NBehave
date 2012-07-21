using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.Text;
using NBehave.Gherkin;
using NBehave.VS2010.Plugin.Editor.Domain;

namespace NBehave.VS2010.Plugin.Editor
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
                Observable.FromEvent<TextContentChangedEventArgs>(
                    handler => textBuffer.Changed += handler,
                    handler => textBuffer.Changed -= handler);

            _inputListener = fromEvent
                .Sample(TimeSpan.FromSeconds(1))
                .Select(event1 => event1.EventArgs.After)
                .Subscribe(Parse);
        }

        private void Parse(ITextSnapshot snapshot)
        {
            _isParsing.OnNext(true);
            _snapshot = snapshot;

            try
            {
                var parser = new Parser(this);
                parser.Scan(new StringReader(snapshot.GetText()));
            }
            catch (Exception) { }
            finally
            {
                _isParsing.OnNext(false);
            }
        }

        public void FirstParse()
        {
            Parse(_snapshot);
        }

        public void Feature(Token keyword, Token title, Token narrative)
        {
            string featureTitle = title.Content;
            string description = narrative.Content;

            _parserEvents.OnNext(new ParserEvent(ParserEventType.Feature)
            {
                Keyword = keyword.Content,
                Title = featureTitle,
                Description = description,
                Line = keyword.LineInFile.Line,
                Snapshot = _snapshot
            });
        }

        public void Scenario(Token keyword, Token name)
        {
            _parserEvents.OnNext(new ParserEvent(ParserEventType.Scenario)
            {
                Keyword = keyword.Content,
                Title = name.Content,
                Line = keyword.LineInFile.Line,
                Snapshot = _snapshot
            });
        }

        public void Examples(Token keyword, Token name)
        {
            OnNext(keyword, name, ParserEventType.Examples);
        }

        public void Step(Token keyword, Token name)
        {
            OnNext(keyword, name, ParserEventType.Step);
        }

        public void Table(IList<IList<Token>> rows, LineInFile tablePosition)
        {
            _parserEvents.OnNext(new ParserEvent(ParserEventType.Table)
            {
                TableColumns = rows.First().Select(token => token.Content),
                Line = tablePosition.Line,
                RowCount = rows.Count - 1,
                Snapshot = _snapshot
            });
        }

        public void Background(Token keyword, Token name)
        {
            OnNext(keyword, name, ParserEventType.Background);
        }

        public void ScenarioOutline(Token keyword, Token name)
        {
            OnNext(keyword, name, ParserEventType.ScenarioOutline);
        }

        public void Comment(Token content)
        {
            OnNext(content, ParserEventType.Comment);
        }

        public void Tag(Token content)
        {
            OnNext(content, ParserEventType.Tag);
        }

        public void SyntaxError(string state, string @event, IEnumerable<string> legalEvents, LineInFile lineInFile)
        {
        }

        public void Eof()
        { }

        public void DocString(Token docString)
        { }


        private void OnNext(Token content, ParserEventType parserEventType)
        {
            _parserEvents.OnNext(new ParserEvent(parserEventType)
            {
                Name = content.Content,
                Line = content.LineInFile.Line,
                Snapshot = _snapshot
            });
        }

        private void OnNext(Token keyword, Token name, ParserEventType parserEventType)
        {
            _parserEvents.OnNext(new ParserEvent(parserEventType)
            {
                Keyword = keyword.Content,
                Name = name.Content,
                Line = keyword.LineInFile.Line,
                Snapshot = _snapshot
            });
        }
        public void Dispose()
        {
            _inputListener.Dispose();
        }
    }
}