using NBehave.Examples.BowlingGame;
using NBehave.Narrator.Framework;
using NBehave.Spec.NUnit;

namespace NBehave.Examples.Bowling_Game.Specs
{
    [ActionSteps]
    public class GameSpecs
    {
        private Game _game;

        [ActionStep()]
        public void Given_a_game_of_bowling()
        {
            _game = new Game();
        }

        [ActionStep("When all my $rolls rolls are $pins")]
        [ActionStep("Given the rest of my $rolls roles are $pins")]
        public void RollMany(int rolls, int pins)
        {
            for (int i = 0; i < rolls; i++)
            {
                _game.Roll(pins);
            }
        }

        [ActionStep]
        public void Then_my_score_should_be_theScore(int theScore)
        {
            _game.Score().ShouldEqual(theScore);
        }

        [ActionStep("Given the first preceding role is $pins")]
        [ActionStep("Given the second preceding role is $pins")]
        public void Roll(int pins)
        {
            _game.Roll(pins);
        }

        [ActionStep]
        public void When_I_role_one_spare()
        {
            _game.Roll(5);
            _game.Roll(5);
        }

        [ActionStep]
        public void When_I_role_one_strike()
        {
            _game.Roll(10);
        }
    }
}
