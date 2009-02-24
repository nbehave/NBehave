using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using System.Reflection;
using System.Globalization;

namespace NBehave.Narrator.Framework
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ActionStepsAttribute : Attribute
    { }

    [AttributeUsage(AttributeTargets.Method)]
    public class ActionStepAttribute : Attribute
    {
        public string TokenString { get; private set; }

        public ActionStepAttribute(string tokenString)
        {
            TokenString = tokenString;
        }
    }

    public sealed class GivenAttribute : ActionStepAttribute
    {
        public GivenAttribute(string tokenString)
            : base("Given " + tokenString)
        { }
    }

    public sealed class WhenAttribute : ActionStepAttribute
    {
        public WhenAttribute(string tokenString)
            : base("When " + tokenString)
        { }
    }

    public sealed class ThenAttribute : ActionStepAttribute
    {
        public ThenAttribute(string tokenString)
            : base("Then " + tokenString)
        { }
    }

    public class ActionMethodInfo
    {
        public string TokenString { get; set; }
        public MethodInfo MethodInfo { get; set; }
    }

    public class ActionStepRunner : RunnerBase
    {
        private readonly List<string> _scenarios = new List<string>();
        private ActionStepAlias _actionStepAlias = new ActionStepAlias();

        public ActionCatalog ActionCatalog { get; protected set; }

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
            listener.StoryCreated(string.Empty);
            RunScenarios(results, listener);
            CompileStoryResults(results);
            listener.StoryResults(results);
            listener.ThemeFinished();
            ClearStoryList();
        }

        private void RunScenarios(StoryResults results, IEventListener listener)
        {
            foreach (var scenario in _scenarios)
            {
                var scenarioResult = new ScenarioResults(string.Empty, string.Empty);
                foreach (var row in scenario.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries))
                {
                    listener.StoryMessageAdded(row);
                    try
                    {
                        InvokeTokenString(row);
                    }
                    catch (Exception e)
                    {
                        Exception realException = FindUsefulException(e);
                        scenarioResult.Fail(realException);
                    }
                }
                results.AddResult(scenarioResult);
            }
        }

        private static Exception FindUsefulException(Exception e)
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
            switch (CountTokensInTokenString(method.TokenString))
            {
                case 0: Action a0 = () => method.MethodInfo.Invoke(instance, null);
                    action = a0;
                    break;
                case 1: Action<object> a1 = a => method.MethodInfo.Invoke(instance, new[] { a });
                    action = a1;
                    break;
                case 2: Action<object, object> a2 = (a, b) => method.MethodInfo.Invoke(instance, new[] { a, b });
                    action = a2;
                    break;
                case 3: Action<object, object, object> a3 = (a, b, c) => method.MethodInfo.Invoke(instance, new[] { a, b, c });
                    action = a3;
                    break;
                case 4: Action<object, object, object, object> a4 = (a, b, c, d) => method.MethodInfo.Invoke(instance, new[] { a, b, c, d });
                    action = a4;
                    break;
            }
            return action;
        }

        private IEnumerable<ActionMethodInfo> GetMethodsWithActionStepAttribute(Type actionSteps)
        {
            var methods = from method in actionSteps.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                          where
                                    method.GetCustomAttributes(typeof(ActionStepAttribute), true)
                                    .Length > 0
                                &&
                                    StoryRunnerFilter.MethodNameFiler.IsMatch(method.Name)
                          select new ActionMethodInfo
                          {
                              MethodInfo = method,
                              TokenString = ((ActionStepAttribute)method.GetCustomAttributes(typeof(ActionStepAttribute), true).First()).TokenString
                          };
            return methods;
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

            object action = ActionCatalog.GetAction(tokenString);

            Type actionType = action.GetType().IsGenericType ? action.GetType().GetGenericTypeDefinition() : action.GetType();
            MethodInfo methodInfo = actionType.GetMethod("DynamicInvoke");
            object[] actionParamValues = ActionCatalog.GetParametersForMessage(tokenString);

            methodInfo.Invoke(action, BindingFlags.InvokeMethod, null,
                        new object[] { actionParamValues }, CultureInfo.CurrentCulture);
        }

        public void Load(IEnumerable<string> scenarioLocations)
        {
            foreach (var location in scenarioLocations)
            {
                Stream stream = File.OpenRead(location);
                Load(stream);
            }
        }

        public void Load(Stream stream)
        {
            using (var fs = new StreamReader(stream))
            {
                string scenario = string.Empty;
                string newScenarioShouldStartRowWith = string.Empty;
                bool isFirstRow = true;
                bool notOnFirstRowType = false;
                while (fs.EndOfStream == false)
                {
                    string row = fs.ReadLine() ?? string.Empty;
                    row = TrimInput(row);
                    if (string.IsNullOrEmpty(row) == false)
                    {
                        if (isFirstRow)
                        {
                            isFirstRow = false;
                            newScenarioShouldStartRowWith = row.Split(new char[] { ' ' }).First();
                        }

                        var currentRowStartsWith = row.Split(new char[] { ' ' }).First();
                        if (currentRowStartsWith != newScenarioShouldStartRowWith)
                            notOnFirstRowType = true;
                        if (notOnFirstRowType && row.StartsWith(newScenarioShouldStartRowWith, true, CultureInfo.CurrentCulture))
                        {
                            _scenarios.Add(scenario);
                            scenario = string.Empty;
                            notOnFirstRowType = false;
                        }
                        scenario += row + Environment.NewLine;
                    }
                    if (row.StartsWith(newScenarioShouldStartRowWith, true, CultureInfo.CurrentCulture) == false)
                        notOnFirstRowType = true;
                }
                _scenarios.Add(scenario);
            }
        }

        private string TrimInput(string row)
        {
            var newRow = row.Trim(new[] { '\t', ' ' });

            return newRow;
        }
    }
}
