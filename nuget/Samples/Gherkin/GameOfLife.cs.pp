using System;

namespace  $rootnamespace$.Gherkin
{
    public class GameOfLife
    {
        private readonly bool[,] board;

        public GameOfLife(int width, int height)
        {
            board = new bool[width, height];
        }

        public void ToggleCell(int x, int y)
        {
            board[x, y] = board[x, y] == false;
        }

        public override string ToString()
        {
            var board = string.Empty;

            for (var y = 0; y <= this.board.GetUpperBound(1); y++)
            {
                for (var x = 0; x <= this.board.GetUpperBound(0); x++)
                {
                    if (this.board[x, y])
                        board += "X";
                    else
                        board += ".";
                }
                if (y < this.board.GetUpperBound(1))
                    board += Environment.NewLine;
            }
            return board;
        }
    }
}
