// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ActionStepText.cs" company="NBehave">
//   Copyright (c) 2007, NBehave - http://nbehave.codeplex.com/license
// </copyright>
// <summary>
//   Defines the ActionStepText type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace NBehave.Narrator.Framework
{
    public class ActionStepText
    {
        public ActionStepText(string text, string source)
        {
            Step = text;
            Source = source;
        }

        public string Step { get; set; }

        public string Source { get; set; }

        public override string ToString()
        {
            return Step;
        }
    }
}
