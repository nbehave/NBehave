using System.IO;

namespace NBehave.VS2010.Plugin
{
    public static class FileExtensions
    {
        public static string ToTempFile(this string content)
        {
            var tempFileName = Path.GetTempFileName();
            using (var writer = new StreamWriter(tempFileName))
            {
                writer.Write(content);
            }
            return tempFileName;
        }
    }
}