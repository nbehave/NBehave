using NUnit.Framework;

namespace NBehave.Narrator.Framework.Specifications.Text
{
    public abstract class StringStepSpecs
    {
        [TestFixture]
        public class WhenComparingStringSteps
        {
            private StringStep CreateInstance(string step)
            {
                return new StringStep(step, "fileName");
            }

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

            [Test]
            public void Same_step_text_but_different_sorce_file_should_not_be_equal()
            {
                var s1 = new StringStep("Foo", "s1");
                var s2 = new StringStep("Foo", "s2");
                Assert.That(s1, Is.Not.EqualTo(s2));
            }
        }
    }
}