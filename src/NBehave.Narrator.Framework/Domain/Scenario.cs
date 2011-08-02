// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Scenario.cs" company="NBehave">
//   Copyright (c) 2007, NBehave - http://nbehave.codeplex.com/license
// </copyright>
// <summary>
//   Defines the Scenario type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
using System.Collections.Generic;

namespace NBehave.Narrator.Framework
{
    public class Scenario
    {
        private readonly List<StringStep> _steps;
        private readonly List<Example> _examples;

        private string _source;

        public Scenario() : this(string.Empty)
        {
        }

        public Scenario(string title) : this(title, new Feature())
        {
            Feature = new Feature();
            Title = title;
            _steps = new List<StringStep>();
            _examples = new List<Example>();
        }

        public Scenario(string title, Feature feature)
        {
            Feature = feature;
            Title = title;
            _steps = new List<StringStep>();
            _examples = new List<Example>();
        }

        public string Title { get; set; }

        public Feature Feature { get; set; }

        public IEnumerable<StringStep> Steps
        {
            get { return _steps; }
        }

        public string Source
        {
            get
            {
                return _source;
            }

            set
            {
                _source = value;
                foreach (var step in Steps)
                {
                    step.Source = _source;
                }
            }
        }

        public IEnumerable<Example> Examples
        {
            get
            {
                return _examples;
            }
        }

        public void AddStep(string step)
        {
            var stringStringStep = new StringStep(step, Source);
            AddStep(stringStringStep);
        }

        public void AddStep(StringStep step)
        {
            _steps.Add(step);
        }

        public void AddExamples(IEnumerable<Example> examples)
        {
            _examples.AddRange(examples);
        }
    }
}