using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.ReSharper.TaskRunnerFramework;
using NBehave.Narrator.Framework;
using NBehave.Narrator.Framework.Tiny;

namespace NBehave.ReSharper.Plugin.UnitTestRunner
{
    public class NBehaveTaskRunnerListener : EventListener, IDisposable
    {
        private readonly List<KeyValuePair<TinyMessageSubscriptionToken, Type>> _hubSubscriberTokens = new List<KeyValuePair<TinyMessageSubscriptionToken, Type>>();
        private readonly ITinyMessengerHub _hub;
        private readonly IEnumerable<TaskExecutionNode> _nodes;
        private readonly IRemoteTaskServer _server;
        private bool _disposed;

        private string _currentFeatureFile = string.Empty;
        private string _currentScenario = string.Empty;
        private string _currentStep = string.Empty;
        private readonly List<RemoteTask> _currentTasks = new List<RemoteTask>();
        private readonly List<ScenarioResult> _scenarioResults = new List<ScenarioResult>();

        public NBehaveTaskRunnerListener(ITinyMessengerHub hub, IEnumerable<TaskExecutionNode> nodes, IRemoteTaskServer server)
        {
            _hub = hub;
            _nodes = nodes;
            _server = server;
            AddSubscriptions();
        }

        public override void ScenarioResult(ScenarioResult result)
        {
            //Find results to _currentTasks, remove thos found
            // OR iterate through results, find tasts and update them. (Remove all methods below this one)
            _scenarioResults.Add(result);
        }

        private void AddSubscriptions()
        {
            Subscribe<ParsingFileStart>(ParsingFileStart);
            Subscribe<ParsedFeature>(ParsedFeature);
            Subscribe<ParsedScenario>(ParsedScenario);
            Subscribe<ParsedStep>(ParsedStep);
            Subscribe<ParsingFileEnd>(ParsingFileEnd);
        }

        private void ParsingFileStart(ParsingFileStart obj)
        {
            _currentTasks.Clear();
            _currentFeatureFile = obj.Content;
        }

        private void ParsedFeature(ParsedFeature obj)
        {
            //signal step end & scenario end (unless first)

            //FeateTask kanske ska ha namnet på featuren också?

            _currentFeatureFile = obj.Content;
            var node = _nodes
                .Where(_ => _.RemoteTask is FeatureTask)
                .Select(_ => new { Task = _.RemoteTask as FeatureTask, Node = _ })
                .Where(_ => _.Task.FeatureFile == _currentFeatureFile)
                .Select(_ => _.Task)
                .FirstOrDefault();
            SignalTaskStarting(node);
        }

        private void ParsedScenario(ParsedScenario obj)
        {
            //signal step end & scenario end (unless first)
            _currentScenario = obj.Content;

            var node = _nodes
                .Where(_ => _.RemoteTask is NBehaveScenarioTask)
                .Select(_ => new { Task = _.RemoteTask as NBehaveScenarioTask, Node = _ })
                .Where(_ => _.Task.FeatureFile == _currentFeatureFile && _.Task.Scenario == _currentScenario)
                .Select(_ => _.Task)
                .FirstOrDefault();
            SignalTaskStarting(node);
        }

        private void ParsedStep(ParsedStep obj)
        {
            //signal step end (unless first)
            _currentStep = obj.Content;
            var node = _nodes
                .Where(_ => _.RemoteTask is NBehaveStepTask)
                .Select(_ => new { Task = _.RemoteTask as NBehaveStepTask, Node = _ })
                .Where(_ => _.Task.FeatureFile == _currentFeatureFile && _.Task.Scenario == _currentScenario && _.Task.Step == _currentStep)
                .Select(_ => _.Task)
                .FirstOrDefault();
            SignalTaskStarting(node);
        }

        private void SignalTaskStarting(RemoteTask task)
        {
            if (task == null)
                return;
            _currentTasks.Add(task);
            _server.TaskStarting(task);
        }

        private void ParsingFileEnd(ParsingFileEnd obj)
        {
            //signal step end & scenario end & feature end
            _currentFeatureFile = string.Empty;
        }

        private void Subscribe<T>(Action<T> subscriber) where T : class, ITinyMessage
        {
            var token = _hub.Subscribe(subscriber);
            _hubSubscriberTokens.Add(new KeyValuePair<TinyMessageSubscriptionToken, Type>(token, typeof(T)));
        }

        private void RemoveSubscriptions()
        {
            lock (_hubSubscriberTokens)
            {
                foreach (var tokenPair in _hubSubscriberTokens)
                {
                    var token = tokenPair.Key;
                    var type = tokenPair.Value;
                    _hub.Unsubscribe(token, type);
                }
                _hubSubscriberTokens.Clear();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (!_disposed)
                {
                    _disposed = true;
                    RemoveSubscriptions();
                }
            }
        }
    }
}