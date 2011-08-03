using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NBehave.Fluent.Extensions;
using NBehave.Narrator.Framework;
using NBehave.Spec.NUnit;
using NUnit.Framework;

namespace NBehave.TestDriven.Plugin.Specs
{
    [TestFixture]
    public class AssemblyHelperSpecs : ScenarioDrivenSpecBase
    {
        protected override Feature CreateFeature()
        {
            return new Feature("Parsing an assembly to try and deduce the default namespace");
        }

        [Test]
        public void should_correctly_parse_NBehave_Narrator_Framework()
        {
            Feature.AddScenario("Parsing NBehave.Narrator.Framework")
                .WithHelperObject<AssemblyHelperSpecSteps>()
                .Given("A reference to the NBehave.Narrator.Framework assembly")
                .When("We deduce the root namespace")
                .Then("It should be NBehave.Narrator.Framework")
                ;
        }

        [Test]
        public void should_correctly_parse_NBehave_Spec_NUnit()
        {
            Feature.AddScenario("Parsing NBehave.Spec.NUnit")
                .WithHelperObject<AssemblyHelperSpecSteps>()
                .Given("A reference to the NBehave.Spec.NUnit assembly")
                .When("We deduce the root namespace")
                .Then("It should be NBehave.Spec.NUnit")
                ;
        }
    }

    [ActionSteps]
    public class AssemblyHelperSpecSteps
    {
        private Assembly _assembly;
        private IEnumerable<string> _deducedRootNamespaceParts;

        [Given("A reference to the NBehave.Narrator.Framework assembly")]
        public void ReferenceNarratorAssembly()
        {
            _assembly = typeof(ScenarioResult).Assembly;
        }

        [Given("A reference to the NBehave.Spec.NUnit assembly")]
        public void ReferenceNBehaveSpecNUnitAssembly()
        {
            _assembly = typeof(ScenarioDrivenSpecBase).Assembly;
        }

        [When("We deduce the root namespace")]
        public void DeduceRootNamespace()
        {
            _deducedRootNamespaceParts = AssemblyHelper.DeduceRootNamespaceParts(_assembly);            
        }

        [Then("It should be $expected")]
        public void CheckDeducedNamespace(string expected)
        {
            var rootNameSpace = String.Join(".", _deducedRootNamespaceParts.ToArray());
            Assert.AreEqual(expected, rootNameSpace);            
        }
    }
}
