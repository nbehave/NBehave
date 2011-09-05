using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using NUnit.Framework;

namespace NBehave.Narrator.Framework.Specifications.Domain
{
    [TestFixture]
    public class StringTableStepSpec
    {
        [Test]
        public void Should_be_able_to_serialize_binary()
        {
            var s = new StringTableStep("Given x", "source");
            var columns = new ExampleColumns(new[] { new ExampleColumn("a") });
            var values = new Dictionary<string, string> { { "a", "value" } };
            var row = new Row(columns, values);
            s.AddTableStep(row);

            var b = new BinaryFormatter();
            using (Stream stream = new MemoryStream())
                b.Serialize(stream, s);
            Assert.Pass("Should not throw exception");
        }
    }
}
