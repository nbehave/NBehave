namespace NBehave.Narrator.Framework
{
    public class StringStep : ActionStepText
    {
        private readonly IEventListener _listener;
        protected StringStepRunner StringStepRunner { get; private set; }

        public StringStep(string step, string fromFile, StringStepRunner stringStepRunner, IEventListener listener)
            : base(step, fromFile)
        {
            StringStepRunner = stringStepRunner;
            _listener = listener;
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
            ActionStepResult actionStepResult = StringStepRunner.RunStringStep(this);
            RaiseScenarioMessage(actionStepResult.Result);
            return actionStepResult;
        }

        private void RaiseScenarioMessage(Result result)
        {
            if (_listener == null)
                return;
            if (result.GetType() == typeof(Passed))
                _listener.ScenarioMessageAdded(Step);
            else
                _listener.ScenarioMessageAdded(Step + " - " + result.ToString().ToUpper());
        }
    }
}