using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace NBehave.Narrator.Framework.EventListeners
{
    public class CodeGenEventListener : EventListener
    {
        private readonly ActionStepCodeGenerator _actionStepCodeGenerator;
        private readonly TextWriter _writer;
        private readonly TextWriter _bufferWriter;
        private bool _firstPendingStep = true;
        private readonly List<CodeGenStep> _pendingSteps = new List<CodeGenStep>();

        public CodeGenEventListener()
            : this(new StringWriter(new StringBuilder()))
        { }

        public CodeGenEventListener(TextWriter writer)
        {
            _writer = writer;
            _bufferWriter = new StringWriter(new StringBuilder());
            _actionStepCodeGenerator = new ActionStepCodeGenerator();
        }

        public IEnumerable<CodeGenStep> PendingSteps { get { return _pendingSteps; } }

        public override void RunFinished()
        {
            if (_firstPendingStep == false)
            {
                _bufferWriter.Flush();
                _writer.Write(_bufferWriter.ToString());
            }

            _writer.Flush();
        }

        public override void ScenarioResult(ScenarioResult result)
        {
            var lastStep = TypeOfStep.Given;
            var validNames = Enum.GetNames(typeof(TypeOfStep)).ToList();
            foreach (var actionStepResult in result.StepResults)
            {
                lastStep = DetermineTypeOfStep(validNames, actionStepResult, lastStep);
                if (actionStepResult.Result is Pending)
                {
                    if (_firstPendingStep)
                    {
                        WriteStart();
                        _firstPendingStep = false;
                    }

                    var code = _actionStepCodeGenerator.GenerateMethodFor(actionStepResult.StringStep, lastStep);
                    _bufferWriter.WriteLine(string.Empty);
                    _bufferWriter.WriteLine(code);
                    _pendingSteps.Add(new CodeGenStep(result.FeatureTitle, result.ScenarioTitle, actionStepResult.StringStep, code));
                }
            }
        }

        private TypeOfStep DetermineTypeOfStep(List<string> validNames, StepResult stepResult, TypeOfStep lastStep)
        {
            if (validNames.Contains(stepResult.StringStep.GetFirstWord()))
            {
                lastStep = (TypeOfStep)Enum.Parse(typeof(TypeOfStep), stepResult.StringStep.GetFirstWord(), true);
            }
            return lastStep;
        }

        private void WriteStart()
        {
            _bufferWriter.WriteLine(string.Empty);
            _bufferWriter.WriteLine("You could implement pending steps with these snippets:");
        }

        public override string ToString()
        {
            _writer.Flush();
            return _writer.ToString();
        }
    }

    public class CodeGenStep
    {
        public string Feature { get; private set; }
        public string Scenario { get; private set; }
        public string Step { get; private set; }
        public string Code { get; private set; }

        public CodeGenStep(string feature, string scenario, string step, string code)
        {
            Feature = feature;
            Scenario = scenario;
            Step = step;
            Code = code;
        }
    }
}