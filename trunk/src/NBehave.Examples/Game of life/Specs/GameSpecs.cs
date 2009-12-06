using NBehave.Narrator.Framework;
using NBehave.Spec.NUnit;

namespace NBehave.Examples.GameOfLife.Specs
{
    [ActionSteps]
    public class GameSpecs
    {
        private Game _game;

        [Given(@"a (?<width>\d+) by (?<height>\d+) game")]
        [Given(@"a new game: (?<width>\d+) by (?<height>\d+)")]
        public void GameOfLife(int width, int height)
        {
            _game = new Game(width, height);
        }

        [When(@"I toggle the cell at \((?<x>\d+), (?<y>\d+)\)")]
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
