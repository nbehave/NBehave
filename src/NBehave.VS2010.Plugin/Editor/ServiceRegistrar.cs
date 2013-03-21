using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.IO;
using System.Linq;
using System.Runtime.ExceptionServices;
using MEFedMVVM.ViewModelLocator;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using NBehave.VS2010.Plugin.Contracts;
using NBehave.VS2010.Plugin.Domain;
using NBehave.VS2010.Plugin.Tagging;

namespace NBehave.VS2010.Plugin.Editor
{
    [Export(typeof(ServiceRegistrar))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    internal class ServiceRegistrar
    {
        [Import]
        public IClassificationTypeRegistryService ClassificationRegistry { get; set; }

        [Import(typeof(SVsServiceProvider))]
        public IServiceProvider ServiceProvider { get; set; }

        public void Initialise(ITextBuffer buffer)
        {
            if (!buffer.Properties.ContainsProperty(typeof(TokenParser)))
            {
                var pluginLogger = (IPluginLogger)ServiceProvider.GetService(typeof(IPluginLogger));

                AppDomain.CurrentDomain.FirstChanceException += (sender, args) => pluginLogger.LogFirstChanceException(args);

                var container = buffer.Properties.GetOrCreateSingletonProperty(() => new CompositionContainer(new AssemblyCatalog(GetType().Assembly)));
                container.ComposeExportedValue(ClassificationRegistry);
                container.ComposeParts();

                var scenarioRunner = ServiceProvider.GetService(typeof(IScenarioRunner)) as IScenarioRunner;
                container.ComposeExportedValue(scenarioRunner);

                LocatorBootstrapper.ApplyComposer(new VisualStudioRuntimeComposer(container));
                LocatorBootstrapper.EnsureLocatorBootstrapper();
            }
        }
    }

    public class VisualStudioRuntimeComposer : IComposer
    {
        public CompositionContainer Container { get; set; }

        public VisualStudioRuntimeComposer(CompositionContainer container)
        {
            Container = container;
        }

        public ComposablePartCatalog InitializeContainer()
        {
            return Container.Catalog;
        }

        public IEnumerable<ExportProvider> GetCustomExportProviders()
        {
            return new[] { Container };
        }

        private AggregateCatalog GetCatalog()
        {
            var location = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                            where assembly == typeof(VisualStudioRuntimeComposer).Assembly
                            select assembly.Location).First();

            var directory = Path.GetDirectoryName(location);

            var catalog = new AggregateCatalog();
            catalog.Catalogs.Add(new DirectoryCatalog(directory));

            return catalog;
        }
    }
}