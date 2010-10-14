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

            var matches = new Regex("^\\s*Feature").Match(lineFromLineNumber);

            Span span = new Span(textSnapshotLine.Start.Position + matches.Captures[0].Index, keyword.Length);            

            _parserEvents.OnNext(new ParserEvent(ParserEventType.Feature)
            {
                Keyword = keyword,
                Title = title,
                Description = description,
                Line = line,
                Span = span
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

    public enum ParserEventType
    {
        Feature,
        Scenario,
        Examples,
        Step,
        Row,
        Background,
        ScenarioOutline,
        Comment,
        Tag,
        PyString,
        Eof
    }

    public class ParserEvent
    {
        public ParserEvent(ParserEventType eventType)
        {
            EventType = eventType;
        }

        public ParserEventType EventType { get; set; }

        public string Keyword { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public int Line { get; set; }

        public string Name { get; set; }

        public string Text { get; set; }

        public IEnumerable<string> List { get; set; }

        public string Comment { get; set; }

        public string Content { get; set; }

        public bool Eof { get; set; }

        public Span Span { get; set; }
    }
}