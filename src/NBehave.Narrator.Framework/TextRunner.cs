using System;
using System.Collections.Generic;
using System.Reflection;
using NBehave.Narrator.Framework.Processors;
using NBehave.Narrator.Framework.Tiny;

namespace NBehave.Narrator.Framework
{
    public class TextRunner : MarshalByRefObject, IRunner
    {
        private readonly NBehaveConfiguration configuration;
        private ActionCatalog actionCatalog;

        public TextRunner(NBehaveConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public FeatureResults Run()
        {
            NBehaveInitialiser.Initialise(configuration);
            actionCatalog = TinyIoCContainer.Current.Resolve<ActionCatalog>();
            var featureRunner = TinyIoCContainer.Current.Resolve<IFeatureRunner>();
            var context = TinyIoCContainer.Current.Resolve<IRunContext>();
            var results = new FeatureResults();
            try
            {
                context.RunStarted();
                LoadAssemblies();
                var loader = new LoadScenarioFiles(configuration);
                var files = loader.LoadFiles();
                var parse = new ParseScenarioFiles(configuration);
                var features = parse.LoadFiles(files);
                results = Run(featureRunner, features, context);
            }
            finally
            {
                context.RunFinished(results);
            }

            return results;
        }

        private FeatureResults Run(IFeatureRunner featureRunner, IEnumerable<Feature> features, IRunContext context)
        {
            var result = new FeatureResults();
            foreach (Feature feature in features)
            {
                FeatureResult featureResult = null;
                try
                {
                    context.FeatureStarted(feature);
                    featureResult = featureRunner.Run(feature);
                    result.Add(featureResult);
                }
                finally
                {
                    context.FeatureFinished(featureResult);
                }
            }
            return result;
        }

        private void LoadAssemblies()
        {
            var parser = new ActionStepParser(configuration.Filter, actionCatalog);
            foreach (var assembly in configuration.Assemblies)
            {
                parser.FindActionSteps(Assembly.LoadFrom(assembly));
            }
        }

        public override object InitializeLifetimeService()
        {
            return null;
        }
    }
}