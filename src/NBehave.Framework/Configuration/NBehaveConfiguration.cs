// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NBehaveConfiguration.cs" company="NBehave">
//   Copyright (c) 2007, NBehave - http://nbehave.codeplex.com/license
// </copyright>
// <summary>
//   Defines the NBehaveConfiguration type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace NBehave.Narrator.Framework
{
    using System.Collections.Generic;

    /// <summary>
    /// Fluent configuration to declare settings for the text based scenario runner.
    /// </summary>
    public class NBehaveConfiguration
    {
        /// <summary>
        /// Gets a new instance of the configuration, used by the fluent interface 
        /// to return a new instance of the configuration.
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

        internal EventListener EventListener { get; set; }

        internal StoryRunnerFilter Filter { get; set; }

        /// <summary>
        /// Sets a value indicating whether the action steps should be executed or not.
        /// </summary>
        /// <param name="dryRun">
        /// True to execute action steps, false to skip execution.
        /// </param>
        /// <returns>
        /// The configuration object to resume the fluent interface.
        /// </returns>
        public NBehaveConfiguration SetDryRun(bool dryRun) 
        {
            this.IsDryRun = dryRun;
            return this;
        }

        public NBehaveConfiguration SetScenarioFiles(IEnumerable<string> scenarioFiles)
        {
            this.ScenarioFiles = scenarioFiles;
            return this;
        }

        public NBehaveConfiguration SetAssemblies(IEnumerable<string> assemblies)
        {
            this.Assemblies = assemblies;
            return this;
        }

        public NBehaveConfiguration SetEventListener(EventListener eventListener)
        {
            this.EventListener = eventListener;
            return this;
        }

        public NBehaveConfiguration SetFilter(StoryRunnerFilter filter)
        {
            this.Filter = filter;
            return this;
        }
    }
}