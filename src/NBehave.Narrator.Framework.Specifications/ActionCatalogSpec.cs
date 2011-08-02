using System;
using System.Reflection;
using System.Text.RegularExpressions;
using NUnit.Framework;

namespace NBehave.Narrator.Framework.Specifications
{
    [TestFixture]
    public abstract class ActionCatalogSpec
    {
        private MethodInfo GetDummyParameterInfo()
        {
            Action<int> a = p => { };
            return a.Method;
        }

        [TestFixture]
        public class WhenAddingAnActionToTheCatalog : ActionCatalogSpec
        {
            [Test]
            public void ShouldConsiderThe2ActionsAsEqual()
            {
                var catalog = new ActionCatalog();

                var action = new ActionMethodInfo(
                    "my savings account balance is $balance".AsRegex(), 
                    new object(), 
                    GetDummyParameterInfo(), 
                    null);
                
                catalog.Add(action);
                var actionExists = catalog.ActionExists("Given my savings account balance is 500".AsActionStepText(""));

                Assert.That(actionExists, Is.True);
            }

            [Test]
            public void ShouldConsiderAllWhitespaceAsEqual()
            {
                var catalog = new ActionCatalog();

                var action = new ActionMethodInfo(
                    "my savings account\nbalance is $balance".AsRegex(),
                    new object(),
                    GetDummyParameterInfo(),
                    null);

                catalog.Add(action);
                var actionExists = catalog.ActionExists("Given my\tsavings account balance is 500".AsActionStepText(""));

                Assert.That(actionExists, Is.True);
            }

            [Test]
            public void ShouldGetAction()
            {
                var catalog = new ActionCatalog();

                var action = new ActionMethodInfo(
                    "my savings account balance is $balance".AsRegex(),
                    new object(),
                    GetDummyParameterInfo(),
                    null);

                catalog.Add(action);

                var actionResult = catalog.GetAction(new ActionStepText("Given my savings account balance is 500", ""));

                Assert.That(actionResult, Is.Not.Null);
            }

            [Test]
            public void ShouldGetActionWithTokenInMiddleOfString()
            {
                var catalog = new ActionCatalog();
                Action<int> action = accountBalance => { };

                var actionMethodInfo = new ActionMethodInfo(
                    "I have $amount euros on my cash account".AsRegex(),
                    action,
                    action.Method,
                    null);

                catalog.Add(actionMethodInfo);

                var actionFetched = catalog.GetAction(new ActionStepText("Given I have 20 euros on my cash account", ""));

                Assert.That(actionFetched, Is.Not.Null);
            }
        }

        [TestFixture]
        public class WhenTwoActionsMatchTheSameTextStep : ActionCatalogSpec
        {
            private ActionCatalog _actionCatalog;
            private bool _wasCalled;

            [SetUp]
            public void EstablishContext()
            {
                _wasCalled = false;
                _actionCatalog = new ActionCatalog();
                Action firstAction = () => Assert.Fail("This action shouldnt be called");
                var actionMethod = new ActionMethodInfo(new Regex("def$"), firstAction, firstAction.Method, "Given", this);
                _actionCatalog.Add(actionMethod);

                Action secondAction = () => { _wasCalled = true; };
                var secondActionMethod = new ActionMethodInfo(new Regex("abc def$"), secondAction, secondAction.Method, "Given", this);
                _actionCatalog.Add(secondActionMethod);
            }

            private void BecauseOf()
            {
                var actionText = new ActionStepText("Given abc def", "somestory.story");
                var action = _actionCatalog.GetAction(actionText);
                (action.Action as Action).Invoke();
            }

            [Test]
            public void ShouldCallGreediestMatchingAction()
            {
                BecauseOf();
                Assert.That(_wasCalled, Is.True);
            }
        }
    }
}