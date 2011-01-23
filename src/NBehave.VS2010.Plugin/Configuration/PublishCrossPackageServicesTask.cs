using System.ComponentModel.Design;
using NBehave.VS2010.Plugin.Contracts;

namespace NBehave.VS2010.Plugin.Configuration
{
    using Castle.MicroKernel.Registration;
    using Castle.MicroKernel.SubSystems.Configuration;
    using Castle.Windsor;

    public class PublishCrossPackageServicesTask : IWindsorInstaller
    {
        private void AddService<T>(IServiceContainer serviceContainer, IWindsorContainer windsorContainer)
        {
            serviceContainer.AddService(
                typeof(T), 
                (container, serviceType) => windsorContainer.Resolve<T>(), 
                true);
        }

        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            /**
             * Don't forget to add a guid to the service interface, and add the 
             * ProvideService attribute to the Package class.
             */

            var serviceContainer = container.Resolve<IServiceContainer>();

            this.AddService<IOutputWindow>(serviceContainer, container);
            this.AddService<IVisualStudioService>(serviceContainer, container);
            this.AddService<IPluginLogger>(serviceContainer, container);
        }
    }
}
