using System;
using System.Linq;
using System.Text;
using NBehave.Fluent.Framework.Extensions;
using Rhino.Mocks;

namespace NBehave.Fluent.Framework
{
    public abstract class ScenarioDrivenSpecBase
    {
        protected Feature Feature { get; private set; }

        protected abstract Feature CreateFeature();

        public virtual void MainSetup()
        {
            Feature = CreateFeature();
        }

        public virtual void MainTeardown()
        {
            var scenariosWithPendingSteps = Feature.FindPendingSteps();
            if (!scenariosWithPendingSteps.Any())
                return;

            // Some steps are still pending an implementation.  Let's build up a message about them.
            var message = new StringBuilder();
            foreach (var scenarioStruct in scenariosWithPendingSteps)
            {
                message.AppendFormat("Scenario: {0}{1}", scenarioStruct.Key.Title, Environment.NewLine);
                foreach (var pendingStep in scenarioStruct)
                {
                    message.AppendFormat("  Step: {0}{1}", pendingStep.Step, Environment.NewLine);
                }
                message.AppendFormat(Environment.NewLine);
            }
            throw new ApplicationException("The following implementations are still pending:" + Environment.NewLine + message);
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
}
