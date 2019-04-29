using System;

namespace minesweeper_cli
{
    internal class Board
    {
        private readonly ushort _boardSize;
        private readonly ushort _totalMines;
        private bool[,] _mines;
        private UserState[,] _userStates;

        public Board(ushort boardSize, ushort totalMines)
        {
            if (boardSize * boardSize <= totalMines)
            {
                throw new ArgumentOutOfRangeException(nameof(totalMines), "Too many mines for such a board size");
            }

            _boardSize = boardSize;
            _totalMines = totalMines;
            _mines = new bool[_boardSize, _boardSize];
            _userStates = new UserState[_boardSize, _boardSize];

            Initialize();
        }

        private void Initialize()
        {
            var random = new Random();
            ushort minesPlaced = 0;

            do
            {
                var row = random.Next(_boardSize);
                var col = random.Next(_boardSize);
                if (!_mines[row, col])
                {
                    _mines[row, col] = true;
                    minesPlaced++;
                }
            } while (minesPlaced < _totalMines);
        }

        public void DrawBoard()
        {
            // output header
            Console.Write("   ");
            for (int col = 0; col < _boardSize; col++)
            {
                Console.Write($" {col} ");
            }
            Console.WriteLine();

            // output the table
            for (int row = 0; row < _boardSize; row++)
            {
                for (int col = 0; col < _boardSize; col++)
                {
                    if (col == 0)
                    {
                        Console.Write($" {row} ");
                    }
                    DrawCell(row, col);
                }
                Console.WriteLine();
            }
        }

        private void DrawCell(int row, int col)
        {
            if (_userStates[row, col] == UserState.Opened && _mines[row, col]) // mine
            {
                Console.Write(" * ");
            }
            else if (_userStates[row, col] == UserState.Opened && GetMinesNearby(row,col) == 0) // empty space
            {
                Console.Write("   ");
            } 
            else if (_userStates[row, col] == UserState.Opened) // number
            {
                Console.Write($" { GetMinesNearby(row,col) } ");
            }
            else if (_userStates[row, col] == UserState.Flagged) // flag
            {
                Console.Write(" F ");
            }
            else if (_userStates[row, col] == UserState.FogOfWar) // fog of war
            {
                Console.Write("\u2592\u2592\u2592");
            }
            else
            {
                Console.Write(" ? ");
            }
        }

        private int GetMinesNearby(int row, int col)
        {
            var mines = 0;

            if (row - 1 >= 0 && col - 1 >= 0 && _mines[row - 1, col - 1]) mines++;
            if (row - 1 >= 0 && _mines[row - 1, col]) mines++;
            if (row - 1 >= 0 && col + 1 < _boardSize && _mines[row - 1, col + 1]) mines++;

            if (col - 1 >= 0 && _mines[row, col - 1]) mines++;
            if (col + 1 < _boardSize && _mines[row, col + 1]) mines++;

            if (row + 1 < _boardSize && col - 1 >= 0 && _mines[row + 1, col - 1]) mines++;
            if (row + 1 < _boardSize && _mines[row + 1, col]) mines++;
            if (row + 1 < _boardSize && col + 1 < _boardSize && _mines[row + 1, col + 1]) mines++;

            return mines;
        }

        public void Process(UserInput userInput)
        {
            if (IsGameOver || userInput.UserAction == UserAction.Unknown)
            {
                return;
            }

            if (userInput.UserAction == UserAction.OpenSpace)
            {
                _userStates[userInput.Row, userInput.Col] = UserState.Opened;
            }
            else if (userInput.UserAction == UserAction.ToggleFlag)
            {
                if (_userStates[userInput.Row, userInput.Col] == UserState.Flagged)
                {
                    _userStates[userInput.Row, userInput.Col] = UserState.FogOfWar;
                }
                else if (_userStates[userInput.Row, userInput.Col] == UserState.FogOfWar)
                {
                    _userStates[userInput.Row, userInput.Col] = UserState.Flagged;
                }
            }
        }

        private bool IsWin
        {
            get
            {
                var win = true;
                for (int row = 0; row < _boardSize; row++)
                {
                    for (int col = 0; col < _boardSize; col++)
                    {
                        if (_mines[row, col] && _userStates[row, col] != UserState.Flagged)
                        {
                            win = false;
                        }
                    }
                }

                return win;
            }
        }

        private bool IsLoss
        {
            get
            {
                var loss = false;
                for (int row = 0; row < _boardSize; row++)
                {
                    for (int col = 0; col < _boardSize; col++)
                    {
                        if (_mines[row, col] && _userStates[row, col] == UserState.Opened)
                        {
                            loss = true;
                        }
                    }
                }

                return loss;
            }
        }

        public bool IsGameOver => IsWin || IsLoss;
      
        public void DrawStatus()
        {
            Console.WriteLine();
            if (IsGameOver)
            {
                if (IsWin)
                {
                    Console.WriteLine("   YOU WIN");
                }
                else
                {
                    Console.WriteLine("   YOU LOSE");
                }
                Console.WriteLine();
                Console.WriteLine("type 'exit' to exit");
            }
            else
            {
                Console.WriteLine("Make a choice:");
                Console.WriteLine("type 'exit' to exit");
                Console.WriteLine("type 'o ROW COL' to open a cell");
                Console.WriteLine("type 'f ROW COL' to flag a cell");
            }
        }
    }
}