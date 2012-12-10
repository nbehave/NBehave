namespace NBehave.Narrator.Framework.Specifications
{
    public static class ConfigurationNoAppDomain
    {
        public static NBehaveConfiguration New
        {
            get
            {
                return NBehaveConfiguration.New.DontIsolateInAppDomain();
            }
        }
    }
}