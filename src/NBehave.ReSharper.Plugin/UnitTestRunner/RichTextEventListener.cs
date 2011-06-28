using System;
using System.Collections.Generic;
using System.Drawing;
using JetBrains.UI.RichText;
using NBehave.Narrator.Framework;

namespace NBehave.ReSharper.Plugin.UnitTestRunner
{
    public class RichTextEventListener : EventListener
    {
        private readonly RichText _writer;
        private readonly List<ScenarioResult> _allResults = new List<ScenarioResult>();
        private readonly TextStyle _normalTextStyle = new TextStyle(FontStyle.Regular, Color.Black, Color.White);

        public RichTextEventListener(RichText writer)
        {
            _writer = writer;
        }

        public override void FeatureCreated(string feature)
        {
            _writer.Append(new RichText("Feature: ", new TextStyle(FontStyle.Bold, Color.Blue, Color.Wheat)));
            _writer.Append(feature + Environment.NewLine, new TextStyle(FontStyle.Bold, Color.Black, Color.White));
        }

        public override void FeatureNarrative(string message)
        {
            _writer.Append("    " + message + Environment.NewLine, _normalTextStyle);
        }

        public override void ScenarioCreated(string scenarioTitle)
        {
            _writer.Append(new RichText("Scenario: ", new TextStyle(FontStyle.Bold, Color.Blue, Color.Wheat)));
            _writer.Append(scenarioTitle + Environment.NewLine, new TextStyle(FontStyle.Bold, Color.Black, Color.White));
        }

        public override void RunFinished()
        {
            WriteSummary();
        }

        public override void ScenarioResult(ScenarioResult result)
        {
            _allResults.Add(result);
            foreach (var actionStepResult in result.ActionStepResults)
            {
                var msg = (actionStepResult.Result is Passed) ? string.Empty : " - " + actionStepResult.Result.ToString().ToUpper();
                var color = (actionStepResult.Result is Passed) ? Color.Green : Color.Yellow;
                color = (actionStepResult.Result is Pending) ? Color.Gray : color;
                color = (actionStepResult.Result is Failed) ? Color.Red: color;
                _writer.Append(actionStepResult.StringStep + Environment.NewLine, new TextStyle(FontStyle.Regular, color, Color.White));
            }
        }

        private void WriteSummary()
        {
            var summaryWriter = new SummaryWriter(Console.Out);
            summaryWriter.WriteCompleteSummary(_allResults);
        }
    }
}