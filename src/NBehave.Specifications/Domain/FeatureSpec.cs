using System;
using NUnit.Framework;

namespace NBehave.Specifications.Domain
{
    [TestFixture]
    public class FeatureSpec
    {
        [Test]
        public void Should_convert_feature_to_string()
        {
            var feature = new Feature("Title", string.Format("  As a x{0}  I want y{0}  So that z", Environment.NewLine), "source", 1);
            var featureAsString = feature.ToString();
            string expected = string.Format("Feature: Title{0}  As a x{0}  I want y{0}  So that z", Environment.NewLine);
            Assert.AreEqual(expected, featureAsString);
        }
 
        [Test]
        public void Should_convert_background_to_string()
        {
            var feature = new Feature("Title", string.Format("  As a x{0}  I want y{0}  So that z", Environment.NewLine), "source", 1);
            var background = new Scenario("backgroundTitle", "source", feature, 4);
            background.AddStep(new StringStep("Given", "a background step", "source, 5"));
            feature.AddBackground(background);
            var featureAsString = feature.ToString();
            string expected = string.Format("Feature: Title{0}  As a x{0}  I want y{0}  So that z", Environment.NewLine);
            expected += string.Format("{0}{0}  Background: backgroundTitle{0}    Given a background step", Environment.NewLine);
            Assert.AreEqual(expected, featureAsString);
        }
    }
}