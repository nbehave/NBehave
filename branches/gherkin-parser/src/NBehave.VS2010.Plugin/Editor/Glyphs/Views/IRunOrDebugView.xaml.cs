namespace NBehave.VS2010.Plugin.Editor.Glyphs.Views
{
    public interface IRunOrDebugView
    {
        bool IsMouseOverPopup { get; }
        void Deselect();
    }
}