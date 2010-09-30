using System.Collections.Generic;
using NUnit.Framework;

namespace NBehave.Narrator.Framework.Specifications.Text
{
    [TestFixture]
    public class RowSpec
    {
        Row _row;

        [SetUp]
        public void Establish_context()
        {
            var colName = "colName";
            var colValue = "a really wide column value";
            var columnNames = new ExampleColumns { colName };
            var columnValues = new Dictionary<string, string>
            {
                { "colName" , colValue }
            };
            _row = new Row(columnNames, columnValues);
        }

        [Test]
        public void Should_make_column_headers_as_wide_as_widest_row_for_column()
        {
      

            var rowAsString = _row.ColumnNamesToString();
            var expected = "|colName                   |";
            Assert.That(rowAsString, Is.EqualTo(expected));
        }

        [Test]
        public void Should_make_column_values_to_string()
        {
            var rowAsString = _row.ColumnValuesToString();
            var expected = "|a really wide column value|";
            Assert.That(rowAsString, Is.EqualTo(expected));
        }
    }
}
