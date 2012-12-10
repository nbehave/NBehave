using System;
using System.Collections.Generic;
using System.Linq;
using NBehave.Narrator.Framework.EventListeners;
using NBehave.Narrator.Framework.Internal;

namespace NBehave.Narrator.Framework
{
    /// <summary>
    ///   Fluent configuration to declare settings for the text based scenario runner.
    /// </summary>
    public class NBehaveConfiguration : MarshalByRefObject
    {
        /// <summary>
        ///   Gets a new instance of the configuration, used by the fluent interface 
        ///   to return a new instance of the configuration.
        /// </summary>
        public static NBehaveConfiguration New
        {
            get { return new NBehaveConfiguration(); }
        }

        private NBehaveConfiguration()
            : this(true)
        { }

        private NBehaveConfiguration(bool createAppDomain)
        {
            CreateAppDomain = createAppDomain;
            EventListener = new NullEventListener();
            Filter = new StoryRunnerFilter();
            TagsFilter = new List<string[]>();
            Assemblies = new List<string>();
        }

        public IEnumerable<string> ScenarioFiles { get; private set; }
        public bool IsDryRun { get; set; }
        public IEnumerable<string> Assemblies { get; private set; }
        public IEventListener EventListener { get; private set; }
        public StoryRunnerFilter Filter { get; private set; }
        public IEnumerable<string[]> TagsFilter { get; private set; }
        public bool CreateAppDomain { get; private set; }

        public NBehaveConfiguration SetDryRun(bool dryRun)
        {
            IsDryRun = dryRun;
            return this;
        }

        public static IEnumerable<string> FeatureFileExtensions
        {
            get
            {
                return new[]
                           {
                               ".feature",
                               ".story",
                               ".specification",
                               ".egenskap"
                           };
            }
        }

        public NBehaveConfiguration SetScenarioFiles(IEnumerable<string> scenarioFiles)
        {
            ScenarioFiles = scenarioFiles.ToList();
            return this;
        }

        public NBehaveConfiguration SetAssemblies(IEnumerable<string> assemblies)
        {
            Assemblies = assemblies.ToList();
            return this;
        }

        public NBehaveConfiguration SetEventListener(IEventListener eventListener)
        {
            EventListener = eventListener;
            return this;
        }

        public NBehaveConfiguration SetFilter(StoryRunnerFilter filter)
        {
            Filter = filter;
            return this;
        }

        public NBehaveConfiguration DontIsolateInAppDomain()
        {
            CreateAppDomain = false;
            return this;
        }

        public NBehaveConfiguration UseTagsFilter(IEnumerable<string[]> tagsFilter)
        {
            TagsFilter = tagsFilter.ToList();
            return this;
        }

        public override object InitializeLifetimeService()
        {
            return null;
        }
    }
}