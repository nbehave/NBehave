// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GherkinScenarioParser.cs" company="NBehave">
//   Copyright (c) 2007, NBehave - http://nbehave.codeplex.com/license
// </copyright>
// <summary>
//   Defines the GherkinScenarioParser type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NBehave.Gherkin;
using NBehave.Narrator.Framework.TextParsing;
using NBehave.Narrator.Framework.Tiny;

namespace NBehave.Narrator.Framework
{
    public class GherkinScenarioParser : IListener
    {
        private readonly ITinyMessengerHub _hub;
        private readonly Queue<GherkinEvent> _events = new Queue<GherkinEvent>();
        private readonly NBehaveConfiguration _configuration;

        public GherkinScenarioParser(ITinyMessengerHub hub, NBehaveConfiguration configuration)
        {
            _hub = hub;
            _configuration = configuration;
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
            _events.Enqueue(new FeatureEvent(_hub, titleAndNarrative));
        }

        public void Scenario(Token keyword, Token title)
        {
            var scenarioTitle = title.Content;
            _events.Enqueue(new ScenarioEvent(_hub, scenarioTitle));
        }

        public void Examples(Token keyword, Token name)
        {
            _events.Enqueue(new ExamplesEvent(_hub));
        }

        public void Step(Token keyword, Token name)
        {
            string stepText = string.Format("{0}{1}", keyword.Content, name.Content);
            _events.Enqueue(new StepEvent(_hub, stepText));
        }

        public void Table(IList<IList<Token>> columns, LineInFile tableRow)
        {
            _events.Enqueue(new TableEvent(_hub, columns));
        }

        public void Background(Token keyword, Token name)
        {
            string backgroundTitle = name.Content;
            _events.Enqueue(new BackgroundEvent(_hub, backgroundTitle));
        }

        public void Comment(Token comment)
        { }

        public void Tag(Token tag)
        {
            _events.Enqueue(new TagEvent(_hub, tag.Content));
        }

        public void SyntaxError(string state, string @event, IEnumerable<string> legalEvents, LineInFile lineInFile)
        {
        }

        public void Eof()
        {
            _events.Enqueue(new EofEvent());

            while (_events.Any())
            {
                var eventsToRaise = FilterByTag().ToList();
                foreach (var @event in eventsToRaise)
                    @event.RaiseEvent();
            }
        }

        private IEnumerable<GherkinEvent> FilterByTag()
        {
            var events = GroupEventsByTag.GroupByTag(_events);
            var eventsToRaise = new List<GherkinEvent>();
            while (events.Any())
            {
                var eventsToHandle = new Queue<GherkinEvent>(GetEventsForNextFeature(events).ToList());

                var tagsFilter = TagFilterBuilder.Build(_configuration.TagsFilter);
                var filteredEvents = tagsFilter.Filter(eventsToHandle).ToList();
                eventsToRaise.AddRange(filteredEvents);
            }
            return eventsToRaise;
        }

        private IEnumerable<GherkinEvent> GetEventsForNextFeature(Queue<GherkinEvent> events)
        {
            return GetEventsWhile(events, nextEvent => nextEvent is EofEvent || nextEvent is FeatureEvent);
        }

        private IEnumerable<GherkinEvent> GetEventsWhile(Queue<GherkinEvent> events, Predicate<GherkinEvent> continueIfEventIsNot)
        {
            if (events.Any() == false)
                yield break;
            GherkinEvent nextEvent;
            do
            {
                yield return events.Dequeue();
                nextEvent = (events.Any()) ? events.Peek() : null;
            } while (events.Any() && !(continueIfEventIsNot(nextEvent)));
        }
    }
}