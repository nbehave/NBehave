namespace NBehave.Narrator.Framework
{
    public class FeatureContext : NBehaveContext
    {
        public string FeatureTitle { get; internal set; }

        public static FeatureContext Current
        {
            get { return Tiny.TinyIoCContainer.Current.Resolve<FeatureContext>(); }
        }

        public override string ToString()
        {
            return FeatureTitle;
        }
    }
}