using JetBrains.Metadata.Reader.API;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.UnitTestFramework;

namespace NBehave.ReSharper.Plugin.UnitTestProvider
{
    [MetadataUnitTestExplorer]
    public class UnitTestMetadataExplorer : IUnitTestMetadataExplorer
    {
        private readonly TestProvider _provider;
        public IUnitTestProvider Provider
        {
            get
            {
                return _provider;
            }
        }

        public UnitTestMetadataExplorer(TestProvider provider)
        {
            _provider = provider;
        }

        public void ExploreAssembly(IProject project, IMetadataAssembly assembly, UnitTestElementConsumer consumer)
        {
            new MetadataExplorer(_provider, project, consumer).ExploreAssembly(assembly);
        }
    }
}