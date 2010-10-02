using NUnit.Framework;

namespace NBehave.Narrator.Framework.Specifications.Text
{
    [TestFixture]
    public class ActionStepAttributeSpec
    {
        [Test]
        public void ShouldIgnoreSpaceAtEndOfStep()
        {
            var g = new GivenAttribute("something");
            Assert.That(g.ActionMatch.IsMatch("something  "), Is.True);
        }
    }
}
