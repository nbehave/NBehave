using System;
using System.Collections.Generic;
using System.IO;

namespace NBehave.Narrator.Framework.EventListeners
{
    public class TextWriterEventListener : IEventListener, IDisposable
    {
        private readonly TextWriter _writer;
        private bool _disposed;
        private readonly List<ScenarioResult> _allResults = new List<ScenarioResult>();

        public TextWriterEventListener(TextWriter writer)
        {
            _writer = writer;
        }

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            Dispose(true);
        }

        #endregion

        public void RunStarted()
        { }

        public void FeatureCreated(string feature)
        {
            _writer.WriteLine("Feature: {0}", feature);
        }

        public void FeatureNarrative(string message)
        {
            _writer.WriteLine(message);
        }

        public void ScenarioCreated(string scenarioTitle)
        {
            _writer.WriteLine("Scenario: {0}", scenarioTitle);
        }

        public void RunFinished()
        {
            WriteSummary();
            _writer.Flush();
        }

        public void ThemeStarted(string name)
        {
            if (string.IsNullOrEmpty(name) == false)
                _writer.WriteLine("Theme: {0}", name);
        }

        public void ThemeFinished()
        { }

        public void ScenarioResult(ScenarioResult result)
        {
            _allResults.Add(result);
            foreach (var actionStepResult in result.ActionStepResults)
            {
                var msg = (actionStepResult.Result is Passed) ? "" : " - " + actionStepResult.Result.ToString().ToUpper();
                _writer.WriteLine(actionStepResult.StringStep + msg);
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                if (_writer != null)
                {
                    _writer.Flush();
                    _writer.Close();
                }
            }

            _disposed = true;
        }

        private void WriteSummary()
        {
            var summaryWriter = new SummaryWriter(Console.Out);
            summaryWriter.WriteCompleteSummary(_allResults);
        }
    }
}