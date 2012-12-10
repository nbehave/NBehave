using NBehave.Configuration;

namespace NBehave.Specifications
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