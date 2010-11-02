using System.Collections.Generic;
using System.Linq;
using NBehave.Narrator.Framework;
using NBehave.Spec.NUnit;

namespace NBehave.Examples.Tables
{
    public class People
    {
        public string Name { get; set; }
        public string Country { get; set; }
    }

    [ActionSteps]
    public class Table
    {
        private List<People> _people;
        private List<People> _found;
        [BeforeScenario]
        public void Init()
        {
            _people = new List<People>();
        }

        [Given("a list of people:")]
        public void List_of_people(string name, string country)
        {
            _people.Add(new People { Name = name.Trim(), Country = country.Trim() });
        }

        [When(@"I search for people from $country")]
        public void SearchByCountry(string country)
        {
            _found = _people.Where(p => p.Country == country).ToList();
        }

        [Then(@"I should find:")]
        public void Should_Find(string name, string country)
        {
            var found = _found.Where(p => p.Name == name.Trim() && p.Country == country.Trim());
            found.Count().ShouldEqual(1);
        }
    }
}
