using System.IO;
using System.Linq;
using NUnit.Framework;

namespace NBehave.Narrator.Framework.Specifications
{
    [TestFixture]
    public class YmlParserSpec
    {
        private YmlParser _ymlParser;
        private YmlEntry _parsedYmlRoot;

        [SetUp]
        public void SetUp()
        {
            EstablishContext();
            BecauseOf();
        }

        protected virtual void EstablishContext()
        {
            _ymlParser = new YmlParser();
        }

        protected virtual void BecauseOf()
        {
            var ms = new MemoryStream();
            var sr = new StreamWriter(ms);
            sr.WriteLine("# comment");
            sr.WriteLine("\"se\":");
            sr.WriteLine("# comment");
            sr.WriteLine("  example: Exempel");
            sr.WriteLine("  feature: Story|Krav|Feature");
            sr.Flush();
            ms.Seek(0, SeekOrigin.Begin);
            _parsedYmlRoot = _ymlParser.Parse(ms);
        }

        [Test]
        public void ShouldFindCategories()
        {

            Assert.That(_parsedYmlRoot.Values.First().Key, Is.EqualTo("se"));
        }

        [Test]
        public void ShouldIgnoreComments()
        {
            Assert.That(_parsedYmlRoot.Values.Count, Is.EqualTo(1));
        }

        [Test]
        public void ShouldFindSubcategories()
        {
            Assert.That(_parsedYmlRoot["se"]["example"].Values.First().Key, Is.EqualTo("Exempel"));
        }

        [Test]
        public void ShouldHaveMultipleValuesForSameKey()
        {
            Assert.That(_parsedYmlRoot["se"]["feature"].Values.Count(), Is.EqualTo(3));
            Assert.That(_parsedYmlRoot["se"]["feature"].Values.First().Key, Is.EqualTo("Story"));
            Assert.That(_parsedYmlRoot["se"]["feature"].Values.Skip(1).First().Key, Is.EqualTo("Krav"));
            Assert.That(_parsedYmlRoot["se"]["feature"].Values.Last().Key, Is.EqualTo("Feature"));
        }
    }
}
