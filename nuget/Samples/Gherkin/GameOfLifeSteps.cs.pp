using NBehave;
using NBehave.Spec.NUnit;

namespace $rootnamespace$.Gherkin
{
    [ActionSteps]
    public class GameOfLifeSteps
    {
        private GameOfLife game;

        [Given(@"a $width by $height game")]
        [Given(@"a new game: $width by $height")]
        public void GameOfLife(int width, int height)
        {
            game = new GameOfLife(width, height);
        }

        [When(@"I toggle the cell at ($x, $y)")]
        public void ToggleCell(int x, int y)
        {
            game.ToggleCell(x, y);
        }

        [Then(@"the grid should look like\s+(?<rows>(.+\s*)+)")]
        [Then(@"the grid should be\s+(?<rows>(.+\s*)+)")]
        public void GameLooksLike(string rows)
        {
            game.ToString().ShouldEqual(rows);
        }
    }
}
