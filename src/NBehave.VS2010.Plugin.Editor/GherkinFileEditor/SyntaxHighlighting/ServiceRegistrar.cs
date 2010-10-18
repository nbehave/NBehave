using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Tagging;
using NBehave.VS2010.Plugin.Contracts;
using NBehave.VS2010.Plugin.Domain;
using NBehave.VS2010.Plugin.GherkinFileEditor.Glyphs;
using IOleServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;

namespace NBehave.VS2010.Plugin.GherkinFileEditor
{
    [Export(typeof(ServiceRegistrar))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    internal class ServiceRegistrar
    {
        [Import]
        public IClassificationTypeRegistryService ClassificationRegistry { get; set; }

        [Import(typeof(SVsServiceProvider))]
        public IServiceProvider ServiceProvider { get; set; }

//        [Import]
//        public IOutputWindow OutputWindow { get; set; }

        public void Initialise(ITextBuffer buffer)
        {
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
                buffer.Properties.GetOrCreateSingletonProperty(() => new PlayTagger(buffer, scenarioRunner) as ITagger<PlayTag>);

                container.ComposeParts(fileClassifierForBuffer);
                fileClassifierForBuffer.BeginClassifications();
            }
        }
    }
}