namespace NBehave.Narrator.Framework
{
    public class StepContext 
    {
        public FeatureContext FeatureContext { get; protected set; }
        public ScenarioContext ScenarioContext { get; protected set; }
        public string Step { get; internal set; }

        public StepContext(FeatureContext featureContext, ScenarioContext scenarioContext)
        {
            FeatureContext = featureContext;
            ScenarioContext = scenarioContext;
        }

        public static StepContext Current
        {
            get { return Tiny.TinyIoCContainer.Current.Resolve<StepContext>(); }
        }

        public override string ToString()
        {
            return Step;
        }
    }
}