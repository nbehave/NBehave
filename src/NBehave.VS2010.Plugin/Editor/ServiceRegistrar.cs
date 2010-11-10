using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Diagnostics;
using System.IO;
using System.Linq;
using MEFedMVVM.ViewModelLocator;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Tagging;
using NBehave.VS2010.Plugin.Contracts;
using NBehave.VS2010.Plugin.Domain;
using NBehave.VS2010.Plugin.Editor.Glyphs;
using NBehave.VS2010.Plugin.Editor.SyntaxHighlighting;

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
            AppDomain.CurrentDomain.UnhandledException +=
                (sender, unhandledExceptionEventArgs) => Debug.Assert(false, unhandledExceptionEventArgs.ToString());

            if (!buffer.Properties.ContainsProperty(typeof(GherkinFileClassifier)))
            {
                var container = buffer.Properties.GetOrCreateSingletonProperty(() => new CompositionContainer(new AssemblyCatalog(typeof(GherkinFileEditorParser).Assembly)));
                container.ComposeExportedValue(ClassificationRegistry);
                container.ComposeParts();

                container.GetExport<GherkinFileEditorParserFactory>().Value.CreateParser(buffer);

                var outputWindow = ServiceProvider.GetService(typeof(IOutputWindow)) as IOutputWindow;
                var visualStudioService = ServiceProvider.GetService(typeof(IVisualStudioService)) as IVisualStudioService;
                var scenarioRunner = new ScenarioRunner(outputWindow, visualStudioService);

                GherkinFileClassifier fileClassifierForBuffer = buffer.Properties.GetOrCreateSingletonProperty(() => new GherkinFileClassifier(buffer));
                buffer.Properties.GetOrCreateSingletonProperty(() => new PlayTagger(buffer) as ITagger<PlayGlyphTag>);

                container.ComposeExportedValue(scenarioRunner);
                container.ComposeParts((fileClassifierForBuffer));

                LocatorBootstrapper.ApplyComposer(new VisualStudioRuntimeComposer(container));
                LocatorBootstrapper.EnsureLocatorBootstrapper();

                fileClassifierForBuffer.BeginClassifications();
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
            return new []{ Container };
        }

        private AggregateCatalog GetCatalog()
        {
            var location = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                                 where assembly == typeof (ServiceRegistrar).Assembly
                                 select assembly.Location).First();

            var directory = Path.GetDirectoryName(location);

            var catalog = new AggregateCatalog();
            catalog.Catalogs.Add(new DirectoryCatalog(directory));

            return catalog;
        }
    }
}