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

        public string Title { get; set; }
        public string Narrative { get; set; }
        public bool IsDryRun { get; set; }

        private Scenario _currentScenario;

        public Story(string title)
        {
            Title = title;
            Narrative = string.Empty;
            _scenarios = new List<Scenario>();
            _scenarioResults = new LinkedList<ScenarioResults>();

            OnStoryCreated(new EventArgs<Story>(this));
        }

        private void OnStoryCreated(EventArgs<Story> e)
        {
            if (StoryCreated != null)
                StoryCreated(null, e);
        }

        private void OnScenarioAdded(EventArgs<Scenario> e)
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

        public ScenarioBuilder WithScenario(string title)
        {
            var scenario = new Scenario(title, this);
            scenario.ScenarioMessageAdded += OnScenarioMessageAdded;
            AddScenario(scenario);
            _currentScenario = scenario;
            var scenarioBuilder = new ScenarioBuilder(scenario, this);
            return scenarioBuilder;
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

        internal bool CanAddMessage
        {
            get { return !_currentScenario.IsPending || IsDryRun; }
        }

        private void SendFailedMessageEvent(string type, string message, params object[] messageParameters)
        {
            var fullMessageParameters = GetFullMessageParameters(type, message, messageParameters);
            OnScenarioMessageAdded(this, new EventArgs<ScenarioMessage>(
                new ScenarioMessage(type,
                                    string.Format(GetFormatStringForParameters(message, messageParameters)
                                                  + " - FAILED",
                                                  fullMessageParameters.ToArray()))));
        }

        private void SendMessageEvent(string type, string message, params object[] messageParameters)
        {
            var fullMessageParameters = GetFullMessageParameters(type, message, messageParameters);
            OnScenarioMessageAdded(this, new EventArgs<ScenarioMessage>(
                new ScenarioMessage(type,
                                    string.Format(GetFormatStringForParameters(message, messageParameters),
                                                  fullMessageParameters.ToArray()))));
        }

        private string GetFormatStringForParameters(string message, params object[] messageParameters)
        {
            return _catalog.BuildFormatString(message, messageParameters);
        }

        List<object> GetFullMessageParameters(string type, string message, params object[] messageParameters)
        {
            string messageToUse = _catalog.BuildMessage(message, messageParameters);
            var fullMessageParameters = new List<object> { type, messageToUse };
            fullMessageParameters.AddRange(messageParameters);
            return fullMessageParameters;
        }

        private void InvokeActionBase(string type, string message, object originalAction, Action actionCallback,
                                      params object[] messageParameters)
        {
            if (CanAddMessage)
            {
                if (!IsDryRun)
                {
                    try
                    {
                        actionCallback();
                    }
                    catch (Exception ex)
                    {
                        _scenarioResults.Last.Value.Fail(ex);
                        SendFailedMessageEvent(type, message, messageParameters);
                        throw;
                    }
                }
                CatalogAction(message, originalAction);
            }
            SendMessageEvent(type, message, messageParameters);
        }

        internal void InvokeAction(string type, string message, Action action)
        {
            InvokeActionBase(type, message, action, action);
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
                if (_currentScenario.IsPending)
                {
                    SendMessageEvent(type, message);
                    return;
                }

                if (IsDryRun)
                {
                    var parameters = new object[0];
                    if (_catalog.ActionExists(message))
                        parameters = _catalog.GetParametersForMessage(message);
                    InvokeActionBase(type, message, null, null, new object[0]);
                }
                else
                {

                    if (_catalog.ActionExists(message) == false)
                        SendMessageEvent(type, message + " - FAILED");
                    ValidateActionExists(message);

                    object action = GetActionFromCatalog(message);
                    object[] actionParamValues = _catalog.GetParametersForMessage(message);
                    Type actionType = action.GetType().IsGenericType
                        ? action.GetType().GetGenericTypeDefinition()
                        : action.GetType();
                    MethodInfo methodInfo = actionType.GetMethod("DynamicInvoke");
                    InvokeActionBase(type, message, action,
                                     () => methodInfo.Invoke(action, BindingFlags.InvokeMethod, null,
                                                             new object[] { actionParamValues },
                                                             CultureInfo.CurrentCulture), new object[0]); 
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
            if (_currentScenario.IsPending)
            {
                SendMessageEvent(type, message, arg0);
                return;
            }
            ValidateActionExists(message);
            var action = (Action<TArg0>)GetActionFromCatalog(message);
            InvokeAction(type, message, action, arg0);
        }

        internal void InvokeActionFromCatalog<TArg0, TArg1>(string type, string message, TArg0 arg0, TArg1 arg1)
        {
            if (_currentScenario.IsPending)
            {
                SendMessageEvent(type, message, arg0, arg1);
                return;
            }

            ValidateActionExists(message);
            var action = (Action<TArg0, TArg1>)GetActionFromCatalog(message);
            InvokeAction(type, message, action, arg0, arg1);
        }

        internal void InvokeActionFromCatalog<TArg0, TArg1, TArg2>(string type, string message, TArg0 arg0, TArg1 arg1,
                                                                   TArg2 arg2)
        {
            if (_currentScenario.IsPending)
            {
                SendMessageEvent(type, message, arg0, arg1, arg2);
                return;
            }
            ValidateActionExists(message);
            var action = (Action<TArg0, TArg1, TArg2>)GetActionFromCatalog(message);
            InvokeAction(type, message, action, arg0, arg1, arg2);
        }

        internal void InvokeActionFromCatalog<TArg0, TArg1, TArg2, TArg3>(string type, string message, TArg0 arg0,
                                                                          TArg1 arg1, TArg2 arg2, TArg3 arg3)
        {
            if (_currentScenario.IsPending)
            {
                SendMessageEvent(type, message, arg0, arg1, arg2, arg3);
                return;
            }

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

            ActionValue actionValue = _catalog.GetAction(message) ?? new ActionValue();
            return actionValue.Action;
        }

        internal void AddScenario(Scenario scenario)
        {
            _scenarios.Add(scenario);
            OnScenarioAdded(new EventArgs<Scenario>(scenario));
            _scenarioResults.AddLast(new ScenarioResults(Title, scenario.Title));
        }
    }
}