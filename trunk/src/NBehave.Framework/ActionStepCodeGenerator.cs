using System;
using System.Text.RegularExpressions;

namespace NBehave.Narrator.Framework
{
    public class ActionStepCodeGenerator
    {
        public string GenerateMethodFor(string actionStep, TypeOfStep step)
        {
            string actionStepParameterized = ParameterizeActionStep(actionStep);
            string attrib = step.ToString();
            string attribute = string.Format("[{0}(\"{1}\")]{2}", attrib, actionStepParameterized, Environment.NewLine);
            string methodName = ExtractMethodName(attrib + " " + actionStepParameterized);
            string methodSignature = string.Format("public void {0}({1}){2}",
                methodName.Replace(' ', '_'),
                GetParameters(actionStep),
                " ");
            const string methodBody = "{ throw new System.NotImplementedException(); }";
            return attribute + methodSignature + Environment.NewLine + methodBody;
        }
        private char[] _whiteSpaces = new[] { ' ', '\n', '\r', '\t' };

        private string GetParameters(string row)
        {
            int numberOfParameters = 0;
            string parameters = string.Empty;
            string[] words = SplitStringToWords(ref row);

            foreach (var word in words)
            {
                if (IsParameter(word))
                {
                    numberOfParameters++;
                    var paramName = string.Format("param{0}", numberOfParameters);
                    if (IsInt(word))
                        parameters += string.Format("int {0}, ", paramName);
                    else
                        parameters += string.Format("string {0}, ", paramName);
                }
            }
            if (numberOfParameters == 0)
                return string.Empty;
            return parameters.Substring(0, parameters.Length - 2);
        }

        private string ParameterizeActionStep(string row)
        {
            string actionStep = string.Empty;
            int paramNumber = 1;
            string[] words = SplitStringToWords(ref row);
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

        private string[] SplitStringToWords(ref string row)
        {
            row = TrimRow(row).RemoveFirstWord();
            string[] words = row.Split(_whiteSpaces, StringSplitOptions.RemoveEmptyEntries);
            return words;
        }

        private Regex _stringRegex = new Regex(@"^('|"").+('|"")$");
        private bool IsParameter(string word)
        {
            return IsInt(word) || _stringRegex.IsMatch(word);
        }

        private bool IsInt(string word)
        {
            int dummy;
            bool isInt = int.TryParse(word, out dummy);
            return isInt;
        }

        private string TrimRow(string row)
        {
            string trimmedRow = row;
            var start = new Regex(@"^\s*");
            if (start.IsMatch(trimmedRow))
                trimmedRow = trimmedRow.Substring(start.Match(trimmedRow).Length);

            var end = new Regex(@"\s*$");
            if (end.IsMatch(trimmedRow))
                trimmedRow = trimmedRow.Substring(0, trimmedRow.Length - end.Match(trimmedRow).Length);
            return trimmedRow;
        }

        private string ExtractMethodName(string row)
        {
            var regEx = new Regex(@"(\w+\d*)(\w|\d)*");
            string methodName = string.Empty;
            foreach (Match match in regEx.Matches(row))
            {
                methodName += match.Value + "_";
            }
            return methodName.Substring(0, methodName.Length - 1);
        }
    }

    public enum TypeOfStep
    {
        Given, When, Then
    }
}
