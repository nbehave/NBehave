using System;
using System.Collections.Generic;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.TaskRunnerFramework;
using JetBrains.Util;
using NBehave.EventListeners.CodeGeneration;
using NBehave.ReSharper.Plugin.UnitTestRunner;
using NUnit.Framework;
using Rhino.Mocks;

namespace NBehave.ReSharper.Plugin.Specifications
{
    [TestFixture]
    public abstract class ResharperResultPublisherSpec
    {
        private const string Source = @"X:\Code\Proj\Features\Cool.feature";
        private const string ScenarioTitle = "scenario title";

        private IRemoteTaskServer server;
        private NBehave.Feature feature;
        private ResharperResultPublisher resultPublisher;
        private NBehaveStepTask task;
        private NBehaveScenarioTask scenarioTask;

        [SetUp]
        public void SetUp()
        {
            var codeGeneration = new CodeGenEventListener();
            server = MockRepository.GenerateMock<IRemoteTaskServer>();

            var featureFile = MockRepository.GenerateStub<IProjectFile>();
            featureFile.Stub(_ => _.Location).Return(new FileSystemPath(Source));
            scenarioTask = new NBehaveScenarioTask(featureFile, ScenarioTitle);
            var scenarioTaskNode = new TaskExecutionNode(null, scenarioTask);

            task = new NBehaveStepTask(featureFile, ScenarioTitle, "Given something");
            var stepTaskNode = new TaskExecutionNode(scenarioTaskNode, task);
            var nodes = new List<TaskExecutionNode> { scenarioTaskNode, stepTaskNode };
            resultPublisher = new ResharperResultPublisher(nodes, server, codeGeneration);

            feature = new NBehave.Feature("feature title");
            Because_of();
        }

        protected abstract void Because_of();

        [Ignore("Fix!")]
        public class When_all_steps_passes : ResharperResultPublisherSpec
        {
            protected override void Because_of()
            {
                var result = new ScenarioResult(feature, ScenarioTitle);
                result.AddActionStepResult(new StepResult(new StringStep("Given", "something", Source), new Passed()));
                resultPublisher.Notify(result);
            }

            [Test]
            public void Should_notify_of_passed_step()
            {
                server.AssertWasCalled(_ => _.TaskFinished(task, "", TaskResult.Success));
            }

            [Test]
            public void Should_notify_of_passed_scenario()
            {
                server.AssertWasCalled(_ => _.TaskFinished(scenarioTask, "", TaskResult.Success));
            }
        }

        [Ignore("Fix!")]
        public class When_step_Fails : ResharperResultPublisherSpec
        {
            protected override void Because_of()
            {
                var result = new ScenarioResult(feature, ScenarioTitle);
                result.AddActionStepResult(new StepResult(new StringStep("Given", "something", Source), new Failed(new ArgumentNullException("wtf!"))));
                resultPublisher.Notify(result);
            }

            [Test]
            public void Should_notify_of_failed_step()
            {
                server.AssertWasCalled(_ => _.TaskException(Arg<RemoteTask>.Is.Same(task), Arg<TaskException[]>.Is.NotNull));
                server.AssertWasCalled(_ => _.TaskFinished(task, "System.ArgumentNullException: Value cannot be null.\r\nParameter name: wtf!", TaskResult.Error));
            }

            [Test]
            public void Should_notify_of_failed_scenario()
            {
                server.AssertWasCalled(_ => _.TaskException(Arg<RemoteTask>.Is.Same(scenarioTask), Arg<TaskException[]>.Is.NotNull));
                server.AssertWasCalled(_ => _.TaskFinished(scenarioTask, "System.ArgumentNullException: Value cannot be null.\r\nParameter name: wtf!", TaskResult.Error));
            }
        }

        [Ignore("Fix!")]
        public class When_step_is_pending : ResharperResultPublisherSpec
        {
            protected override void Because_of()
            {
                var result = new ScenarioResult(feature, ScenarioTitle);
                result.AddActionStepResult(new StepResult(new StringStep("Given", "something that passes", Source), new Passed()));
                result.AddActionStepResult(new StepResult(new StringStep("Given", "something", Source), new PendingNotImplemented("not implemented")));
                resultPublisher.Notify(result);
            }

            [Test]
            public void Should_notify_of_pending_step()
            {
                server.AssertWasCalled(_ => _.TaskOutput(task, "", TaskOutputType.STDOUT));
                server.AssertWasCalled(_ => _.TaskFinished(task, "not implemented", TaskResult.Inconclusive));
            }

            [Test]
            public void Should_notify_of_pending_scenario()
            {
                server.AssertWasCalled(_ => _.TaskFinished(scenarioTask, "not implemented", TaskResult.Inconclusive));
            }
        }

        [Ignore("Fix!")]
        public class When_running_Table_steps : ResharperResultPublisherSpec
        {
            protected override void Because_of()
            {
                var result = new ScenarioResult(feature, ScenarioTitle);
                var stringTableStep = new StringTableStep("Given", "something", Source);
                stringTableStep.AddTableStep(new Example(new ExampleColumns(new[] { new ExampleColumn("A"), new ExampleColumn("B"), }),
                                                         new Dictionary<string, string> { { "A", "aaa" }, { "B", "bb" } }));
                result.AddActionStepResult(new StepResult(stringTableStep, new Passed()));
                resultPublisher.Notify(result);
            }

            [Test]
            public void Should_add_table_to_explanation()
            {
                server.AssertWasCalled(_ => _.TaskOutput(task, "| A   | B  |\r\n| aaa | bb |", TaskOutputType.STDOUT));
            }
        }
    }
}