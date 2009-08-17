using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace NBehave.Narrator.Framework
{
    public class TextToTokenStringsParser
    {
        public IList<string> TokenStrings { get; private set; }

        private readonly ActionStepAlias _actionStepAlias;

        public TextToTokenStringsParser(ActionStepAlias actionStepAlias)
        {
            _actionStepAlias = actionStepAlias;
        }

        public void ParseScenario(string scenario)
        {
            string normalizedScenario = NormalizeNewLine(scenario);
            TokenStrings = new List<string>();
            string scenarioText = RemoveWhiteSpacesAtBeginning(normalizedScenario);
            while (scenarioText.Length > 0)
            {
                string nextTokenString = GetNextToken(scenarioText).Trim();
                if (nextTokenString.Length > 0)
                    TokenStrings.Add(nextTokenString);
                scenarioText = scenarioText.Remove(0, nextTokenString.Length);
                scenarioText = RemoveWhiteSpacesAtBeginning(scenarioText);
            }
        }

        private string RemoveWhiteSpacesAtBeginning(string text)
        {
            var whiteSpaces = new Regex(@"^\s*");
            Match match = whiteSpaces.Match(text);
            return text.Substring(match.Value.Length);
        }

        private string GetNextToken(string scenario)
        {
            string regexString = BuildRegexString();
            var regex = new Regex(regexString, RegexOptions.IgnoreCase);
            string[] actionRows = Split(scenario);
            int firstActionWordRow = FindRowForFirstActionWord(actionRows, regex);
            var secondActionWordRow = FindRowForNextActionWord(actionRows, regex, firstActionWordRow + 1);
            string actionRow;
            if (actionRows.Length == 1 && secondActionWordRow == 0 && regex.IsMatch(actionRows[0]))
                actionRow = actionRows[0];
            else
                actionRow = BuildActionStep(actionRows, firstActionWordRow, secondActionWordRow);
            return actionRow;
        }

        private string BuildActionStep(string[] rows, int startRow, int endRow)
        {
            string actionRow = string.Empty;
            for (int row = startRow; row < endRow; row++)
            {
                actionRow += rows[row] + Environment.NewLine;
            }
            return actionRow.TrimEnd(Environment.NewLine.ToCharArray());
        }

        private int FindRowForFirstActionWord(string[] rows, Regex regex)
        {
            return FindRowForNextActionWord(rows, regex, 0);
        }

        private int FindRowForNextActionWord(string[] rows, Regex regex, int startAtRow)
        {
            int row = startAtRow;
            while (row < rows.Length && regex.IsMatch(rows[row]) == false)
                row++;
            if (row == rows.Length && regex.IsMatch(rows[row - 1]))
                row--;
            return row;
        }

        private string NormalizeNewLine(string text)
        {
            string str = text.Replace(Environment.NewLine, "\n");
            str = str.Replace('\r', '\n');
            return str.Replace("\n", Environment.NewLine); ;
        }

        private string[] Split(string text)
        {
            string[] rows = text.Replace(Environment.NewLine, "\n").Split(new char[] { '\n' });
            if (string.IsNullOrEmpty(rows[rows.Length - 1]))
            {
                return rows.Take(rows.Length - 1).ToArray();
            }
            return rows;
        }

        private int GetIndexOfNewLine(string scenario, int first)
        {
            int index = scenario.IndexOf(Environment.NewLine, first);
            if (index == -1)
                index = scenario.IndexOf("\n", first);
            if (index == -1)
                index = scenario.IndexOf("\r", first);
            return index;
        }

        private string BuildRegexString()
        {
            string regex = ""; // @"^\n*";
            var aliases = new List<string>();
            foreach (string actionWord in _actionStepAlias.AliasesForAllActionWords.Keys)
            {
                if (aliases.Contains(actionWord) == false)
                    aliases.Add(actionWord);
                foreach (var alias in _actionStepAlias.AliasesForAllActionWords[actionWord])
                {
                    if (aliases.Contains(alias) == false)
                        aliases.Add(alias);
                }
            }
            foreach (var alias in aliases)
            {
                regex += alias + "|";
            }
            regex = regex.Substring(0, regex.Length - 1);
            return regex;
        }

        public string ScenarioMessage()
        {
            string scenarioMessageToAdd = string.Empty;

            foreach (var row in TokenStrings)
            {
                if (Scenario.IsScenarioTitle(row) == false)
                    scenarioMessageToAdd += row + Environment.NewLine;
            }
            return scenarioMessageToAdd;
        }
    }
}