using System.ComponentModel.Design;
using NBehave.Narrator.Framework.Tiny;
using NBehave.VS2010.Plugin.Contracts;
using NBehave.VS2010.Plugin.Tiny;

namespace NBehave.VS2010.Plugin.Configuration
{
    public class PublishCrossPackageServicesTask : ITinyIocInstaller
    {
        private void AddService<T>(IServiceContainer serviceContainer, TinyIoCContainer iocContainer)
            where T : class
        {
            serviceContainer.AddService(
                typeof(T),
                (container, serviceType) => iocContainer.Resolve<T>(),
                true);
        }

        public void Install(TinyIoCContainer container)
        {
            /**
             * Don't forget to add a guid to the service interface, and add the 
             * ProvideService attribute to the Package class.
             */

            var serviceContainer = container.Resolve<IServiceContainer>();

            AddService<IOutputWindow>(serviceContainer, container);
            AddService<IVisualStudioService>(serviceContainer, container);
            AddService<IPluginLogger>(serviceContainer, container);
        }
    }
}
