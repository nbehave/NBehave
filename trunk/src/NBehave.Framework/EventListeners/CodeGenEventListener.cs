using System;
using System.IO;
using System.Linq;
using System.Text;

namespace NBehave.Narrator.Framework.EventListeners
{
    public class CodeGenEventListener : IEventListener
    {
        private readonly ActionStepCodeGenerator _actionStepCodeGenerator;
        private readonly TextWriter _writer;
        private readonly TextWriter _bufferWriter;
        private bool _isFirstPendingStep = true;

        public CodeGenEventListener(TextWriter writer)
        {
            _writer = writer;
            _bufferWriter = new StringWriter(new StringBuilder());
            _actionStepCodeGenerator = new ActionStepCodeGenerator();
        }
        void IEventListener.FeatureCreated(string feature)
        {
        }

        void IEventListener.FeatureNarrative(string message)
        {
        }

        void IEventListener.ScenarioCreated(string scenarioTitle)
        {
        }

        void IEventListener.RunStarted()
        {
        }

        void IEventListener.RunFinished()
        {
            if (_isFirstPendingStep == false)
            {
                _bufferWriter.Flush();
                _writer.Write(_bufferWriter.ToString());
            }
            _writer.Flush();
        }

        void IEventListener.ThemeStarted(string name)
        {
        }

        void IEventListener.ThemeFinished()
        {
        }

        void IEventListener.ScenarioResult(ScenarioResult result)
        {
            var lastStep = TypeOfStep.Given;
            var validNames = Enum.GetNames(typeof(TypeOfStep)).ToList();
            foreach (var actionStepResult in result.ActionStepResults)
            {
                if (validNames.Contains(actionStepResult.StringStep.GetFirstWord()))
                    lastStep = (TypeOfStep)Enum.Parse(typeof(TypeOfStep), actionStepResult.StringStep.GetFirstWord(), true);
                if (actionStepResult.Result is Pending)
                {
                    if (_isFirstPendingStep)
                    {
                        WriteStart();
                        _isFirstPendingStep = false;
                    }
                    var code = _actionStepCodeGenerator.GenerateMethodFor(actionStepResult.StringStep, lastStep);
                    _bufferWriter.WriteLine("");
                    _bufferWriter.WriteLine(code);
                }
            }
        }

        private void WriteStart()
        {
            _bufferWriter.WriteLine("");
            _bufferWriter.WriteLine("You could implement pending steps with these snippets:");
        }
    }
}