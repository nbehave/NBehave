using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NBehave.Narrator.Framework
{
    public class Language
    {
        private readonly YmlEntry _ymlEntry;

        public string Lang { get { return _ymlEntry.Key; } }

        public Language(YmlEntry ymlEntry)
        {
            _ymlEntry = ymlEntry;
        }

        public IEnumerable<string> this[string key]
        {
            get
            {
                return _ymlEntry[key].Values.Select(_ => _.Key).ToArray();
            }
        }

        public static IEnumerable<Language> LoadLanguages()
        {
            var ymlParser = new YmlParser();
            string path = GetFullFileName();
            var ymlStream = File.OpenRead(path);
            var yml = ymlParser.Parse(ymlStream);
            var languages = CreateLanguagesList(yml);
            return languages;
        }

        private static IEnumerable<Language> CreateLanguagesList(YmlEntry yml)
        {
            var languages = new List<Language>();
            foreach (var ymlEntry in yml.Values)
            {
                var language = new Language(ymlEntry);
                languages.Add(language);
            }
            return languages;
        }

        private static string GetFullFileName()
        {
            string directory = Path.GetDirectoryName((new System.Uri(typeof(YmlEntry).Assembly.CodeBase)).AbsolutePath);
            return Path.Combine(directory, "languages.yml");
        }
    }
}