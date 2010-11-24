// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ScenarioWithSteps.cs" company="NBehave">
//   Copyright (c) 2007, NBehave - http://nbehave.codeplex.com/license
// </copyright>
// <summary>
//   Defines the ScenarioWithSteps type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace NBehave.Narrator.Framework
{
    using System.Collections.Generic;
    using System.Linq;

    public class ScenarioWithSteps
    {
        private readonly List<StringStep> _steps;
        private readonly List<Example> _examples;

        private string _source;

        public ScenarioWithSteps()
        {
            Feature = new Feature();
            Title = string.Empty;
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

        public void AddExamples(List<Example> examples)
        {
            _examples.AddRange(examples);
        }

        public void RemoveLastStep()
        {
            _steps.Remove(_steps.Last());
        }
    }
}