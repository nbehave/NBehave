using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace NBehave.Narrator.Framework
{
	public class ActionCatalog
	{
		public const char TokenPrefix = '$';

		private readonly List<ActionMethodInfo> _actions = new List<ActionMethodInfo>();

		[Obsolete("Use Add(Regex actionMatch, object action)")]
		public void Add(string tokenString, object action, MethodInfo methodInfo)
		{
			if (ActionExists(new ActionStepText(tokenString, "")))
				return;
			var regex = GetRegexForActionKey(tokenString);
			Add(new ActionMethodInfo(regex, action, methodInfo, null));
		}

		public void Add(ActionMethodInfo actionValue)
		{
			_actions.Add(actionValue);
		}

		public bool ActionExists(string text)
		{
			return ActionExists(new ActionStepText(text,""));
		}
		
		public bool ActionExists(ActionStepText actionStepText)
		{
			return (FindMatchingAction(actionStepText) != null);
		}

		public string BuildFormatString(string message, ICollection<object> args)
		{
			if ((message.IndexOf(TokenPrefix) == -1))
			{
				if (args.Count == 0)
					return "{0} {1}";
				if (args.Count == 1)
					return "{0} {1}: {2}";
				string formatString = "{0} {1}: (";
				for (int i = 0; i < args.Count; i++)
					formatString += "{" + (i + 2) + "}, ";
				return formatString.Remove(formatString.Length - 2) + ")";
			}
			return "{0} {1}";
		}

		public object[] GetParametersForActionStepText(ActionStepText actionStepText)
		{
			ActionMethodInfo action = GetAction(actionStepText);
			List<string> paramNames = GetParameterNames(action);
			ParameterInfo[] args = action.ParameterInfo;
			var values = new object[args.Length];

			Match match = action.ActionStepMatcher.Match(actionStepText.Text);
			for (int argNumber = 0; argNumber < paramNames.Count(); argNumber++)
			{
				var strParam = match.Groups[paramNames[argNumber]].Value;
				if (args[argNumber].ParameterType.IsArray)
				{
					var strParamAsArray = strParam.Replace(Environment.NewLine, "\n")
						.Split(new[] { ',' });
					for (int i =0; i < strParamAsArray.Length; i++)
					{
						if (string.IsNullOrEmpty(strParamAsArray[i]) == false)
							strParamAsArray[i] = strParamAsArray[i].Trim();
					}
					while (string.IsNullOrEmpty(strParamAsArray.Last()))
						strParamAsArray = strParamAsArray.Take(strParamAsArray.Length - 1).ToArray();
					values[argNumber] = Convert.ChangeType(strParamAsArray, args[argNumber].ParameterType); //converts string to an instance of args[argNumber]
				}
				else
					values[argNumber] = Convert.ChangeType(strParam, args[argNumber].ParameterType); //converts string to an instance of args[argNumber]
			}
			return values;
		}

		public ActionMethodInfo GetAction(ActionStepText message)
		{
			return FindMatchingAction(message);
		}

		public string BuildMessage(string message, object[] parameters)
		{
			string resultString = message;
			string[] tokens = GetTokensInMessage(message);
			if (tokens.Length > 0 && tokens.Length != parameters.Length)
				throw new ArgumentException(string.Format("message has {0} tokens and there are {1} parameters", tokens.Length, parameters.Length));
			for (int i = 0; i < tokens.Length; i++)
			{
				resultString = resultString.Replace(tokens[i], parameters[i].ToString());
			}

			return resultString;
		}

		private ActionMethodInfo FindMatchingAction(ActionStepText actionStepText)
		{
			string message = actionStepText.Text;
			ActionMethodInfo matchedAction = null;
			int lengthOfMatch = -1;
			foreach (ActionMethodInfo action in _actions)
			{
				Regex regex = action.ActionStepMatcher;
				var match = regex.Match(message);
				if (match.Success)
				{
					if (MatchesFileName(action, actionStepText)
					    && match.Value.Length > lengthOfMatch)
					{
						lengthOfMatch = match.Value.Length;
						matchedAction = action;
					}
				}
			}
			return matchedAction;
		}
		
		private bool MatchesFileName(ActionMethodInfo action, ActionStepText actionStepText)
		{
			return action.MatchesFileName(actionStepText.FromFile);
		}

		private List<string> GetParameterNames(ActionMethodInfo actionValue)
		{
			return actionValue.GetParameterNames();
		}

		private string[] GetTokensInMessage(string message)
		{
			var tokens = new List<string>();

		    var matches = Regex.Matches(message, ActionStepConverterExtensions.TokenRegexPattern);
			foreach (var match in matches)
			{
				tokens.Add(match.ToString());
			}
			return tokens.ToArray();
		}

		private Regex GetRegexForActionKey(string actionKey)
		{
			return actionKey.AsRegex();
		}
	}
}
