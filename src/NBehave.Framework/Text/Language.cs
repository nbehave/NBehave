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
            IEnumerable<Language> languages;
            using (var ymlStream = OpenLanguageFile())
            {
                var yml = ymlParser.Parse(ymlStream);
                languages = CreateLanguagesList(yml);
            }
            return languages;
        }

        private static Stream OpenLanguageFile()
        {
            Assembly assembly = typeof(Language).Assembly;
            return assembly.GetManifestResourceStream(assembly.GetName().Name + ".Resource.languages.yml");
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
    }
}