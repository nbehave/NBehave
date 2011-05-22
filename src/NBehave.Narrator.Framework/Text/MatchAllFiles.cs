namespace NBehave.Narrator.Framework
{
    public class MatchAllFiles : IFileMatcher
    {
        bool IFileMatcher.IsMatch(string fileName)
        {
            return true;
        }
    }
}