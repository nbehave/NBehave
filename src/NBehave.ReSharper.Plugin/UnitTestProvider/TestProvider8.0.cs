using JetBrains.ReSharper.TaskRunnerFramework;
using NBehave.ReSharper.Plugin.UnitTestRunner;

namespace NBehave.ReSharper.Plugin.UnitTestProvider
{
    public partial class TestProvider
    {
        public RemoteTaskRunnerInfo GetTaskRunnerInfo()
        {
            return new RemoteTaskRunnerInfo(NBehaveId, typeof(NBehaveTaskRunner));
        }
    }
}
