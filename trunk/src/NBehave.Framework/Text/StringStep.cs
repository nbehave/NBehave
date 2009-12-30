using System;

namespace NBehave.Narrator.Framework
{
    public class StringStep : ActionStepText
    {
        public static event EventHandler<EventArgs<MessageEventData>> MessageAdded;

        protected IStringStepRunner StringStepRunner { get; private set; }

        public StringStep(string step, string fromFile, IStringStepRunner stringStepRunner)
            : base(step, fromFile)
        {
            StringStepRunner = stringStepRunner;
        }

        public override string ToString()
        {
            return Step;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || obj as StringStep == null)
                return false;

            return ((obj == this) || obj.ToString() == ToString());
        }

        public virtual ActionStepResult Run()
        {
            ActionStepResult actionStepResult = StringStepRunner.Run(this);
            RaiseScenarioMessage(actionStepResult.Result);
            return actionStepResult;
        }

        protected void RaiseScenarioMessage(Result result)
        {
            if (MessageAdded == null)
                return;

            EventArgs<MessageEventData> e;
            MessageEventType stepType = MessageEventType.StringStep;
            if (result is Pending)
                stepType = MessageEventType.Pending;
            if (result.GetType() == typeof(Passed))
                e = new EventArgs<MessageEventData>(new MessageEventData(stepType, Step));
            else
                e = new EventArgs<MessageEventData>(new MessageEventData(stepType, Step + " - " + result.ToString().ToUpper()));

            MessageAdded.Invoke(this, e);
        }
    }
}