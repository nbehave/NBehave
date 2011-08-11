using JetBrains.Metadata.Reader.API;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.UnitTestFramework;

namespace NBehave.ReSharper.Plugin.UnitTestProvider
{
    [MetadataUnitTestExplorer]
    public class UnitTestMetadataExplorer : IUnitTestMetadataExplorer
    {
        private readonly TestProvider _provider;
        private readonly ISolution _solution;

        public IUnitTestProvider Provider
        {
            get
            {
                return _provider;
            }
        }

        public UnitTestMetadataExplorer(TestProvider provider, ISolution solution)
        {
            _provider = provider;
            _solution = solution;
        }

        public void ExploreAssembly(IProject project, IMetadataAssembly assembly, UnitTestElementConsumer consumer)
        {
            new MetadataExplorer(_provider, _solution, project, consumer).ExploreProject();
        }
    }
}