using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using NBehave.Narrator.Framework;
using NBehave.Narrator.Framework.Internal;

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
    public class GherkinFileEditorParser : IDisposable
    {
        private Subject<Feature> parserEvents;
        private IDisposable inputListener;
        private ITextSnapshot snapshot;
        private Subject<bool> isParsing;

        public IObservable<Feature> ParserEvents
        {
            get { return parserEvents; }
        }

        public IObservable<bool> IsParsing
        {
            get { return isParsing; }
        }

        public void InitialiseWithBuffer(ITextBuffer textBuffer)
        {
            parserEvents = new Subject<Feature>();
            isParsing = new Subject<bool>();

            snapshot = textBuffer.CurrentSnapshot;

            IObservable<IEvent<TextContentChangedEventArgs>> fromEvent =
                Observable.FromEvent<TextContentChangedEventArgs>(
                    handler => textBuffer.Changed += handler,
                    handler => textBuffer.Changed -= handler);

            inputListener = fromEvent
                .Sample(TimeSpan.FromSeconds(1))
                .Select(event1 => event1.EventArgs.After)
                .Subscribe(Parse);
        }

        public List<Feature> ParseSnapshot(ITextSnapshot textSnapshot)
        {
            var text = textSnapshot.GetText();
            var tempFile = text.ToTempFile();
            var config = NBehaveConfiguration.New.SetScenarioFiles(new[] {tempFile});
            var scenarioParser = new ParseScenarioFiles(config);
            var features = scenarioParser.LoadFiles(config.ScenarioFiles).ToList();
            return features;
        }

        public void FirstParse()
        {
            Parse(snapshot);
        }

        private void Parse(ITextSnapshot snapshot)
        {
            isParsing.OnNext(true);
            this.snapshot = snapshot;
            try
            {
                var features = ParseSnapshot(snapshot);
                features.ForEach(f => parserEvents.OnNext(f));
            }
            catch (Exception) { }
            finally
            {
                isParsing.OnNext(false);
            }
        }


        public void Dispose()
        {
            inputListener.Dispose();
        }
    }
}