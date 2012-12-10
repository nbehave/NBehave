using System.Collections.Generic;
using NUnit.Framework;

namespace NBehave.Narrator.Framework.Specifications.Domain
{
    [TestFixture]
    public class ExampleBuilderSpec
    {
        [Test]
        public void Should_create_table_instance_from_string()
        {
            var columnNames = new ExampleColumns(new[] { new ExampleColumn("colA"), new ExampleColumn("colB"), });
            var columnValues = new Dictionary<string, string> { { "colA", "A" }, { "colB", "B" } };
            var example = new Example(columnNames, columnValues);

            var str = example.ToString();
            var exampleFromString = ExampleBuilder.BuildFromString(str);

            CollectionAssert.AreEqual(exampleFromString.ColumnNames, example.ColumnNames);
            CollectionAssert.AreEqual(exampleFromString.ColumnValues, example.ColumnValues);
        }
    }
}