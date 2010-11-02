using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Gherkin;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using NBehave.Narrator.Framework;

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
    public class GherkinFileEditorParser : IListener
    {
        private ITextBuffer buffer;
        private Subject<ParserEvent> _parserEvents;

        public IObservable<ParserEvent> ParserEvents
        {
            get { return _parserEvents; }
        }

        public void InitialiseWithBuffer(ITextBuffer textBuffer)
        {
            _parserEvents = new Subject<ParserEvent>();

            buffer = textBuffer;
            this.buffer.Changed += PartialParse;
        }

        public void FirstParse()
        {
            var languageService = new LanguageService();

            ILexer lexer = languageService.GetLexer(buffer.CurrentSnapshot.GetText(), this);
            lexer.Scan(new StringReader(buffer.CurrentSnapshot.GetText()));
        }

        private void PartialParse(object sender, TextContentChangedEventArgs e)
        {
        }

        public void Feature(Token keyword, Token title)
        {
            ITextSnapshotLine textSnapshotLine = buffer.CurrentSnapshot.GetLineFromLineNumber(keyword.Position.Line - 1);
            string lineFromLineNumber = textSnapshotLine.GetText();
            var keywordMatches = new Regex("^\\s*" + keyword.Content).Match(lineFromLineNumber);
            Span KeywordSpan = new Span(textSnapshotLine.Start.Position + keywordMatches.Captures[0].Index, keyword.Content.Length);

            var titleMatches = new Regex(":").Match(lineFromLineNumber);
            Span titleSpan = new Span(textSnapshotLine.Start.Position + titleMatches.Captures[0].Index + 1, lineFromLineNumber.Substring(titleMatches.Captures[0].Index + 1).Length);            




            _parserEvents.OnNext(new ParserEvent(ParserEventType.Feature)
            {
                Keyword = keyword.Content,
                Title = title.Content,
                Line = keyword.Position.Line,
                KeywordSpan = KeywordSpan,
                TitleSpan = titleSpan
            });
        }

        public void Scenario(Token keyword, Token name)
        {
            _parserEvents.OnNext(new ParserEvent(ParserEventType.Scenario)
            {
                Keyword = keyword.Content,
                Title = name.Content,
                Line = keyword.Position.Line
            });
        }

        public void Examples(Token keyword, Token name)
        {
            _parserEvents.OnNext(new ParserEvent(ParserEventType.Examples)
            {
                Keyword = keyword.Content,
                Name = name.Content,
                Line = keyword.Position.Line
            });
        }

        public void Step(Token keyword, Token text, StepKind stepKind)
        {
            _parserEvents.OnNext(new ParserEvent(ParserEventType.Step)
            {
                Keyword = keyword.Content,
                Text = text.Content,
                Line = keyword.Position.Line
            });
        }

        public void Table(IList<IList<Token>> rows, Position tablePosition)
        {
            
        }

        public void row(ArrayList list, int line)
        {
            _parserEvents.OnNext(new ParserEvent(ParserEventType.Row)
            {
                List = list.ToArray().Cast<string>(),
                Line = line
            });
        }

        public void Background(Token keyword, Token name)
        {
            _parserEvents.OnNext(new ParserEvent(ParserEventType.Background)
            {
                Keyword = keyword.Content,
                Name = name.Content,
                Line = keyword.Position.Line
            });
        }

        public void ScenarioOutline(Token keyword, Token name)
        {
            _parserEvents.OnNext(new ParserEvent(ParserEventType.ScenarioOutline)
            {
                Keyword = keyword.Content,
                Name = name.Content,
                Line = keyword.Position.Line
            });
        }

        public void Comment(Token comment)
        {
            _parserEvents.OnNext(new ParserEvent(ParserEventType.Comment)
            {
                Comment = comment.Content,
                Line = comment.Position.Line
            });
        }

        public void Tag(Token name)
        {
            _parserEvents.OnNext(new ParserEvent(ParserEventType.Tag)
            {
                Name = name.Content,
                Line = name.Position.Line
            });
        }

        public void PythonString(Token content)
        {
            _parserEvents.OnNext(new ParserEvent(ParserEventType.PyString)
            {
                Content = content.Content,
                Line = content.Position.Line
            });
        }

        public void SyntaxError(string state, string @event, IEnumerable<string> legalEvents, Position position)
        {
        }

        public void eof()
        {
            _parserEvents.OnNext(new ParserEvent(ParserEventType.Eof)
            {
                Eof = true
            });
        }
    }
}