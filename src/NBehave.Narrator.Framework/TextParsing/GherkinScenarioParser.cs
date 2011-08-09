// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GherkinScenarioParser.cs" company="NBehave">
//   Copyright (c) 2007, NBehave - http://nbehave.codeplex.com/license
// </copyright>
// <summary>
//   Defines the GherkinScenarioParser type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using NBehave.Gherkin;
using NBehave.Narrator.Framework.Tiny;

namespace NBehave.Narrator.Framework
{
    public class GherkinScenarioParser : IListener
    {
        private readonly ITinyMessengerHub _hub;

        public GherkinScenarioParser(ITinyMessengerHub hub)
        {
            _hub = hub;
        }

        public void Parse(string file)
        {
            _hub.Publish(new ModelBuilderInitialise(this));
            _hub.Publish(new ParsingFileStart(this, file));

            var content = File.ReadAllText(file);
            var gherkinParser = new Parser(this);
            gherkinParser.Scan(content);
            _hub.Publish(new ParsingFileEnd(this, file));
            _hub.Publish(new ModelBuilderCleanup(this));
        }

        public void Feature(Token keyword, Token title, Token narrative)
        {
            var titleAndNarrative = title.Content + Environment.NewLine + narrative.Content;
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

        public void Step(Token keyword, Token name)
        {
            string stepText = string.Format("{0}{1}", keyword.Content, name.Content);
            _hub.Publish(new ParsedStep(this, stepText));
        }

        public void Table(IList<IList<Token>> columns, LineInFile tableRow)
        {
            _hub.Publish(new ParsedTable(this, columns));
        }

        public void Background(Token keyword, Token name)
        {
            string backgroundTitle = name.Content;
            _hub.Publish(new ParsedBackground(this, backgroundTitle));
        }

        public void Comment(Token comment)
        {
        }

        public void Tag(Token name)
        {
        }

        public void SyntaxError(string state, string @event, IEnumerable<string> legalEvents, LineInFile lineInFile)
        {
        }

        public void Eof()
        {
        }
    }
}