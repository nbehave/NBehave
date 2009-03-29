using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

namespace NBehave.Narrator.Framework
{
    public class MessageEventData
    {
        public MessageEventData(string type, string message)
        {
            Type = type;
            Message = message;
        }

        public string Type { get; private set; }
        public string Message { get; private set; }
    }

    public class Story
    {
        private readonly ActionCatalog _catalog = new ActionCatalog();
        private readonly LinkedList<ScenarioResults> _scenarioResults;
        private readonly List<Scenario> _scenarios;

        public static event EventHandler<EventArgs<Story>> StoryCreated;
        public static event EventHandler<EventArgs<Scenario>> ScenarioCreated;
        public static event EventHandler<EventArgs<MessageEventData>> MessageAdded;

        public string Title { get; private set; }
        public bool IsDryRun { get; set; }

        public Story(string title)
        {
            Title = title;
            _scenarios = new List<Scenario>();
            _scenarioResults = new LinkedList<ScenarioResults>();

            OnStoryCreated(new EventArgs<Story>(this));
        }

        protected void OnStoryCreated(EventArgs<Story> e)
        {
            if (StoryCreated != null)
                StoryCreated(null, e);
        }

        protected void OnScenarioAdded(EventArgs<Scenario> e)
        {
            if (ScenarioCreated != null)
                ScenarioCreated(null, e);
        }

        private void OnScenarioMessageAdded(object sender, EventArgs<ScenarioMessage> eventArgs)
        {
            OnMessageAdded(sender, new EventArgs<MessageEventData>(new MessageEventData(eventArgs.EventData.Category, eventArgs.EventData.Message)));
        }

        internal void OnMessageAdded(object sender, EventArgs<MessageEventData> eventArgs)
        {
            if (MessageAdded != null)
                MessageAdded(sender, eventArgs);
        }

        public AsAFragment AsA(string role)
        {
            return new AsAFragment(role, this);
        }

        public Scenario WithScenario(string title)
        {
            var scenario = new Scenario(title, this);
            scenario.ScenarioMessageAdded += OnScenarioMessageAdded;
            AddScenario(scenario);

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

        private void InvokeActionBase(string type, string message, object originalAction, Action actionCallback,
                                      params object[] messageParameters)
        {
            string formatString = BuildFormatString(message, messageParameters);
            string messageToUse = _catalog.BuildMessage(message, messageParameters);

            var fullMessageParameters = new List<object> { type, messageToUse };
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
                    OnScenarioMessageAdded(this, new EventArgs<ScenarioMessage>(new ScenarioMessage(type, string.Format(formatString + " - FAILED", fullMessageParameters.ToArray()))));
                    throw;
                }
            }
            OnScenarioMessageAdded(this, new EventArgs<ScenarioMessage>(new ScenarioMessage(type, string.Format(formatString, fullMessageParameters.ToArray()))));
            CatalogAction(message, originalAction);
        }

        internal void InvokeAction(string type, string message, Action action)
        {
            InvokeActionBase(type, message, action, action);
        }

        private string BuildFormatString(string message, ICollection<object> args)
        {
            if ((message.IndexOf(ActionCatalog.TokenPrefix) == -1)
                && (!(_catalog.ActionExists(message) && _catalog.CatalogedActionIsTokenized(message))))
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

        internal void InvokeAction<TArg0>(string type, string message, Action<TArg0> action, TArg0 arg0)
        {
            InvokeActionBase(type, message, action, () => action(arg0), new object[] { arg0 });
        }

        internal void InvokeAction<TArg0, TArg1>(string type, string message, Action<TArg0, TArg1> action, TArg0 arg0,
                                                 TArg1 arg1)
        {
            InvokeActionBase(type, message, action, () => action(arg0, arg1), new object[] { arg0, arg1 });
        }

        internal void InvokeAction<TArg0, TArg1, TArg2>(string type, string message, Action<TArg0, TArg1, TArg2> action,
                                                        TArg0 arg0, TArg1 arg1, TArg2 arg2)
        {
            InvokeActionBase(type, message, action, () => action(arg0, arg1, arg2),
                             new object[] { arg0, arg1, arg2 });
        }

        internal void InvokeAction<TArg0, TArg1, TArg2, TArg3>(string type, string message,
                                                               Action<TArg0, TArg1, TArg2, TArg3> action, TArg0 arg0,
                                                               TArg1 arg1, TArg2 arg2, TArg3 arg3)
        {
            InvokeActionBase(type, message, action, () => action(arg0, arg1, arg2, arg3),
                             new object[] { arg0, arg1, arg2, arg3 });
        }

        internal void InvokeActionFromCatalog(string type, string message)
        {
            try
            {
                ValidateActionExists(message);
                if (IsDryRun)
                {
                    InvokeActionBase(type, message, null, null, _catalog.GetParametersForMessage(message));
                }
                else
                {
                    object action = GetActionFromCatalog(message);
                    Type actionType = action.GetType().IsGenericType
                                          ? action.GetType().GetGenericTypeDefinition()
                                          : action.GetType();
                    MethodInfo methodInfo = actionType.GetMethod("DynamicInvoke");
                    object[] actionParamValues = _catalog.GetParametersForMessage(message);
                    InvokeActionBase(type, message, action,
                                     () => methodInfo.Invoke(action, BindingFlags.InvokeMethod, null,
                                                             new object[] { actionParamValues },
                                                             CultureInfo.CurrentCulture), actionParamValues);
                }
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

            var action = (Action<TArg0>)GetActionFromCatalog(message);

            InvokeAction(type, message, action, arg0);
        }

        internal void InvokeActionFromCatalog<TArg0, TArg1>(string type, string message, TArg0 arg0, TArg1 arg1)
        {
            ValidateActionExists(message);

            var action = (Action<TArg0, TArg1>)GetActionFromCatalog(message);

            InvokeAction(type, message, action, arg0, arg1);
        }

        internal void InvokeActionFromCatalog<TArg0, TArg1, TArg2>(string type, string message, TArg0 arg0, TArg1 arg1,
                                                                   TArg2 arg2)
        {
            ValidateActionExists(message);

            var action = (Action<TArg0, TArg1, TArg2>)GetActionFromCatalog(message);

            InvokeAction(type, message, action, arg0, arg1, arg2);
        }

        internal void InvokeActionFromCatalog<TArg0, TArg1, TArg2, TArg3>(string type, string message, TArg0 arg0,
                                                                          TArg1 arg1, TArg2 arg2, TArg3 arg3)
        {
            ValidateActionExists(message);

            var action = (Action<TArg0, TArg1, TArg2, TArg3>)GetActionFromCatalog(message);

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
            OnScenarioAdded(new EventArgs<Scenario>(scenario));
            _scenarioResults.AddLast(new ScenarioResults(Title, scenario.Title));
        }
    }
}