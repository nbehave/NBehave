using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace NBehave.Narrator.Framework
{
	//    A B $T1 C D $T2 E
	//      ^(A B )(.+)( C D )(.+)( E)

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
			TokenStrings = new List<string>();
			string scenarioText = RemoveWhiteSpacesAtBeginning(scenario);
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
			//Title?
			string regexString = BuildRegexString();
			var regex = new Regex(regexString, RegexOptions.IgnoreCase);

			var matches = regex.Matches(scenario);
			if (matches.Count == 1)
			{
				return scenario.Substring(matches[0].Index);
			}
			var first = matches[0].Index;
			var second = matches[1].Index;
			return scenario.Substring(first, second - first);
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
			return regex.Substring(0, regex.Length - 1);
		}
		
		public string ScenarioMessage()
		{
			string scenarioMessageToAdd = string.Empty;
			
			foreach (var row in TokenStrings)
			{
				if (Scenario.IsScenarioTitle(row)==false)
					scenarioMessageToAdd += row + Environment.NewLine;
			}
			return scenarioMessageToAdd;
		}
	}
}