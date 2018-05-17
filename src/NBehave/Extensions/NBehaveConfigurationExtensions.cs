using NBehave.Configuration;
using NBehave.Internal;

namespace NBehave.Extensions
{
    public static class NBehaveConfigurationExtensions
    {
        public static IRunner Build(this NBehaveConfiguration configuration)
        {
            return RunnerFactory.CreateTextRunner(configuration);
        }
    }
}
