using System.IO;

namespace NBehave.TestDriven.Plugin
{
    public class DirectoryWalker : IDirectoryWalker
    {
        public void WalkSubDirectories(string directory, IDirectoryVisitor visitor)
        {
            WalkSubDirectories(new DirectoryInfo(directory), visitor);
        }

        private static void WalkSubDirectories(DirectoryInfo directory, IDirectoryVisitor visitor)
        {
            visitor.VisitDirectory(directory);

            foreach (var subDirectory in directory.GetDirectories())
            {
                WalkSubDirectories(subDirectory, visitor);
            }
        }
    }

    public interface IDirectoryWalker
    {
        void WalkSubDirectories(string directory, IDirectoryVisitor visitor);
    }

    public interface IDirectoryVisitor
    {
        void VisitDirectory(DirectoryInfo directory);
    }
}