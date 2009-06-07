using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;

namespace NBehave.Narrator.Framework
{
	public class ActionMethodInfo
	{
		public string TokenString { get; set; }
		public MethodInfo MethodInfo { get; set; }
	}

	public class ActionStepRunner : RunnerBase
	{
		private readonly List<string> _scenarios = new List<string>();
		private readonly ActionStepAlias _actionStepAlias = new ActionStepAlias();

		public ActionCatalog ActionCatalog { get; private set; }

		public ActionStepRunner()
		{
			ActionCatalog = new ActionCatalog();
			StoryRunnerFilter = new StoryRunnerFilter();
		}

		protected override void ParseAssembly(Assembly assembly)
		{
			FindActionSteps(assembly);
		}

		protected override void RunStories(StoryResults results, IEventListener listener)
		{
			listener.ThemeStarted(string.Empty);
			RunScenarios(results, listener);
			listener.StoryResults(results);
			listener.ThemeFinished();
			ClearStoryList();
		}

		private void RunScenarios(StoryResults storyResults, IEventListener listener)
		{
			var story = new Story(string.Empty) { IsDryRun = IsDryRun };
			int scenarioCounter = 0;
			foreach (string scenarioText in _scenarios)
			{
				scenarioCounter++;

				RunScenario(story, scenarioText, storyResults, listener, scenarioCounter);
			}
		}

		private void RunScenario(Story story, string scenarioText, StoryResults storyResults, IEventListener listener,
		                         int scenarioCounter)
		{
			var textToTokenStringsParser = new TextToTokenStringsParser(_actionStepAlias);

			textToTokenStringsParser.ParseScenario(scenarioText);
			string scenarioTitle = string.Format("Scenario {0}", scenarioCounter);
			var scenarioResult = new ScenarioResults(string.Empty, scenarioTitle);
			foreach (var row in textToTokenStringsParser.TokenStrings)
			{
				RunScenario(row, scenarioResult);
			}
			var scenario = new Scenario(scenarioResult.ScenarioTitle, story);
			story.AddScenario(scenario);
			listener.ScenarioMessageAdded(scenarioText);
			storyResults.AddResult(scenarioResult);
		}

		private void RunScenario(string row, ScenarioResults scenarioResult)
		{
			try
			{
				if (Scenario.IsScenarioTitle(row))
				{
					scenarioResult.ScenarioTitle = Scenario.GetTitle(row);
				}
				else
				{
					if (ActionCatalog.ActionExists(row) == false)
						scenarioResult.Pend(string.Format("No matching Action found for \"{0}\"", row));
					else
						InvokeTokenString(row);
				}
			}
			catch (Exception e)
			{
				Exception realException = FindUsefulException(e);
				scenarioResult.Fail(realException);
			}
		}

		private Exception FindUsefulException(Exception e)
		{
			Exception realException = e;
			while (realException != null && realException.GetType() == typeof(TargetInvocationException))
			{
				realException = realException.InnerException;
			}
			if (realException == null)
				return e;
			return realException;
		}

		private void FindActionSteps(Assembly assembly)
		{
			foreach (Type t in assembly.GetExportedTypes())
			{
				if (t.GetCustomAttributes(typeof(ActionStepsAttribute), false).Length > 0)
				{
					if (StoryRunnerFilter.NamespaceFilter.IsMatch(t.Namespace) &&
					    StoryRunnerFilter.ClassNameFilter.IsMatch(t.Name))
					{
						FindActionStepMethods(t);
					}
				}
			}
		}

		private void FindActionStepMethods(Type actionSteps)
		{
			var instance = Activator.CreateInstance(actionSteps);
			var methods = GetMethodsWithActionStepAttribute(actionSteps);
			foreach (var method in methods)
			{
				object action = CreateAction(instance, method);
				ActionCatalog.Add(method.TokenString, action);
				RegisterAliases(method, action);
			}
		}

		private void RegisterAliases(ActionMethodInfo method, object action)
		{
			IEnumerable<string> tokenStringAliases = _actionStepAlias.GetAliasForTokenString(method.TokenString);
			foreach (var tokenAlias in tokenStringAliases)
			{
				ActionCatalog.Add(tokenAlias, action);
			}
		}

		private object CreateAction(object instance, ActionMethodInfo method)
		{
			object action = null;
			MethodInfo methodInfo = method.MethodInfo;
			switch (CountTokensInTokenString(method.TokenString))
			{
				case 0:
					action = GetActionWithNoParameters(method, instance);
					break;
				case 1:
					action = GetActionForOneParameter(instance, methodInfo);
					break;
				case 2:
					action = GetActionForTwoParameters(instance, methodInfo);
					break;
				case 3:
					action = GetActionForThreeParameters(instance, methodInfo);
					break;
				case 4:
					action = GetActionForFourParameters(instance, methodInfo);
					break;
			}
			return action;
		}

		private object GetActionWithNoParameters(ActionMethodInfo method, object instance)
		{
			Action action = () => method.MethodInfo.Invoke(instance, null);
			return action;
		}

		private object GetActionForOneParameter(object instance, MethodInfo methodInfo)
		{
			Action<object> action = a =>
				methodInfo.Invoke(instance, new[]
				                  {
				                  	ChangeType(methodInfo, a, 0),
				                  });
			return action;
		}

		private object GetActionForTwoParameters(object instance, MethodInfo methodInfo)
		{
			Action<object, object> action = (a, b) =>
				methodInfo.Invoke(instance, new[]
				                  {
				                  	ChangeType(methodInfo,a,0),
				                  	ChangeType(methodInfo,b,1),
				                  });
			return action;
		}

		private object GetActionForThreeParameters(object instance, MethodInfo methodInfo)
		{
			Action<object, object, object> action = (a, b, c) =>
				methodInfo.Invoke(instance, new[]
				                  {
				                  	ChangeType(methodInfo,a,0),
				                  	ChangeType(methodInfo,b,1),
				                  	ChangeType(methodInfo,c,2),
				                  });
			return action;
		}

		private object GetActionForFourParameters(object instance, MethodInfo methodInfo)
		{
			Action<object, object, object, object> action = (a, b, c, d) =>
				methodInfo.Invoke(instance, new[]
				                  {
				                  	ChangeType(methodInfo,a,0),
				                  	ChangeType(methodInfo,b,1),
				                  	ChangeType(methodInfo,c,2),
				                  	ChangeType(methodInfo,d,3)
				                  });
			return action;
		}

		private object ChangeType(MethodInfo methodInfo, object parameter, int parameterIndex)
		{
			return Convert.ChangeType(parameter, methodInfo.GetParameters()[parameterIndex].ParameterType);
		}

		private IEnumerable<ActionMethodInfo> GetMethodsWithActionStepAttribute(Type actionSteps)
		{
			IEnumerable<MethodInfo> methodsWithActionStepAttribute = GetAllMethodsWithActionStepAttribute(actionSteps);
			List<ActionMethodInfo> allMethods = GetAllMethodsWithActionStepAttribute(methodsWithActionStepAttribute);
			return allMethods;
		}

		private List<ActionMethodInfo> GetAllMethodsWithActionStepAttribute(IEnumerable<MethodInfo> methodsWithActionStepAttribute)
		{
			var allMethods = new List<ActionMethodInfo>();
			foreach (var method in methodsWithActionStepAttribute)
			{
				foreach (ActionStepAttribute actionStep in method.GetCustomAttributes(typeof(ActionStepAttribute), true))
				{
					string tokenString = GetTokenString(actionStep, method);
					allMethods.Add(new ActionMethodInfo{MethodInfo=method, TokenString = tokenString});
				}
			}
			return allMethods;
		}
		
		private string GetTokenString(ActionStepAttribute actionStep, MethodInfo method)
		{
			if (string.IsNullOrEmpty(actionStep.TokenString))
			{
				string methodName = method.Name.Replace('_', ' ');
				ParameterInfo[] parameters = method.GetParameters();
				
				foreach(var param in parameters)
				{
					var paramName = param.Name;
					int pos = methodName.IndexOf(paramName);
					if (pos>0)
						methodName = methodName.Substring(0, pos) + "$" + methodName.Substring(pos);
				}
				return methodName;
			}
			return actionStep.TokenString;
		}

		private IEnumerable<MethodInfo> GetAllMethodsWithActionStepAttribute(Type actionSteps)
		{
			return from method in
				actionSteps.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
				where
				method.GetCustomAttributes(typeof (ActionStepAttribute), true)
				.Length > 0
				&&
				StoryRunnerFilter.MethodNameFiler.IsMatch(method.Name)
				select method;
		}

		private int CountTokensInTokenString(string tokenString)
		{
			string[] words = tokenString.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
			return words.Where(s => s.StartsWith(ActionCatalog.TokenPrefix.ToString())).Count();
		}

		public void InvokeTokenString(string tokenString)
		{
			if (ActionCatalog.ActionExists(tokenString) == false)
				throw new ArgumentException(string.Format("cannot find Token string '{0}'", tokenString));

			object action = ActionCatalog.GetAction(tokenString).Action;

			Type actionType = action.GetType().IsGenericType
				? action.GetType().GetGenericTypeDefinition()
				: action.GetType();
			MethodInfo methodInfo = actionType.GetMethod("DynamicInvoke");
			object[] actionParamValues = ActionCatalog.GetParametersForMessage(tokenString);

			methodInfo.Invoke(action, BindingFlags.InvokeMethod, null,
			                  new object[] { actionParamValues }, CultureInfo.CurrentCulture);
		}

		public void Load(IEnumerable<string> scenarioLocations)
		{
			foreach (var location in scenarioLocations)
			{
				string[] files;
				if (Path.IsPathRooted(location))
					files = Directory.GetFiles(Path.GetDirectoryName(location), Path.GetFileName(location));
				else
					files = Directory.GetFiles(".", location);
				foreach (var file in files)
				{
					Stream stream = File.OpenRead(file);
					Load(stream);
				}
			}
		}

		public void Load(Stream stream)
		{
			var scenarioTextParser = new TextToTokenStringsParser(_actionStepAlias);
			using (var fs = new StreamReader(stream))
				scenarioTextParser.ParseScenario(fs.ReadToEnd());
			var tokenStringsToScenarioParser = new TokenStringsToScenarioParser();
			tokenStringsToScenarioParser.ParseTokensToScenarios(scenarioTextParser.TokenStrings);
			List<string> scenarios = tokenStringsToScenarioParser.Scenarios;
			_scenarios.AddRange(scenarios);
		}
	}
}