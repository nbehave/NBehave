using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Design;
using NBehave.VS2010.Plugin.Contracts;

namespace NBehave.VS2010.Plugin.Configuration
{
    [Export(typeof(IStartUpTask))]
    public class PublishCrossPackageServicesTask : IStartUpTask
    {
        [Import(typeof(IServiceContainer))]
        public IServiceContainer ServiceContainer { get; set; }

        [Import(typeof(CompositionContainer))]
        public CompositionContainer CompositionContainer { get; set; }

        public void Initialise()
        {
            /**
             * Don't forget to add a guid to the service interface, and add the 
             * ProvideService attribute to the Package class.
             */

            AddService<IOutputWindow>();
            AddService<IVisualStudioService>();
            AddService<IPluginLogger>();
        }

        private void AddService<T>()
        {
            ServiceContainer.AddService(
                typeof(T), 
                (container, serviceType) => CompositionContainer.GetExport<T>().Value, 
                true);
        }
    }
}
