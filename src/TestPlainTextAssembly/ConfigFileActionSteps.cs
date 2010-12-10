using System.Configuration;
using NBehave.Spec.NUnit;
using NBehave.Narrator.Framework;

namespace TestPlainTextAssembly
{
    [ActionSteps]
    public class ConfigFileActionSteps
    {
        private string _configValue;

        [Given(@"an assembly with a matching configuration file")]
        public void DoNothing(string action, string person)
        {
        }

        [When(@"the value of setting $key is read")]
        public void ReadConfigValue(string key)
        {
            _configValue = ConfigurationManager.AppSettings[key];
        }

        [Then("the value should be $val")]
        public void ActionPerformed(string actionPerformed)
        {
            _configValue.ShouldEqual(actionPerformed);
        }
    }
}