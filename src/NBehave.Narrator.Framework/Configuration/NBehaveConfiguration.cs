using System;
using System.Collections.Generic;
using NBehave.Narrator.Framework.EventListeners;

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
            get
            {
                return new NBehaveConfiguration
                           {
                               Filter = new StoryRunnerFilter()
                           };
            }
        }

        internal IEnumerable<string> ScenarioFiles { get; set; }

        internal bool IsDryRun { get; set; }

        internal IEnumerable<string> Assemblies { get; set; }

        internal IEventListener EventListener { get; set; }

        internal StoryRunnerFilter Filter { get; set; }

        /// <summary>
        ///   Sets a value indicating whether the action steps should be executed or not.
        /// </summary>
        /// <param name = "dryRun">
        ///   True to execute action steps, false to skip execution.
        /// </param>
        /// <returns>
        ///   The configuration object to resume the fluent interface.
        /// </returns>
        public NBehaveConfiguration SetDryRun(bool dryRun)
        {
            IsDryRun = dryRun;
            return this;
        }

        public string[] FeatureFileExtensions
        {
            get
            {
                return new[]
                           {
                               "feature",
                               "story",
                               "specification",
                               "egenskap"
                           };
            }
        }

        public NBehaveConfiguration SetScenarioFiles(IEnumerable<string> scenarioFiles)
        {
            ScenarioFiles = scenarioFiles;
            return this;
        }

        public NBehaveConfiguration SetAssemblies(IEnumerable<string> assemblies)
        {
            Assemblies = assemblies;
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
    }
}