using System.Collections.Generic;
using System.IO;
using System.Linq;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.UnitTestFramework;
using JetBrains.Util;
using NBehave.Domain;


namespace NBehave.ReSharper.Plugin.UnitTestProvider
{
    public class FeatureMapper
    {
        private List<NBehaveUnitTestElementBase> _elements;
        private NBehaveUnitTestElementBase _parent;
        private readonly IUnitTestProvider _unitTestProvider;
        private readonly ProjectModelElementEnvoy _projectModel;
        private readonly ISolution _solution;

        public FeatureMapper(IUnitTestProvider unitTestProvider, ProjectModelElementEnvoy projectModel, ISolution solution)
        {
            _unitTestProvider = unitTestProvider;
            _projectModel = projectModel;
            _solution = solution;
        }

        public IEnumerable<NBehaveUnitTestElementBase> MapFeatures(IEnumerable<Feature> features)
        {
            _parent = null;
            _elements = new List<NBehaveUnitTestElementBase>();
            foreach (var feature in features)
            {
                CreateFeatureElement(feature);
                BuildScenarios(feature);
            }
            return _elements;
        }

        private void BuildScenarios(Feature feature)
        {
            foreach (var scenario in feature.Scenarios)
            {
                BuildScenario(scenario);
            }
        }

        private void BuildScenario(Scenario scenario)
        {
            var parent = _parent;
            _parent = new NBehaveScenarioTestElement(scenario.Title, _parent.FeatureFile, _unitTestProvider, _projectModel, _parent);
            Add(_parent);
            BuildBackgroundElements(scenario);
            BuildSteps(scenario.Steps);
            BuildExamples(scenario);
            _parent = parent;
        }

        private void BuildExamples(Scenario scenario)
        {
            if (scenario.Examples.Any() == false)
                return;
            var ex = new NBehaveExampleParentTestElement(scenario.Examples, _parent.FeatureFile, _unitTestProvider, _projectModel, _parent);
            Add(ex);
            foreach (var example in scenario.Examples)
            {
                var s = new NBehaveExampleTestElement(example, _parent.FeatureFile, _unitTestProvider, _projectModel, ex);
                Add(s);
            }
        }

        private void BuildBackgroundElements(Scenario scenario)
        {
            var feature = scenario.Feature;
            if (feature.Background.Steps.Any() == false)
                return;
            var origParent = _parent;
            _parent = new NBehaveBackgroundTestElement(_parent.ShortName, _parent.FeatureFile, _unitTestProvider, _projectModel, _parent);
            Add(_parent);
            BuildSteps(feature.Background.Steps);
            _parent = origParent;
        }

        private void BuildSteps(IEnumerable<StringStep> steps)
        {
            foreach (var step in steps)
            {
                var s = new NBehaveStepTestElement(step.Step, _parent.FeatureFile, _unitTestProvider, _projectModel, _parent);
                Add(s);
            }
        }

        private void CreateFeatureElement(Feature feature)
        {
            IProjectFile featureFile = FindFile(feature);
            _parent = new NBehaveFeatureTestElement(feature.Title, featureFile, _unitTestProvider, _projectModel);
            Add(_parent);
        }

        private IProjectFile FindFile(Feature feature)
        {
            ICollection<IProjectItem> proj = _solution.FindProjectItemsByLocation(new FileSystemPath(feature.Source));

            string featureFileName = Path.GetFileName(feature.Source.ToLower());
            foreach (var item in proj)
            {
                var project = item.GetProject();
                var file = project.GetAllProjectFiles().SingleOrDefault(_ => Path.GetFileName(_.Location.FullPath.ToLower()) == featureFileName);
                if (file != null)
                    return file;
            }
            return null;
        }

        private void Add(NBehaveUnitTestElementBase element)
        {
            if (element != null)
                _elements.Add(element);
        }
    }
}