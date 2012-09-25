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
using System.Linq;

namespace NBehave.Narrator.Framework
{
    [Serializable]
    public class Scenario
    {
        private readonly List<StringStep> _steps;
        private readonly List<Example> _examples;

        public Scenario()
            : this(string.Empty, string.Empty)
        {
        }

        public Scenario(string title, string source)
            : this(title, source, new Feature())
        {
            Feature = new Feature();
            Title = title;
            _steps = new List<StringStep>();
            _examples = new List<Example>();
        }

        public Scenario(string title, string source, Feature feature)
            : this(title, source, feature, -1)
        { }

        public Scenario(string title, string source, Feature feature, int sourceLine)
        {
            Feature = feature;
            Title = title;
            Source = source;
            SourceLine = sourceLine;
            _steps = new List<StringStep>();
            _examples = new List<Example>();
        }

        public string Source { get; private set; }
        public int SourceLine { get; private set; }
        public string Title { get; private set; }
        public Feature Feature { get; set; }

        public IEnumerable<StringStep> Steps
        {
            get { return _steps; }
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

        public string ToStringAsBackground()
        {
            return ToString("Background", "  ");
        }

        public override string ToString()
        {
            return ToString("Scenario", "");
        }

        private string ToString(string type, string prefixAllLinesWith)
        {
            var scenario = prefixAllLinesWith + type + ": " + Title + Environment.NewLine;
            var steps = Steps.Select(_ => prefixAllLinesWith + "  " + _.ToString()).ToArray();
            var str = scenario + String.Join(Environment.NewLine, steps);
            if (_examples.Any())
                str += Environment.NewLine + ExamplesAsString(prefixAllLinesWith);
            return str;
        }

        private string ExamplesAsString(string prefixAllLinesWith)
        {
            if (!_examples.Any())
                return "";
            var prefix = prefixAllLinesWith + "    ";
            var str = prefixAllLinesWith + "Examples:" + Environment.NewLine + 
                prefix + _examples.First().ColumnNamesToString() + Environment.NewLine;
            var colStr = _examples.Select(_ => prefix + _.ColumnValuesToString()).ToArray();
            return str + string.Join(Environment.NewLine, colStr);
        }
    }
}