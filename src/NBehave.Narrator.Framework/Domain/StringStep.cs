using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using NBehave.Narrator.Framework.Extensions;
using NBehave.Narrator.Framework.Internal;

namespace NBehave.Narrator.Framework
{
    [Serializable]
    public class StringStep
    {
        public StringStep(string step, string source)
        {
            Step = step;
            Source = source;
            StepResult = new StepResult(this, new Passed());
        }

        private string _matchableStep;

        public string MatchableStep
        {
            get { return _matchableStep; }
        }

        private string _step;

        public string Step
        {
            get { return _step; }
            private set
            {
                _step = value;
                _matchableStep = value.RemoveFirstWord();
            }
        }

        public string Source { get; private set; }
        public StepResult StepResult { get; set; }

        public virtual IEnumerable<MethodParametersType> MatchableStepTypes
        {
            get { return new[] { MethodParametersType.TypedStep, MethodParametersType.UntypedStep, MethodParametersType.UntypedListStep, MethodParametersType.TypedListStep }; }
        }

        public TypeOfStep TypeOfStep
        {
            get
            {
                var validNames = Enum.GetNames(typeof(TypeOfStep)).ToList();
                var firstWord = Step.GetFirstWord();
                if (validNames.Contains(firstWord))
                    return (TypeOfStep)Enum.Parse(typeof(TypeOfStep), firstWord, true);
                return TypeOfStep.Unknown;
            }
        }

        public bool HasDocString { get; set; }
        public string DocString { get; set; }

        public void AddDocString(string docString)
        {
            HasDocString = string.IsNullOrEmpty(docString) == false;
            DocString = docString;
        }
        public virtual StringStep BuildStep(Example values)
        {
            var template = Step;
            foreach (var columnName in values.ColumnNames)
            {
                var columnValue = values.ColumnValues[columnName.Name].TrimWhiteSpaceChars();
                var replace = BuildColumnValueReplaceRegex(columnName);
                template = replace.Replace(template, columnValue);
            }
            return new StringStep(template, Source);
        }

        protected static Regex BuildColumnValueReplaceRegex(ExampleColumn columnName)
        {
            return new Regex(string.Format(@"(\${0})|(\[{0}\])|(\<{0}\>)", columnName), RegexOptions.IgnoreCase);
        }

        public void Pend(string reason)
        {
            StepResult = new StepResult(this, new Pending(reason));
        }

        public void PendNotImplemented(string pendReason)
        {
            StepResult = new StepResult(this, new PendingNotImplemented(pendReason));
        }

        public void PendBecauseOfPreviousFailedStep()
        {
            StepResult = new StepResult(this, new Skipped("Previous step has failed"));
        }

        public void Fail(Exception exception)
        {
            StepResult = new StepResult(this, new Failed(exception));
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as StringStep);
        }

        public bool Equals(StringStep other)
        {
            if (other == null)
                return false;
            return (ReferenceEquals(this, other)) || (other.MatchableStep == MatchableStep && other.Source == Source);
        }

        public override int GetHashCode()
        {
            return (Step != null ? Step.GetHashCode() : 0);
        }

        public static bool operator ==(StringStep left, StringStep right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(StringStep left, StringStep right)
        {
            return !Equals(left, right);
        }

        public override string ToString()
        {
            return Step;
        }
    }
}