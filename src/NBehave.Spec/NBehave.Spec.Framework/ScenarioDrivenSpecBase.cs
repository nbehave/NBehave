using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NBehave.Narrator.Framework;
using NBehave.Spec.Extensions;
using Rhino.Mocks;

namespace NBehave.Spec
{
    public abstract class ScenarioDrivenSpecBase
    {
        protected Feature Feature { get; private set; }

        protected abstract Feature CreateFeature();

        public virtual void MainSetup()
        {
            Feature = new ScenarioDrivenFeature(CreateFeature());
        }

        public virtual void MainTeardown()
        {
            ((ScenarioDrivenFeature) Feature).CloseBuilders();

            var scenariosWithPendingSteps = Feature.FindPendingSteps();
            if (scenariosWithPendingSteps.Count() <= 0)
            {
                return;
            }
            
            // Some steps are still pending an implementation.  Let's build up a message about them.
            var message = new StringBuilder();
            foreach (var scenarioStruct in scenariosWithPendingSteps)
            {
                message.AppendFormat("Scenario: {0}\r\n", scenarioStruct.Key.Title);
                foreach (var pendingStep in scenarioStruct)
                {
                    message.AppendFormat("  Step: {0}\r\n", pendingStep.Step);
                }
                message.AppendFormat("\r\n");
            }
            throw new ApplicationException("The following implementations are still pending:\r\n" + message);
        }

        protected TType CreateDependency<TType>()
            where TType : class
        {
            return MockRepository.GenerateMock<TType>();
        }

        protected TType CreateStub<TType>()
            where TType : class
        {
            return MockRepository.GenerateStub<TType>();
        }
    }

    public class ScenarioDrivenFeature : Feature
    {
        private readonly List<ScenarioBuilder> _builders = new List<ScenarioBuilder>();

        public ScenarioDrivenFeature(Feature feature) : base(feature.Title)
        {
            Narrative = feature.Narrative;
            IsDryRun = feature.IsDryRun;

            foreach (var scenario in feature.Scenarios)
            {
                AddScenario(scenario);
            }
        }

        public void RegisterScenarioBuilder(ScenarioBuilder scenarioBuilder)
        {
            _builders.Add(scenarioBuilder);
        }

        public void CloseBuilders()
        {
            foreach (var builder in _builders)
            {
                builder.OnFeatureClosing();
            }
        }
    }
}
