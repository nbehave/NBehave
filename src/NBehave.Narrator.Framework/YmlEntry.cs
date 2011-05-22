using System.Collections.Generic;
using System.Linq;

namespace NBehave.Narrator.Framework
{
    public class YmlEntry
    {
        public string Key { get; private set; }
        private readonly List<YmlEntry> _values = new List<YmlEntry>();
        public ICollection<YmlEntry> Values { get { return _values; } }

        public YmlEntry this[string key]
        {
            get { return Values.First(e => e.Key == key); }
        }

        public YmlEntry(string name)
        {
            Key = name;
        }

        public void AddValue(string value)
        {
            _values.Add(new YmlEntry(value));
        }
    }
}