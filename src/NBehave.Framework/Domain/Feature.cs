// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Feature.cs" company="NBehave">
//   Copyright (c) 2007, NBehave - http://nbehave.codeplex.com/license
// </copyright>
// <summary>
//   Defines the Feature type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace NBehave.Narrator.Framework
{
    using System;
    using System.Collections.Generic;

    public class Feature
    {
        private readonly List<ScenarioWithSteps> _scenarios = new List<ScenarioWithSteps>();

        public Feature()
            : this(string.Empty)
        {
        }

        public Feature(string title)
        {
            ExtractTitleAndNarrative(title);
        }

        public static event EventHandler<EventArgs<Feature>> FeatureCreated;

        public string Title { get; set; }

        public string Narrative { get; set; }

        public bool IsDryRun { get; set; }

        public IEnumerable<ScenarioWithSteps> Scenarios
        {
            get
            {
                return _scenarios;
            }
        }

        public bool HasTitle
        {
            get
            {
                return !string.IsNullOrEmpty(Title);
            }
        }

        public void AddScenario(ScenarioWithSteps scenario)
        {
            _scenarios.Add(scenario);
        }

        public void RaiseFeatureCreated()
        {
            if (FeatureCreated == null)
            {
                return;
            }

            var e = new EventArgs<Feature>(this);
            FeatureCreated(null, e);
        }

        public void ExtractTitleAndNarrative(string content)
        {
            if (content.Contains(Environment.NewLine))
            {
                var lineBreakPosn = content.IndexOf(Environment.NewLine);
                Title = content.Substring(0, lineBreakPosn);
                Narrative = content.Substring(lineBreakPosn + Environment.NewLine.Length);
            }
            else
            {
                Title = content;
                Narrative = String.Empty;
            }
        }
    }
}