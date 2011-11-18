sing System.Collections.Generic;
using NBehave.Narrator.Framework;

namespace $rootnamespace$
{
	[ActionSteps]
	public class ExampleSteps
	{
		[Given("an empty list")]
		public void EmptyList()
		{
			ScenarioContext.Current.Add("list", new List<string>());
		}

        [When("I add $x to list")]
        public void AddToList(string x)
        {
            var list = ScenarioContext.Current.Get<List<string>>("list");
            list.Add(x);
        }

        [Then("the list should contain $y")]
        public void ListShouldContain(string y)
        {
            // To get this step to pass, add reference to nunit (ex via nuget: Install-Package nunit)
            // remove the Step.Pend line and uncomment the other 2 lines of code
            Step.Pend("not implemented");
            //var list = ScenarioContext.Current.Get<List<string>>("list");
            //NUnit.Framework.CollectionAssert.Contains(list, y);              
        }
	}
}