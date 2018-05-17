using System.IO;
using NUnit.Framework;

namespace NBehave.Narrator.Framework.Specifications.Text
{
    [TestFixture]
    public class ExampleColumnsSpec
    {
        [Test]
        public void ShouldBeSerializable()
        {
            var e = new ExampleColumns(new[] { "a", "b" });
            var ser = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            using (var ms = new MemoryStream())
                ser.Serialize(ms, e);
            Assert.IsTrue(true, "Serialization succeded");
        }
    }
}