using System;

namespace NBehave.Examples.GameOfLife
{
    public class Game
    {
        private readonly bool[,] _board;

        public Game(int width, int height)
        {
            _board = new bool[width, height];
        }

        public void ToggleCell(int x, int y)
        {
            _board[x, y] = _board[x, y] == false;
        }

        public override string ToString()
        {
            string board = string.Empty;

            for (int y = 0; y <= _board.GetUpperBound(1); y++)
            {
                for (int x = 0; x <= _board.GetUpperBound(0); x++)
                {
                    if (_board[x, y])
                        board += "X";
                    else
                        board += ".";
                }
                if (y < _board.GetUpperBound(1))
                    board += Environment.NewLine;
            }
            return board;
        }
    }
}
