using NBehave.Spec.NUnit;
using NBehave.Narrator.Framework;

namespace TestPlainTextAssembly
{
    [ActionSteps]
    public class GreetingSystemActionSteps
    {
        private GreetingSystem _greetingSystem;
        private string _greeting;

        [Given("my name is $name")]
        public void Given_my_name_is(string name)
        {
            _greetingSystem = new GreetingSystem();
            _greetingSystem.GiveName(name);
        }

        [When("I'm greeted")]
        public void When_Im_greeted()
        {
            _greeting = _greetingSystem.Greeting();
        }

        [Then("I should be greeted with “Hello, $name!”")]
        public void Then_I_should_be_greeted(string name)
        {
            _greeting.ShouldEqual(string.Format("“Hello, {0}!”", name));
        }
    }

    public class GreetingSystem
    {
        private string _name;

        public void GiveName(string name)
        {
            _name = name;
        }

        public string Greeting()
        {
            return string.Format("“Hello, {0}!”", _name);
        }
    }
}
