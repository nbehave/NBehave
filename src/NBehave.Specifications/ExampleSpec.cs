using System.Collections.Generic;
using NUnit.Framework;

namespace NBehave.Narrator.Framework.Specifications.Text
{
    [TestFixture]
    public class ExampleSpec
    {
        Example _row;

        [SetUp]
        public void EstablishContext()
        {
            const string colName = "colName";
            const string colValue = "a really wide column value";
            var columnNames = new ExampleColumns { new ExampleColumn(colName) };
            var columnValues = new Dictionary<string, string>
            {
                { "colName" , colValue }
            };
            _row = new Example(columnNames, columnValues);
        }

        [Test]
        public void Should_use_formatted_column()
        {
            var rowAsString = _row.ColumnNamesToString();
            const string expected = " | colName |";
            Assert.That(rowAsString, Is.EqualTo(expected));
        }

        [Test]
        public void ShouldMakeColumnValuesToString()
        {
            var rowAsString = _row.ColumnValuesToString();
            const string expected = " | a really wide column value |";
            Assert.That(rowAsString, Is.EqualTo(expected));
        }
    }
}
