using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NBehave.Narrator.Framework;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Rhino.Mocks;
using Context = NUnit.Framework.TestFixtureAttribute;
using Specification = NUnit.Framework.TestAttribute;

namespace NBehave.Narrator.Framework.Specifications
{
    public class ErgoAttribute : ActionStepAttribute
    {
        //Ergo = Given in latin, from http://www.freedict.com/onldict/onldict.php
        public ErgoAttribute(string tokenString)
            : base("Ergo " + tokenString)
        { }
    }

    [Context]
    public class When_registering_alias_for_ActionStepAttribute
    {
        [Specification]
        public void ShouldRegisterAndAsDefaultAliasForGiven()
        {
            var actionStepAlias = new ActionStepAlias();
            var aliases = actionStepAlias.GetAliasFor(typeof(GivenAttribute));
            Assert.That(aliases.Count(), Is.EqualTo(1));
            Assert.That(aliases.First(), Is.EqualTo("And"));
        }

        [Specification]
        public void ShouldRegisterAndAsDefaultAliasForThen()
        {
            var actionStepAlias = new ActionStepAlias();
            var aliases = actionStepAlias.GetAliasFor(typeof(ThenAttribute));
            Assert.That(aliases.Count(), Is.EqualTo(1));
            Assert.That(aliases.First(), Is.EqualTo("And"));
        }

        [Specification]
        public void ShouldRegisterAndAsDefaultAliasForErgo()
        {
            var actionStepAlias = new ActionStepAlias();
            IEnumerable<string> aliases = actionStepAlias.GetAliasFor(typeof(ErgoAttribute));
            Assert.That(aliases.Count(), Is.EqualTo(3));
            Assert.That(aliases.First(), Is.EqualTo("Tunc"));
            Assert.That(aliases.Skip(1).First(), Is.EqualTo("Deinde"));
            Assert.That(aliases.Skip(2).First(), Is.EqualTo("Mox"));

        }

        [Specification]
        public void ShouldGetAliasForTokenString()
        {
            string tokenStringTemplate = "{0} err, I dont know latin!";
            string tokenString = string.Format(tokenStringTemplate, "Ergo");
            var actionStepAlias = new ActionStepAlias();
            IEnumerable<string> aliases = actionStepAlias.GetAliasForTokenString(tokenString);
            Assert.That(aliases.Count(), Is.EqualTo(3));
            Assert.That(aliases.First(), Is.EqualTo(string.Format(tokenStringTemplate, "Tunc")));
            Assert.That(aliases.Skip(1).First(), Is.EqualTo(string.Format(tokenStringTemplate, "Deinde")));
            Assert.That(aliases.Skip(2).First(), Is.EqualTo(string.Format(tokenStringTemplate, "Mox")));
        }

        [Specification]
        public void ShouldGetDefaultAliasForGiven()
        {
            string tokenString = "Given something";
            var actionStepAlias = new ActionStepAlias();
            IEnumerable<string> aliases = actionStepAlias.GetAliasForTokenString(tokenString);
            Assert.That(aliases.Count(), Is.EqualTo(1));
            Assert.That(aliases.First(), Is.EqualTo("And something"));
        }
    }
}
