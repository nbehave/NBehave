namespace NBehave.Narrator.Framework
{
    public class ScenarioContext : NBehaveContext
    {
        public FeatureContext FeatureContext { get; protected set; }
        public virtual string ScenarioTitle { get; internal set; }

        public ScenarioContext(FeatureContext featureContext)
        {
            FeatureContext = featureContext;
        }

        public static ScenarioContext Current
        {
            get { return Tiny.TinyIoCContainer.Current.Resolve<ScenarioContext>(); }
        }

        public override string ToString()
        {
            return ScenarioTitle;
        }
    }
}