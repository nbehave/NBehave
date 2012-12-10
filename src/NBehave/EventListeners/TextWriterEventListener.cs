// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TextWriterEventListener.cs" company="NBehave">
//   Copyright (c) 2007, NBehave - http://nbehave.codeplex.com/license
// </copyright>
// <summary>
//   Defines the TextWriterEventListener type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using NBehave.Domain;

namespace NBehave.EventListeners
{
    public class TextWriterEventListener : EventListener, IDisposable
    {
        private readonly TextWriter writer;

        private readonly List<ScenarioResult> allResults = new List<ScenarioResult>();

        private bool disposed;

        public TextWriterEventListener(TextWriter writer)
        {
            this.writer = writer;
        }

        void IDisposable.Dispose()
        {
            Dispose(true);
        }

        public override void FeatureStarted(Feature feature)
        {
            writer.WriteLine("Feature: {0}", feature.Title);
            writer.WriteLine(feature.Narrative);
        }

        public override void ScenarioStarted(string scenarioTitle)
        {
            writer.WriteLine("Scenario: {0}", scenarioTitle);
        }

        public override void RunFinished()
        {
            WriteSummary();
            writer.Flush();
        }

        public override void ScenarioFinished(ScenarioResult result)
        {
            allResults.Add(result);
            foreach (var actionStepResult in result.StepResults)
            {
                var msg = (actionStepResult.Result is Passed) ? string.Empty : " - " + actionStepResult.Result.ToString().ToUpper();
                writer.WriteLine(actionStepResult.StringStep + msg);
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }

            if (disposing)
            {
                if (writer != null)
                {
                    writer.Flush();
                    writer.Close();
                }
            }

            disposed = true;
        }

        private void WriteSummary()
        {
            var summaryWriter = new SummaryWriter(writer);
            summaryWriter.WriteCompleteSummary(allResults);
        }
    }
}