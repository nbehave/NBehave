using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

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
            var path = GetPath(typeof(YmlEntry).Assembly);
            if (File.Exists(path))
                return path;
            path = GetPath(Assembly.GetExecutingAssembly());
            return path;
        }

        private static string GetPath(Assembly assembly)
        {
            string directory = Path.GetDirectoryName((new System.Uri(assembly.CodeBase)).LocalPath);
            var path = Path.Combine(directory, "languages.yml");
            return path;
        }
    }
}