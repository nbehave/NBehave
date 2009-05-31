using NBehave.Examples.BowlingGame;
using NBehave.Narrator.Framework;
using NBehave.Spec.NUnit;

namespace NBehave.Examples.Bowling_Game.Specs
{
    [ActionSteps]
    public class GameSpecs
    {
        private Game _game;

        [ActionStep("Given a game of bowling")]
        public void Given_a_game_of_bowling()
        {
            _game = new Game();
        }

        [ActionStep("When all my $rolls rolls are $pins")]
        public void RollMany(int rolls, int pins)
        {
            for (int i = 0; i < rolls; i++)
            {
                _game.Roll(pins);
            }
        }

        [ActionStep("And the rest of my $rolls roles are $pins")]
        public void RollMany_again(int rolls, int pins)
        {
            RollMany(rolls, pins);
        }

        [ActionStep("Then my score should be $score")]
        public void Then_my_score_should_be(int score)
        {
            _game.Score().ShouldEqual(score);
        }

        [ActionStep("When I role one strike")]
        public void RollStrike()
        {
            _game.Roll(10);
        }

        [ActionStep("And the first preceding role is $pins")]
        public void Roll(int pins)
        {
            _game.Roll(pins);
        }

        [ActionStep("When I role one spare")]
        public void RollSpare()
        {
            _game.Roll(5);
            _game.Roll(5);
        }

        [ActionStep("Given the first preceding role is 3")]
        public void RollOne(int pins)
        {
            _game.Roll(3);
        }

        [ActionStep("And the second preceding role is $pins")]
        public void RollSecondOne(int pins)
        {
            _game.Roll(pins);
        }
    }
}
