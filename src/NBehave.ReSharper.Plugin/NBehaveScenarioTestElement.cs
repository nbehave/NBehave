using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.TaskRunnerFramework;
using JetBrains.ReSharper.UnitTestFramework;
using NBehave.ReSharper.Plugin.Task;

namespace NBehave.ReSharper.Plugin
{
    public class NBehaveScenarioTestElement : NBehaveUnitTestElementBase
    {
        private readonly string _featureFile;
        private readonly string _projectFile;
        private readonly string _assemblyOutFile;

        public NBehaveScenarioTestElement(IProjectFile feature, TestProvider testProvider, ProjectModelElementEnvoy projectModel, ClrTypeName typeName)
            : base(testProvider, feature.Location.FullPath, typeName, projectModel, null)
        {
            _featureFile = feature.Location.FullPath;
            var project = feature.GetProject();
            _projectFile = project.Name;
            _assemblyOutFile = project.GetOutputAssemblyFile().Location.FullPath;
        }

        public override string ShortName
        {
            get { return Path.GetFileName(_featureFile); }
        }

        public override string Kind
        {
            get { return "NBehave scenario"; }
        }

        public override IList<UnitTestTask> GetTaskSequence(IEnumerable<IUnitTestElement> explicitElements)
        {
            var list = new List<UnitTestTask>
			{
				new UnitTestTask(null, new AssemblyLoadTask(_assemblyOutFile)), 
                //new UnitTestTask(this, new NBehaveAssemblySetTask(_assemblyOutFile)),
				new UnitTestTask(this, new RunScenarioTask(_featureFile))
			};
            return list;
        }

        public override IEnumerable<IProjectFile> GetProjectFiles()
        {
            IProject project = GetProject();
            var files = project.GetAllProjectFiles().Where(_ => _.Location.FullPath == _featureFile);
            return files.ToList();
        }

        public override string GetPresentation()
        {
            var f = File.ReadAllText(_featureFile);
            return f;
        }

        public override UnitTestElementDisposition GetDisposition()
        {
            throw new NotImplementedException();
        }

        public override IDeclaredElement GetDeclaredElement()
        {
            throw new NotImplementedException();
        }

        public override bool Equals(IUnitTestElement other)
        {
            var o = other as NBehaveScenarioTestElement;
            return o != null
                   && _featureFile == o._featureFile;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = _featureFile.GetHashCode();
                result = (result * 397) ^ _projectFile.GetHashCode();
                result = (result * 397) ^ _assemblyOutFile.GetHashCode();
                return result;
            }
        }
    }
}