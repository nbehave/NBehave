using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace NBehave.Narrator.Framework.Specifications.Domain
{
    [TestFixture]
    public class ExampleTableFormatterSpec
    {
        private ExampleTableFormatter exampleTableFormatter;
        private List<Example> examples;

        [SetUp]
        public void SetUp()
        {
            const string colC = "A really long column name";
            var columnNames = new ExampleColumns
                                  {
                                      new ExampleColumn("A"),
                                      new ExampleColumn("B"),
                                      new ExampleColumn(colC)
                                  };
            examples = new List<Example>
                               {
                                   new Example(columnNames, new Dictionary<string, string> {{"A", "12345"}, {"B", "b"}, {colC, "c"}}),
                                   new Example(columnNames, new Dictionary<string, string> {{"A", "a"}, {"B", "123456"}, {colC, "c"}}),
                               };
            exampleTableFormatter = new ExampleTableFormatter();            
        }

        [Test]
        public void Should_format_tableHeader_after_widest_column_values()
        {
            var header = exampleTableFormatter.TableHeader(examples);
            Assert.That(header, Is.EqualTo("| A     | B      | A really long column name |"));
        }

        [Test]
        public void Should_format_tablecolumns_after_widest_column_values()
        {
            var rows = exampleTableFormatter.TableRows(examples);
            Assert.That(rows[0], Is.EqualTo("| 12345 | b      | c                         |"));
            Assert.That(rows[1], Is.EqualTo("| a     | 123456 | c                         |"));
        }
    }
}