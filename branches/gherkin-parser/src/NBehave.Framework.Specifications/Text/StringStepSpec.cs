using NUnit.Framework;

namespace NBehave.Narrator.Framework.Specifications.Text
{
    [TestFixture]
    public class StringStepSpec
    {
        private StringStepRunner _stringStepRunner;
        private ActionCatalog _catalog;

        [SetUp]
        public void EstablishContext()
        {
            _catalog = new ActionCatalog();
            _stringStepRunner = new StringStepRunner(_catalog);
        }

        private StringStep CreateInstance(string step)
        {
            return new StringStep(step, "fileName", _stringStepRunner);
        }

        public class WhenComparingStringSteps : StringStepSpec
        {
            [Test]
            public void SameRefShouldBeEqual()
            {
                CreateInstance("Foo");
                Assert.That(true, Is.True);
            }

            [Test]
            public void InstanceIsNotEqualToNull()
            {
                var s = CreateInstance("Foo");
                Assert.That(s.Equals(null), Is.False);
            }

            [Test]
            public void InstanceIsNotEqualToInt()
            {
                var s = CreateInstance("1");
                Assert.That(s.Equals(1), Is.False);
            }

            [Test]
            public void SameStepsShouldBeEqual()
            {
                var s1 = CreateInstance("Foo");
                var s2 = CreateInstance("Foo");
                Assert.That(s1.Equals(s2), Is.True);
            }

            [Test]
            public void DifferentTextInStepsShouldNotBeEqual()
            {
                var s1 = CreateInstance("Foo");
                var s2 = CreateInstance("Bar");
                Assert.That(s1.Equals(s2), Is.False);
            }
        }
    }
}