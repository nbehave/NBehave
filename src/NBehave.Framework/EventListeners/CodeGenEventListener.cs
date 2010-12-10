// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CodeGenEventListener.cs" company="NBehave">
//   Copyright (c) 2007, NBehave - http://nbehave.codeplex.com/license
// </copyright>
// <summary>
//   Defines the CodeGenEventListener type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace NBehave.Narrator.Framework.EventListeners
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;

    public class CodeGenEventListener : IEventListener
    {
        private readonly ActionStepCodeGenerator _actionStepCodeGenerator;
        private readonly TextWriter _writer;
        private readonly TextWriter _bufferWriter;
        private bool _firstPendingStep = true;

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
            if (this._firstPendingStep == false)
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
                lastStep = DetermineTypeOfStep(validNames, actionStepResult, lastStep);
                if (actionStepResult.Result is Pending)
                {
                    if (this._firstPendingStep)
                    {
                        WriteStart();
                        this._firstPendingStep = false;
                    }

                    var code = _actionStepCodeGenerator.GenerateMethodFor(actionStepResult.StringStep, lastStep);
                    _bufferWriter.WriteLine(string.Empty);
                    _bufferWriter.WriteLine(code);
                }
            }
        }

        private TypeOfStep DetermineTypeOfStep(List<string> validNames, ActionStepResult actionStepResult, TypeOfStep lastStep)
        {
            if (validNames.Contains(actionStepResult.StringStep.GetFirstWord()))
            {
                lastStep = (TypeOfStep)Enum.Parse(typeof(TypeOfStep), actionStepResult.StringStep.GetFirstWord(), true);
            }
            return lastStep;
        }

        private void WriteStart()
        {
            _bufferWriter.WriteLine(string.Empty);
            _bufferWriter.WriteLine("You could implement pending steps with these snippets:");
        }
    }
}