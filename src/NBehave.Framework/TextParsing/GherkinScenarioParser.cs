// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GherkinScenarioParser.cs" company="NBehave">
//   Copyright (c) 2007, NBehave - http://nbehave.codeplex.com/license
// </copyright>
// <summary>
//   Defines the GherkinScenarioParser type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace NBehave.Narrator.Framework
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Gherkin;

    using NBehave.Narrator.Framework.Tiny;

    public class GherkinScenarioParser : IListener
    {
        private readonly ITinyMessengerHub _hub;
        private readonly LanguageService _languageService;

        public GherkinScenarioParser(ITinyMessengerHub hub)
        {
            _hub = hub;
            _languageService = new LanguageService();
        }

        public void Parse(string file)
        {
            _hub.Publish(new ModelBuilderInitialise(this));
            _hub.Publish(new ParsingFileStart(this, file));

            using (Stream stream = File.OpenRead(file))
            {
                var reader = new StreamReader(stream);
                var scenarioText = reader.ReadToEnd();

                // We write a new stream just to remove \r
                var ms = new MemoryStream();
                var sr = new StreamWriter(ms);
                sr.Write(scenarioText.Replace("\r", string.Empty));
                sr.Flush();
                ms.Seek(0, SeekOrigin.Begin);

                var lexer = _languageService.GetLexer(scenarioText, this);
                try
                {
                    lexer.Scan(new StreamReader(ms));
                }
                catch (LexingException ex)
                {
                    throw new LexingException("Error lexing file " + file, ex);
                }
            }

            _hub.Publish(new ParsingFileEnd(this, file));
            _hub.Publish(new ModelBuilderCleanup(this));
        }

        public void Feature(Token keyword, Token title)
        {
            var titleAndNarrative = title.Content;
            _hub.Publish(new ParsedFeature(this, titleAndNarrative));
        }

        public void Scenario(Token keyword, Token title)
        {
            var scenarioTitle = title.Content;
            _hub.Publish(new ParsedScenario(this, scenarioTitle));
        }

        public void Examples(Token keyword, Token name)
        {
            _hub.Publish(new EnteringExamples(this));
        }

        public void Step(Token keyword, Token name, StepKind stepKind)
        {
            string stepText = string.Format("{0}{1}", keyword.Content, name.Content);
            _hub.Publish(new ParsedStep(this, stepText));
        }

        public void Table(IList<IList<Token>> rows, Position tablePosition)
        {
            _hub.Publish(new ParsedTable(this, rows));
        }

        public void Background(Token keyword, Token name)
        {
            string backgroundTitle = name.Content;
            _hub.Publish(new ParsedBackground(this, backgroundTitle));
        }

        public void ScenarioOutline(Token keyword, Token name)
        {
            string scenarioOutlineTitle = name.Content;
            _hub.Publish(new ParsedScenarioOutline(this, scenarioOutlineTitle));
        }

        public void Comment(Token comment)
        {
        }

        public void Tag(Token name)
        {
        }

        public void PythonString(Token content)
        {
        }

        public void SyntaxError(string state, string @event, IEnumerable<string> legalEvents, Position position)
        {
        }

        public void Eof()
        {
        }
    }
}