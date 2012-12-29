using NBehave.Contracts;

namespace NBehave
{
    public class MatchAllFiles : IFileMatcher
    {
        bool IFileMatcher.IsMatch(string fileName)
        {
            return true;
        }
    }
}