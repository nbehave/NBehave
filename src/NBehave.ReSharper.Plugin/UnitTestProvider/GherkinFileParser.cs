using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.ProjectModel;
using JetBrains.ProjectModel.Model2.Assemblies.Interfaces;
using JetBrains.ReSharper.UnitTestFramework;
using NBehave.Narrator.Framework;
using NBehave.Narrator.Framework.EventListeners;
using NBehave.Narrator.Framework.Tiny;

namespace NBehave.ReSharper.Plugin.UnitTestProvider
{
    public class GherkinFileParser
    {
        private readonly ITinyMessengerHub _hub;
        private readonly IUnitTestProvider _testProvider;
        private readonly ProjectModelElementEnvoy _projectModel;
        private List<NBehaveUnitTestElementBase> _elements;

        private string _currentFile = string.Empty;
        private NBehaveFeatureTestElement _currentFeature;
        private NBehaveScenarioTestElement _currentScenario;
        private readonly Dictionary<string, IProjectFile> _featureFiles = new Dictionary<string, IProjectFile>();
        private readonly List<KeyValuePair<TinyMessageSubscriptionToken, Type>> _hubSubscriberTokens = new List<KeyValuePair<TinyMessageSubscriptionToken, Type>>();

        public GherkinFileParser(IAssemblyFile assembly, IEnumerable<IProjectFile> featureFiles, IUnitTestProvider testProvider, ProjectModelElementEnvoy projectModel)
        {
            _testProvider = testProvider;
            _projectModel = projectModel;
            var container = TinyIoCContainer.Current;
            foreach (var featureFile in featureFiles)
                _featureFiles.Add(featureFile.Location.FullPath.ToLower(), featureFile);
            NBehaveConfiguration config = CreateConfiguration(assembly.Location.FullPath.ToLower(), _featureFiles.Keys);
            NBehaveInitialiser.Initialise(container, config);
            _hub = container.Resolve<ITinyMessengerHub>();
        }

        private NBehaveConfiguration CreateConfiguration(string pathToAssembly, IEnumerable<string> featureFiles)
        {
            return NBehaveConfiguration
                .New
                .SetAssemblies(new[] { pathToAssembly })
                .SetEventListener(EventListeners.NullEventListener())
                .SetScenarioFiles(featureFiles);
        }

        public IEnumerable<NBehaveUnitTestElementBase> ParseFilesToElements(IEnumerable<IProjectFile> featureFiles)
        {
            _elements = new List<NBehaveUnitTestElementBase>();
            var parser = new GherkinScenarioParser(_hub);
            AddSubscriptions();
            foreach (var featureFile in featureFiles)
                parser.Parse(featureFile.Location.FullPath);
            RemoveSubscriptions();
            return _elements;
        }

        private void AddSubscriptions()
        {
            Subscribe<ParsingFileStart>(ParsingFileStart);
            Subscribe<ParsedFeature>(ParsedFeature);
            Subscribe<ParsedScenario>(ParsedBackground);
            Subscribe<ParsedScenario>(ParsedScenario);
            Subscribe<ParsedStep>(ParsedStep);
        }

        private void Subscribe<T>(Action<T> subscriber) where T : class, ITinyMessage
        {
            var token = _hub.Subscribe(subscriber);
            _hubSubscriberTokens.Add(new KeyValuePair<TinyMessageSubscriptionToken, Type>(token, typeof(T)));
        }

        private void RemoveSubscriptions()
        {
            foreach (var tokenPair in _hubSubscriberTokens)
            {
                var token = tokenPair.Key;
                var type = tokenPair.Value;
                _hub.Unsubscribe(token, type);
            }
        }

        private void ParsingFileStart(ParsingFileStart obj)
        {
            _currentFile = obj.Content.ToLower();
        }

        private void ParsedFeature(ParsedFeature obj)
        {
            var element = new NBehaveFeatureTestElement(obj.Content, FeatureFile, _testProvider, _projectModel);
            AddToElements(element);
            _currentFeature = element;
        }

        private void ParsedScenario(ParsedScenario obj)
        {
            var element = new NBehaveScenarioTestElement(obj.Content, FeatureFile, _testProvider, _projectModel, GetParent(_currentFeature));
            AddToElements(element);
            _currentScenario = element;
        }

        private void ParsedBackground(ParsedScenario obj)
        {
            var element = new NBehaveScenarioTestElement(obj.Content, FeatureFile, _testProvider, _projectModel, GetParent(_currentFeature));
            AddToElements(element);
            _currentScenario = element;
        }

        private void ParsedStep(ParsedStep obj)
        {
            var element = new NBehaveStepTestElement(obj.Content, FeatureFile, _testProvider, _projectModel, GetParent(_currentScenario, _currentFeature));
            AddToElements(element);
        }

        private IProjectFile FeatureFile
        {
            get { return _featureFiles[_currentFile]; }
        }

        private void AddToElements(NBehaveUnitTestElementBase element)
        {
            _elements.Add(element);
        }

        private NBehaveUnitTestElementBase GetParent(params NBehaveUnitTestElementBase[] elements)
        {
            return elements.FirstOrDefault(element => element != null);
        }
    }
}