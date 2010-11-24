using System.Collections.Generic;
using System.IO;

namespace NBehave.Narrator.Framework
{
    public class YmlParser
    {
        public YmlEntry Parse(Stream stream)
        {
            var root = new YmlEntry("root");
            ICollection<YmlEntry> categories = root.Values;
            IEnumerable<string> rows = ReadAllRows(stream);
            YmlEntry parent = null;
            foreach (string row in rows)
            {
                if (row.StartsWith("\""))
                {
                    string name = row.Substring(1, row.IndexOf("\"", 1) - 1);
                    parent = new YmlEntry(name);
                    categories.Add(parent);
                }
                else if (row.StartsWith("  "))
                {
                    var nameValue = row.Trim().Split(':');
                    var entry = new YmlEntry(nameValue[0]);
                    var values = nameValue[1].Split('|');
                    foreach (var value in values)
                        entry.AddValue(value.Trim());
                    parent.Values.Add(entry);
                }
            }
            return root;
        }

        IEnumerable<string> ReadAllRows(Stream stream)
        {
            var sr = new StreamReader(stream);
            var rows = new List<string>();
            while (sr.EndOfStream == false)
                rows.Add(sr.ReadLine());
            return rows;
        }
    }
}