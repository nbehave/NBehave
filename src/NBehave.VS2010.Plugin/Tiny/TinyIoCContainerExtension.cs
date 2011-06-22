using NBehave.Narrator.Framework.Tiny;

namespace NBehave.VS2010.Plugin.Tiny
{
    public static class TinyIoCContainerExtension
    {
        public static void Install(this TinyIoCContainer container, params ITinyIocInstaller[] installers)
        {
            foreach (var installer in installers)
                installer.Install(container);
        }
    }

    public interface ITinyIocInstaller
    {
        void Install(TinyIoCContainer container);
    }
}