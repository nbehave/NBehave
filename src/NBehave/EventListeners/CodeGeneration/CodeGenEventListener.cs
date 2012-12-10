using System.Collections.Generic;
using System.IO;
using System.Text;
using NBehave.Domain;

namespace NBehave.EventListeners.CodeGeneration
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

        public override void ScenarioFinished(ScenarioResult result)
        {
            foreach (var actionStepResult in result.StepResults)
            {
                if (actionStepResult.Result is PendingNotImplemented)
                {
                    if (_firstPendingStep)
                    {
                        WriteStart();
                        _firstPendingStep = false;
                    }

                    var code = _actionStepCodeGenerator.GenerateMethodFor(actionStepResult.StringStep);
                    _bufferWriter.WriteLine(string.Empty);
                    _bufferWriter.WriteLine(code);
                    _pendingSteps.Add(new CodeGenStep(result.FeatureTitle, result.ScenarioTitle, actionStepResult.StringStep, code));
                }
            }
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

        public CodeGenStep(string feature, string scenario, StringStep step, string code)
        {
            Feature = feature;
            Scenario = scenario;
            Step = step.Step;
            Code = code;
        }
    }
}