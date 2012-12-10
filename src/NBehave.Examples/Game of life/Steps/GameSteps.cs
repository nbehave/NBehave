
using NBehave.Spec.NUnit;

namespace NBehave.Examples.GameOfLife.Specs
{
    [ActionSteps]
    public class GameSteps
    {
        private Game _game;

        [Given(@"a $width by $height game")]
        [Given(@"a new game: $width by $height")]
        public void GameOfLife(int width, int height)
        {
            _game = new Game(width, height);
        }

        [When(@"I toggle the cell at ($x, $y)")]
        public void ToggleCell(int x, int y)
        {
            _game.ToggleCell(x, y);
        }

        [Then(@"the grid should look like\s+(?<rows>(.+\s*)+)")]
        [Then(@"the grid should be\s+(?<rows>(.+\s*)+)")]
        public void GameLooksLike(string rows)
        {
            _game.ToString().ShouldEqual(rows);
        }
    }
}
