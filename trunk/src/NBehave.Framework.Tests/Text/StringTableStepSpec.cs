using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using NUnit.Framework;

namespace NBehave.Narrator.Framework.Specifications.Text
{
    [TestFixture]
    public class StringTableStepSpec
    {
        public class When_running_step_with_table : StringTableStepSpec
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
                Action<string> action = name => { _names.Add(name); }; //Depending on the parametername of the action
                var stringStep = new Regex("I have a  list of names:");
                var actionMethodInfo = new ActionMethodInfo(stringStep, action, action.Method, "Given");
                _actionCatalog.Add(actionMethodInfo);

                _stringStepRunner = new StringStepRunner(_actionCatalog);
                var tableStep = new StringTableStep("Given " + stringStep, "file", _stringStepRunner);
                var columnNames = new ExampleColumns(new[] { "name", "country" });

                tableStep.AddTableStep(new Row(columnNames, new Dictionary<string, string> { { "name", "Morgan Persson" }, { "country", "Sweden" } }));
                tableStep.AddTableStep(new Row(columnNames, new Dictionary<string, string> { { "name", "Jimmy Nilsson" }, { "country", "Sweden" } }));
                tableStep.AddTableStep(new Row(columnNames, new Dictionary<string, string> { { "name", "Jimmy Bogard" }, { "country", "USA" } }));

                _actionStepResult = tableStep.Run();
            }

            [Test]
            public void Step_should_pass()
            {
                Assert.That(_actionStepResult.Result, Is.TypeOf(typeof(Passed)));
            }

            [Test]
            public void Should_call_step_three_times()
            {
                Assert.That(_names.Count, Is.EqualTo(3));
            }

            [Test]
            public void List_of_users_should_contain_Jimmy_Nilsson_from_sweden()
            {
                CollectionAssert.Contains(_names, "Jimmy Nilsson");
            }
        }
    }
}