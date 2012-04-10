// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Scenario.cs" company="NBehave">
//   Copyright (c) 2007, NBehave - http://nbehave.codeplex.com/license
// </copyright>
// <summary>
//   Defines the Scenario type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace NBehave.Narrator.Framework
{
    [Serializable]
    public class Scenario
    {
        private readonly List<StringStep> _steps;
        private readonly List<Example> _examples;

        private readonly string _source;

        public Scenario() : this(string.Empty, string.Empty)
        {
        }

        public Scenario(string title, string source) : this(title, source, new Feature())
        {
            Feature = new Feature();
            Title = title;
            _steps = new List<StringStep>();
            _examples = new List<Example>();
        }

        public Scenario(string title, string source, Feature feature)
        {
            Feature = feature;
            Title = title;
            _steps = new List<StringStep>();
            _examples = new List<Example>();
            _source = source;
        }

        public string Title { get; private set; }

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