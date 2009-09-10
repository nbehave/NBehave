using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;

namespace NBehave.Narrator.Framework
{
	public class ActionMethodInfo
	{
		public string ActionType { get; private set; } //Given, When Then etc..
		public MethodInfo MethodInfo { get; private set; }
		public Regex ActionStepMatcher { get; private set; }
		public object Action { get; private set; }
		public IFileMatcher FileMatcher { get; set; }
		
		public ActionMethodInfo()
		{
			FileMatcher = new MatchAllFiles();
		}
		
		public ActionMethodInfo(Regex actionStepMatcher, object action, MethodInfo methodInfo)
			:this(actionStepMatcher, action, methodInfo, string.Empty)
		{ }

		public ActionMethodInfo(Regex actionStepMatcher, object action, MethodInfo methodInfo, string actionType)
			:this()
		{
			ActionStepMatcher = actionStepMatcher;
			Action = action;
			MethodInfo = methodInfo;
			ActionType = actionType;
		}
		
		public ParameterInfo[] ParameterInfo
		{
			get{
				return MethodInfo.GetParameters();
			}
		}

		public List<string> GetParameterNames()
		{
			var names = new List<string>();
			int index = 0;
			string name = ".";
			Regex regex = ActionStepMatcher;
			while (string.IsNullOrEmpty(name) == false)
			{
				name = regex.GroupNameFromNumber(index);
				if (string.IsNullOrEmpty(name) == false && name != index.ToString())
					names.Add(name);
				index++;
			}
			return names;
		}
		
		public bool MatchesFileName(string fileName)
		{
			return FileMatcher.IsMatch(fileName);
		}
	}
}