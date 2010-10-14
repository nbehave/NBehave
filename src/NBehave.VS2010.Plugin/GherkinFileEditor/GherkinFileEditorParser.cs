using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text.RegularExpressions;
using gherkin.lexer;
using java.util;
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
    public class GherkinFileEditorParser : Listener
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

            Lexer lexer = languageService.GetLexer(buffer.CurrentSnapshot.GetText(), this);
            lexer.scan(buffer.CurrentSnapshot.GetText());
        }

        private void PartialParse(object sender, TextContentChangedEventArgs e)
        {
        }

        public void feature(string keyword, string title, string description, int line)
        {
            ITextSnapshotLine textSnapshotLine = buffer.CurrentSnapshot.GetLineFromLineNumber(line - 1);
            string lineFromLineNumber = textSnapshotLine.GetText();
            var keywordMatches = new Regex("^\\s*" + keyword).Match(lineFromLineNumber);
            Span KeywordSpan = new Span(textSnapshotLine.Start.Position + keywordMatches.Captures[0].Index, keyword.Length);

            var titleMatches = new Regex(":").Match(lineFromLineNumber);
            Span titleSpan = new Span(textSnapshotLine.Start.Position + titleMatches.Captures[0].Index + 1, lineFromLineNumber.Substring(titleMatches.Captures[0].Index + 1).Length);            




            _parserEvents.OnNext(new ParserEvent(ParserEventType.Feature)
            {
                Keyword = keyword,
                Title = title,
                Description = description,
                Line = line,
                KeywordSpan = KeywordSpan,
                TitleSpan = titleSpan
//                DescriptionSpan = titleSpan
            });
        }

        public void scenario(string keyword, string title, string description, int line)
        {
            _parserEvents.OnNext(new ParserEvent(ParserEventType.Scenario)
            {
                Keyword = keyword,
                Title = title,
                Description = description,
                Line = line
            });
        }

        public void examples(string keyword, string name, string description, int line)
        {
            _parserEvents.OnNext(new ParserEvent(ParserEventType.Examples)
            {
                Keyword = keyword,
                Name = name,
                Description = description,
                Line = line
            });
        }

        public void step(string keyword, string text, int line)
        {
            _parserEvents.OnNext(new ParserEvent(ParserEventType.Step)
            {
                Keyword = keyword,
                Text = text,
                Line = line
            });
        }

        public void row(List list, int line)
        {
            _parserEvents.OnNext(new ParserEvent(ParserEventType.Row)
            {
                List = list.toArray().Cast<string>(),
                Line = line
            });
        }

        public void background(string keyword, string name, string description, int line)
        {
            _parserEvents.OnNext(new ParserEvent(ParserEventType.Background)
            {
                Keyword = keyword,
                Name = name,
                Description = description,
                Line = line
            });
        }

        public void scenarioOutline(string keyword, string name, string description, int line)
        {
            _parserEvents.OnNext(new ParserEvent(ParserEventType.ScenarioOutline)
            {
                Keyword = keyword,
                Name = name,
                Description = description,
                Line = line
            });
        }

        public void comment(string comment, int line)
        {
            _parserEvents.OnNext(new ParserEvent(ParserEventType.Comment)
            {
                Comment = comment,
                Line = line
            });
        }

        public void tag(string name, int line)
        {
            _parserEvents.OnNext(new ParserEvent(ParserEventType.Tag)
            {
                Name = name,
                Line = line
            });
        }

        public void pyString(string content, int line)
        {
            _parserEvents.OnNext(new ParserEvent(ParserEventType.PyString)
            {
                Content = content,
                Line = line
            });
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