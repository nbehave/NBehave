using NUnit.Framework;

namespace NBehave.Narrator.Framework.Specifications.Text
{
    [TestFixture]
    public class StringStepSpec
    {
        private StringStepRunner _stringStepRunner;
        private ActionCatalog _catalog;

        [SetUp]
        public void Establish_context()
        {
            _catalog = new ActionCatalog();
            _stringStepRunner = new StringStepRunner(_catalog);
        }

        private StringStep CreateInstance(string step)
        {
            return new StringStep(step, "fileName", _stringStepRunner);
        }

        public class When_comparing_StringSteps : StringStepSpec
        {
            [Test]
            public void same_ref_should_be_equal()
            {
                StringStep s = CreateInstance("Foo");
                Assert.That(s.Equals(s), Is.True);
            }

            [Test]
            public void instance_is_not_equal_to_null()
            {
                var s = CreateInstance("Foo");
                Assert.That(s.Equals(null), Is.False);
            }

            [Test]
            public void instance_is_not_equal_to_int()
            {
                var s = CreateInstance("1");
                Assert.That(s.Equals(1), Is.False);
            }

            [Test]
            public void same_steps_should_be_equal()
            {
                var s1 = CreateInstance("Foo");
                var s2 = CreateInstance("Foo");
                Assert.That(s1.Equals(s2), Is.True);
            }

            [Test]
            public void different_text_in_steps_should_not_be_equal()
            {
                var s1 = CreateInstance("Foo");
                var s2 = CreateInstance("Bar");
                Assert.That(s1.Equals(s2), Is.False);
            }
        }
    }
}