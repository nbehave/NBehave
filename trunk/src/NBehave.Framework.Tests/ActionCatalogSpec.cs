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
    [Context]
    public class when_adding_an_action_to_the_catalog
    {

        [Specification]
        public void should_consider_the_2_actions_as_equal()
        {
            ActionCatalog catalog = new ActionCatalog();

            catalog.Add("my savings account balance is $balance", new object());
            bool actionExists = catalog.ActionExists("my savings account balance is 500");

            Assert.That(actionExists, Is.True);
        }

        [Specification]
        public void should_get_action()
        {
            ActionCatalog catalog = new ActionCatalog();

            catalog.Add("my savings account balance is $balance", new object());
            object action = catalog.GetAction("my savings account balance is 500");

            Assert.That(action, Is.Not.Null);
            
        }
    }
}
