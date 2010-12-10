using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using NUnit.Framework;

namespace NBehave.Narrator.Framework.Specifications.Text
{
    [TestFixture]
    public class StringTableStepSpec
    {
        public class WhenRunningStepWithTable : StringTableStepSpec
        {
            private ActionStepResult _actionStepResult;
            private IStringStepRunner _stringStepRunner;
            private ActionCatalog _actionCatalog;

            private List<string> _names;

            [SetUp]
            public virtual void SetUp()
            {
                _names = new List<string>();
                _actionCatalog = new ActionCatalog();
                Action<string> action = name => _names.Add(name); //Depending on the parametername of the action
                var stringStep = new Regex("I have a  list of names:");
                var actionMethodInfo = new ActionMethodInfo(stringStep, action, action.Method, "Given");
                _actionCatalog.Add(actionMethodInfo);

                _stringStepRunner = new StringStepRunner(_actionCatalog);
                var tableStep = new StringTableStep("Given " + stringStep, "file", _stringStepRunner);
                var columnNames = new ExampleColumns(new[] { "name", "country" });

                tableStep.AddTableStep(new Row(columnNames, new Dictionary<string, string> { { "name", "Morgan Persson" }, { "country", "Sweden" } }));
                tableStep.AddTableStep(new Row(columnNames, new Dictionary<string, string> { { "name", "Jimmy Nilsson" }, { "country", "Sweden" } }));
                tableStep.AddTableStep(new Row(columnNames, new Dictionary<string, string> { { "name", "Jimmy Bogard" }, { "country", "USA" } }));

                tableStep.Run();
                _actionStepResult = tableStep.StepResult;
            }

            [Test]
            public void StepShouldPass()
            {
                Assert.That(_actionStepResult.Result, Is.TypeOf(typeof(Passed)));
            }

            [Test]
            public void ShouldCallStepThreeTimes()
            {
                Assert.That(_names.Count, Is.EqualTo(3));
            }

            [Test]
            public void ListOfUsersShouldContainJimmyNilssonFromSweden()
            {
                CollectionAssert.Contains(_names, "Jimmy Nilsson");
            }
        }
    }
}