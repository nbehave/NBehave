using JetBrains.Application;
using JetBrains.Metadata.Reader.API;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.UnitTestFramework;

namespace NBehave.ReSharper.Plugin.UnitTestProvider
{
    public partial class UnitTestMetadataExplorer
    {
        public void ExploreAssembly(IProject project, IMetadataAssembly assembly, UnitTestElementConsumer consumer)
        {
            ReadLockCookie.Execute(() => new MetadataExplorer(_provider, _solution, project, consumer).ExploreProject());
        }
    }
}
