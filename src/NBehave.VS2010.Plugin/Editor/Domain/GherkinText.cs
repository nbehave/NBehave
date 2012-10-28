namespace NBehave.VS2010.Plugin.Editor.Domain
{
    public interface IGherkinText
    {
        string Title { get; }
        string AsString { get; }
        int SourceLine { get; }
    }
}