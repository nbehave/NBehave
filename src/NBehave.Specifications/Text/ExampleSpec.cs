using System.Collections.Generic;
using System.IO;
using NUnit.Framework;

namespace NBehave.Narrator.Framework.Specifications.Text
{
    [TestFixture]
    public class ExampleSpec
    {
        [Test]
        public void ShouldBeSerializable()
        {
            var e = new Example(new ExampleColumns(new[] { "a" }), new Dictionary<string, string> { { "a", "a" } });
            var ser = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            using (var ms = new MemoryStream())
                ser.Serialize(ms, e);
            Assert.IsTrue(true, "Serialization succeded");
        }
    }
}