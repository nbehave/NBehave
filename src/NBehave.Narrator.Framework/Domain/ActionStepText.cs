// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ActionStepText.cs" company="NBehave">
//   Copyright (c) 2007, NBehave - http://nbehave.codeplex.com/license
// </copyright>
// <summary>
//   Defines the ActionStepText type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Linq;

namespace NBehave.Narrator.Framework
{
    [Serializable]
    public class ActionStepText
    {
        public ActionStepText(string text, string source)
        {
            Step = text;
            Source = source;
        }

        private string _matchableStep;
        public string MatchableStep { get { return _matchableStep; } }

        private string _step;
        public string Step
        {
            get { return _step; }
            set
            {
                _step = value;
                _matchableStep = value.RemoveFirstWord();
            }
        }

        public string Source { get; set; }

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

        public override string ToString()
        {
            return Step;
        }
    }
}
