// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TextWriterEventListener.cs" company="NBehave">
//   Copyright (c) 2007, NBehave - http://nbehave.codeplex.com/license
// </copyright>
// <summary>
//   Defines the TextWriterEventListener type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace NBehave.Narrator.Framework.EventListeners
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    public class TextWriterEventListener : EventListener, IDisposable
    {
        private readonly TextWriter _writer;

        private readonly List<ScenarioResult> _allResults = new List<ScenarioResult>();

        private bool _disposed;

        public TextWriterEventListener(TextWriter writer)
        {
            _writer = writer;
        }

        void IDisposable.Dispose()
        {
            Dispose(true);
        }

        public override void FeatureStarted(string feature)
        {
            _writer.WriteLine("Feature: {0}", feature);
        }

        public override void FeatureNarrative(string message)
        {
            _writer.WriteLine(message);
        }

        public override void ScenarioStarted(string scenarioTitle)
        {
            _writer.WriteLine("Scenario: {0}", scenarioTitle);
        }

        public override void RunFinished()
        {
            WriteSummary();
            _writer.Flush();
        }

        public override void ScenarioFinished(ScenarioResult result)
        {
            _allResults.Add(result);
            foreach (var actionStepResult in result.StepResults)
            {
                var msg = (actionStepResult.Result is Passed) ? string.Empty : " - " + actionStepResult.Result.ToString().ToUpper();
                _writer.WriteLine(actionStepResult.StringStep + msg);
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

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
            var summaryWriter = new SummaryWriter(_writer);
            summaryWriter.WriteCompleteSummary(_allResults);
        }
    }
}