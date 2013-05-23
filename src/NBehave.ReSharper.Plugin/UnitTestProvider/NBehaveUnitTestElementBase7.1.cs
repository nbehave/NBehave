using System.Collections.Generic;
using System.Linq;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.UnitTestFramework;

namespace NBehave.ReSharper.Plugin.UnitTestProvider
{
    public abstract partial class NBehaveUnitTestElementBase
    {
        public string AssemblyOutFile { get { return _project.GetOutputAssemblyFile().Location.FullPath; } }

        public IList<UnitTestTask> GetTaskSequence(ICollection<IUnitTestElement> explicitElements, IUnitTestLaunch launch)
        {
            return GetTaskSequence(explicitElements.ToList());
        }
    }
}