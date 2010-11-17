// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ActionStepCodeGenerator.cs" company="NBehave">
//   Copyright (c) 2007, NBehave - http://nbehave.codeplex.com/license
// </copyright>
// <summary>
//   Defines the ActionStepCodeGenerator type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace NBehave.Narrator.Framework
{
    using System;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;

    public class ActionStepCodeGenerator
    {
        private readonly Regex _stringRegex = new Regex(@"^('|"").+('|"")$");

        private readonly char[] _whiteSpaces = new[] { ' ', '\n', '\r', '\t' };

        public string GenerateMethodFor(string actionStep, TypeOfStep step)
        {
            var actionStepParameterized = ParameterizeActionStep(actionStep);
            var attrib = step.ToString();
            var attribute = string.Format("[{0}(\"{1}\")]{2}", attrib, actionStepParameterized, Environment.NewLine);
            var methodName = ExtractMethodName(attrib + " " + actionStepParameterized);
            var methodSignature = string.Format(
                "public void {0}({1}){2}",
                methodName.Replace(' ', '_'),
                GetParameters(actionStep),
                " ");
            string methodBody = "{ throw new System.NotImplementedException(); }";
            return attribute + methodSignature + Environment.NewLine + methodBody;
        }

        private string GetParameters(string row)
        {
            var numberOfParameters = 0;
            var parameters = string.Empty;
            var words = SplitStringToWords(ref row);

            foreach (var word in words)
            {
                if (IsParameter(word))
                {
                    numberOfParameters++;
                    var paramName = string.Format("param{0}", numberOfParameters);
                    if (IsInt(word))
                    {
                        parameters += string.Format("int {0}, ", paramName);
                    }
                    else
                    {
                        parameters += string.Format("string {0}, ", paramName);
                    }
                }
            }

            if (numberOfParameters == 0)
            {
                return string.Empty;
            }

            return parameters.Substring(0, parameters.Length - 2);
        }

        private string ParameterizeActionStep(string row)
        {
            var actionStep = string.Empty;
            var paramNumber = 1;
            var words = SplitStringToWords(ref row);
            foreach (var word in words)
            {
                if (IsParameter(word))
                {
                    actionStep += string.Format("$param{0} ", paramNumber);
                    paramNumber++;
                }
                else
                {
                    actionStep += word + " ";
                }
            }

            return actionStep.Substring(0, actionStep.Length - 1);
        }

        private IEnumerable<string> SplitStringToWords(ref string row)
        {
            row = TrimRow(row).RemoveFirstWord();
            var words = row.Split(_whiteSpaces, StringSplitOptions.RemoveEmptyEntries);
            return words;
        }

        private bool IsParameter(string word)
        {
            return IsInt(word) || _stringRegex.IsMatch(word);
        }

        private bool IsInt(string word)
        {
            int dummy;
            var isInt = int.TryParse(word, out dummy);
            return isInt;
        }

        private string TrimRow(string row)
        {
            var trimmedRow = row;
            var start = new Regex(@"^\s*");
            if (start.IsMatch(trimmedRow))
            {
                trimmedRow = trimmedRow.Substring(start.Match(trimmedRow).Length);
            }

            var end = new Regex(@"\s*$");
            if (end.IsMatch(trimmedRow))
            {
                trimmedRow = trimmedRow.Substring(0, trimmedRow.Length - end.Match(trimmedRow).Length);
            }

            return trimmedRow;
        }

        private string ExtractMethodName(string row)
        {
            var regEx = new Regex(@"(\w+\d*)(\w|\d)*");
            var methodName = string.Empty;
            foreach (Match match in regEx.Matches(row))
            {
                methodName += match.Value + "_";
            }

            return methodName.Substring(0, methodName.Length - 1);
        }
    }
}
