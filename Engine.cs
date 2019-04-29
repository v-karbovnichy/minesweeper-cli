using System;

namespace minesweeper_cli
{
    class Engine
    {
        static void Main(string[] args)
        {
            var board = new Board(boardSize: 6, totalMines: 6);

            UserInput userInput;
            do
            {
                // clear screen 
                Console.Clear();

                // show the board
                board.DrawBoard();

                board.DrawStatus();

                // get user input
                var rawUserInput = Console.ReadLine();
                userInput = UserInput.FromString(rawUserInput);

                // modify board according to user input
                board.Process(userInput);

            } while (!userInput.DidUserAskToExit);
        }
    }
}
