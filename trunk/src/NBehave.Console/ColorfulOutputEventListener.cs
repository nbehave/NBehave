using NBehave.Narrator.Framework;

namespace NBehave.Console
{
    public class ColorfulOutputEventListener : IEventListener
    {
        public void StoryCreated(string story)
        {
            System.Console.WriteLine(story);
        }

        public void StoryMessageAdded(string message)
        {
        }

        public void ScenarioCreated(string scenarioTitle)
        {
            System.Console.WriteLine("Scenario: " + scenarioTitle);
        }

        public void ScenarioMessageAdded(string message)
        {
            var color = System.ConsoleColor.Green;
            if (message.EndsWith("FAILED"))
                color = System.ConsoleColor.Red;
            if (message.EndsWith("PENDING"))
                color = System.ConsoleColor.Gray;

            WriteColorString(message, color);
        }

        public void RunStarted()
        { }

        public void RunFinished()
        {
            System.Console.WriteLine("");
        }

        public void ThemeStarted(string name)
        {
            WriteColorString(name, System.ConsoleColor.Yellow);
        }

        public void ThemeFinished()
        {
        }

        public void ScenarioResult(ScenarioResult result)
        { }

        public void StoryResults(FeatureResults results)
        { }

        private void WriteColorString(string text, System.ConsoleColor color)
        {
            var currentColor = System.Console.ForegroundColor;
            System.Console.ForegroundColor = color;
            System.Console.WriteLine(text);
            System.Console.ForegroundColor = currentColor;
        }
    }
}