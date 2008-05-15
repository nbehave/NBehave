using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using NBehave.Narrator.Framework;
using System.Globalization;

namespace NBehave.Narrator.Framework
{
    public class Story
    {
        private readonly string _title = null;
        private readonly IMessageProvider _messageProvider = null;
        private readonly List<Scenario> _scenarios = null;
        private readonly ActionCatalog _catalog = new ActionCatalog();
        private readonly LinkedList<ScenarioResults> _scenarioResults = null;
        private bool _isDryRun;

        public static event EventHandler<EventArgs<Story>> StoryCreated;

        protected static void OnStoryCreated(EventArgs<Story> e)
        {
            EventHandler<EventArgs<Story>> handler = StoryCreated;
            if (handler != null)
                handler(null, e);
        }

        public Story(string title) : this(title, MessageProviderRegistry.GetInstance()) { }

        public Story(string title, IMessageProvider messageProvider)
        {
            _title = title;
            _messageProvider = messageProvider;
            _scenarios = new List<Scenario>();
            _scenarioResults = new LinkedList<ScenarioResults>();

            OnStoryCreated(new EventArgs<Story>(this));

            if (!string.IsNullOrEmpty(_title))
            {
                _messageProvider.AddMessage("Story: " + title);
            }
        }

        public string Title
        {
            get { return _title; }
        }

        public bool IsDryRun
        {
            get { return _isDryRun; }
            set { _isDryRun = value; }
        }

        public AsAFragment AsA(string role)
        {
            return new AsAFragment(role, this);
        }

        public Scenario WithScenario(string title)
        {
            Scenario scenario = new Scenario(title, this);

            AddScenario(scenario);

            AddMessage("");
            AddMessage(string.Format("\tScenario {0}: {1}", _scenarios.Count, scenario.Title));

            return scenario;
        }

        public void CompileResults(StoryResults results)
        {
            foreach (ScenarioResults result in _scenarioResults)
            {
                results.AddResult(result);
            }
        }

        internal void PendLastScenarioResults(string reason)
        {
            _scenarioResults.Last.Value.Pend(reason);
        }

        internal void AddMessage(string message)
        {
            _messageProvider.AddMessage(message);
        }

        private void InvokeActionBase(string type, string message, object originalAction, Action actionCallback, params object[] messageParameters)
        {
            string formatString = BuildFormatString(message, messageParameters);
            string messageToUse = _catalog.BuildMessage(message, messageParameters);

            List<object> fullMessageParameters = new List<object>();
            fullMessageParameters.Add(type);
            fullMessageParameters.Add(messageToUse);
            fullMessageParameters.AddRange(messageParameters);


            if (!IsDryRun)
            {
                try
                {
                    actionCallback();
                }
                catch (Exception ex)
                {
                    _scenarioResults.Last.Value.Fail(ex);
                    AddMessage(string.Format(formatString + " - FAILED", fullMessageParameters.ToArray()));
                    throw;
                }
            }
            AddMessage(string.Format(formatString, fullMessageParameters.ToArray()));
            CatalogAction(message, originalAction);
        }

        internal void InvokeAction(string type, string message, Action action)
        {
            InvokeActionBase(type, message, action, delegate { action(); });
        }


        private string BuildFormatString(string message, object[] args)
        {
            string messageToUse = _catalog.BuildMessage(message, args);
            if ((message.IndexOf(ActionCatalog.TokenPrefix) == -1)
            && (!(_catalog.ActionExists(message) && _catalog.CatalogedActionIsTokenized(message))))
            {
                if (args.Length == 0)
                    return "{0} {1}";
                else if (args.Length == 1)
                    return "{0} {1}: {2}";
                else
                {
                    string formatString = "{0} {1}: (";
                    for (int i = 0; i < args.Length; i++)
                        formatString += "{" + (i + 2).ToString() + "}, ";
                    return formatString.Remove(formatString.Length - 2) + ")";
                }
            }
            return "{0} {1}";
        }

        internal void InvokeAction<TArg0>(string type, string message, Action<TArg0> action, TArg0 arg0)
        {
            InvokeActionBase(type, message, action, delegate { action(arg0); }, new object[] { arg0 });
        }

        internal void InvokeAction<TArg0, TArg1>(string type, string message, Action<TArg0, TArg1> action, TArg0 arg0, TArg1 arg1)
        {
            InvokeActionBase(type, message, action, delegate { action(arg0, arg1); }, new object[] { arg0, arg1 });
        }

        internal void InvokeAction<TArg0, TArg1, TArg2>(string type, string message, Action<TArg0, TArg1, TArg2> action, TArg0 arg0, TArg1 arg1, TArg2 arg2)
        {
            InvokeActionBase(type, message, action, delegate { action(arg0, arg1, arg2); }, new object[] { arg0, arg1, arg2 });
        }

        internal void InvokeAction<TArg0, TArg1, TArg2, TArg3>(string type, string message, Action<TArg0, TArg1, TArg2, TArg3> action, TArg0 arg0, TArg1 arg1, TArg2 arg2, TArg3 arg3)
        {
            InvokeActionBase(type, message, action, delegate { action(arg0, arg1, arg2, arg3); }, new object[] { arg0, arg1, arg2, arg3 });
        }

        internal void InvokeActionFromCatalog(string type, string message)
        {
            try
            {
                ValidateActionExists(message);
                object action = GetActionFromCatalog(message);
                Type actionType = action.GetType().GetGenericTypeDefinition();
                MethodInfo methodInfo = actionType.GetMethod("DynamicInvoke");
                object[] actionParamValues = _catalog.GetParametersForMessage(message);
                InvokeActionBase(type, message, action,
                    delegate
                    {
                        methodInfo.Invoke(action, BindingFlags.InvokeMethod, null,
                            new object[] { actionParamValues }, CultureInfo.CurrentCulture);
                    }, actionParamValues);
            }
            catch (Exception e)
            {
                ScenarioResults result = _scenarioResults.Last.Value;
                result.Fail(e);
            }
        }

        internal void InvokeActionFromCatalog<TArg0>(string type, string message, TArg0 arg0)
        {
            ValidateActionExists(message);

            Action<TArg0> action = (Action<TArg0>)GetActionFromCatalog(message);

            InvokeAction(type, message, action, arg0);
        }

        internal void InvokeActionFromCatalog<TArg0, TArg1>(string type, string message, TArg0 arg0, TArg1 arg1)
        {
            ValidateActionExists(message);

            Action<TArg0, TArg1> action = (Action<TArg0, TArg1>)GetActionFromCatalog(message);

            InvokeAction(type, message, action, arg0, arg1);
        }

        internal void InvokeActionFromCatalog<TArg0, TArg1, TArg2>(string type, string message, TArg0 arg0, TArg1 arg1, TArg2 arg2)
        {
            ValidateActionExists(message);

            Action<TArg0, TArg1, TArg2> action = (Action<TArg0, TArg1, TArg2>)GetActionFromCatalog(message);

            InvokeAction(type, message, action, arg0, arg1, arg2);
        }

        internal void InvokeActionFromCatalog<TArg0, TArg1, TArg2, TArg3>(string type, string message, TArg0 arg0, TArg1 arg1, TArg2 arg2, TArg3 arg3)
        {
            ValidateActionExists(message);

            Action<TArg0, TArg1, TArg2, TArg3> action = (Action<TArg0, TArg1, TArg2, TArg3>)GetActionFromCatalog(message);

            InvokeAction(type, message, action, arg0, arg1, arg2, arg3);
        }

        private void CatalogAction(string message, object action)
        {
            if (_catalog.ActionExists(message))
                return;
            _catalog.Add(message, action);
        }

        private void ValidateActionExists(string message)
        {
            if (!_catalog.ActionExists(message) && !IsDryRun)
                throw new ActionMissingException(string.Format("Action missing for action '{0}'.", message));
        }

        private object GetActionFromCatalog(string message)
        {
            if (IsDryRun)
                return null;

            return _catalog.GetAction(message);
        }

        private void AddScenario(Scenario scenario)
        {
            _scenarios.Add(scenario);
            _scenarioResults.AddLast(new ScenarioResults(Title, scenario.Title));
        }

    }
}