using NBehave.Examples.BowlingGame;
using NBehave.Narrator.Framework;
using NBehave.Spec.NUnit;

namespace NBehave.Examples.Bowling_Game.Specs
{
    [ActionSteps]
    public class GameSteps
    {
        private Game _game;

        [Given]
        public void Given_a_game_of_bowling()
        {
            _game = new Game();
        }

        [Given(@"all my (?<rolls>\d+) rolls are (?<pins>\d+)$")]
        [When(@"the rest of my $rolls rolls are $pins")]
        public void RollMany(int rolls, int pins)
        {
            for (var i = 0; i < rolls; i++)
            {
                _game.Roll(pins);
            }
        }

        [Then]
        public void Then_my_score_should_be_theScore(int theScore)
        {
            _game.Score().ShouldEqual(theScore);
        }

        [Given(@"the first preceding roll is $pins")]
        [Given(@"the second preceding roll is (?<pins>\d+)$")]
        public void Roll(int pins)
        {
            _game.Roll(pins);
        }

        [When]
        public void When_I_roll_one_spare()
        {
            _game.Roll(5);
            _game.Roll(5);
        }

        [When]
        public void When_I_roll_one_strike()
        {
            _game.Roll(10);
        }
    }
}
