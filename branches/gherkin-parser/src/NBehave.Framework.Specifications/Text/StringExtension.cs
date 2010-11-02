using System.IO;

namespace NBehave.Narrator.Framework.Specifications
{
    public static class StringExtension
    {
        public static Stream ToStream(this string str)
        {
            var ms = new MemoryStream();
            var sr = new StreamWriter(ms);
            sr.Write(str);
            sr.Flush();
            ms.Seek(0, SeekOrigin.Begin);
            return ms; 
        }
    }
}