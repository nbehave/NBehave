namespace NBehave.Narrator.Framework
{
    using NBehave.Narrator.Framework.Tiny;

    public interface IMessengerHubAware
    {
        void Initialise(ITinyMessengerHub hub);
    }
}