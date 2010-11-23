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
        public Feature()
            : this(string.Empty)
        {
        }

        public Feature(string title)
        {
            this.Scenarios = new List<ScenarioWithSteps>();
            ExtractTitleAndNarrative(title);
        }

        public string Title { get; set; }

        public string Narrative { get; set; }

        public List<ScenarioWithSteps> Scenarios { get; private set; }

        public bool HasTitle
        {
            get
            {
                return !string.IsNullOrEmpty(Title);
            }
        }

        public void AddScenario(ScenarioWithSteps scenario)
        {
            this.Scenarios.Add(scenario);
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