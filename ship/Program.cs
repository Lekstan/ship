using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ship
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Battleships game");

            char[,] player1ShipBoard = InitializeBoard();
            char[,] player1TargetBoard = InitializeBoard();

            char[,] player2ShipBoard = InitializeBoard();
            char[,] player2TargetBoard = InitializeBoard();

            try
            {
                Console.WriteLine("Player 1, start placing your ships.");
                PlaceShips(player1ShipBoard);
                DisplayPlacementBoard(player1ShipBoard); 

                Console.Clear();

                Console.WriteLine("Player 2, start placing your ships.");
                PlaceShips(player2ShipBoard);
                DisplayPlacementBoard(player2ShipBoard); 

                Console.Clear();

                PlayGame(player1ShipBoard, player1TargetBoard, player2ShipBoard, player2TargetBoard);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Zle dane: {ex.Message}");
                Console.WriteLine("Wpisz ponownie");
                Console.ReadLine();
                Main(args); 
            }
        }

        static char[,] InitializeBoard()
        {
            char[,] board = new char[10, 10];

            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    board[i, j] = '-';
                }
            }

            return board;
        }

        static void PlaceShips(char[,] board)
        {
            var shipCounts = new Dictionary<int, int> { { 1, 4 }, { 2, 3 }, { 3, 2 }, { 4, 1 } };
            foreach (var kvp in shipCounts)
            {
                int shipLength = kvp.Key;
                int shipCount = kvp.Value;

                for (int i = 0; i < shipCount; i++)
                {
                    bool placedShip = false;
                    do
                    {
                        Console.Clear();
                        Console.WriteLine($"Place your {shipLength}-unit ship number {i + 1}");
                        DisplayPlacementBoard(board); 

                        Console.Write("Enter starting row (0-9): ");
                        int startRow = Convert.ToInt32(Console.ReadLine());

                        Console.Write("Enter starting column (0-9): ");
                        int startColumn = Convert.ToInt32(Console.ReadLine());

                        Console.Write("Choose orientation (h - horizontal, v - vertical): ");
                        char orientation = Convert.ToChar(Console.ReadLine());

                        if (CheckPlacement(board, startRow, startColumn, shipLength, orientation))
                        {
                            SetShip(board, startRow, startColumn, shipLength, orientation);
                            placedShip = true;
                        }
                        else
                        {
                            Console.WriteLine("Cannot place ship there. Try again.");
                        }
                    } while (!placedShip);
                }
            }
        }

        static bool CheckPlacement(char[,] board, int row, int column, int shipLength, char orientation)
        {
            if (orientation == 'h')
            {
                if (column + shipLength > 10)
                    return false;

                for (int i = column; i < column + shipLength; i++)
                {
                    if (board[row, i] != '-')
                        return false;
                }
            }
            else if (orientation == 'v')
            {
                if (row + shipLength > 10)
                    return false;

                for (int i = row; i < row + shipLength; i++)
                {
                    if (board[i, column] != '-')
                        return false;
                }
            }

            return true;
        }

        static void SetShip(char[,] board, int row, int column, int shipLength, char orientation)
        {
            if (orientation == 'h')
            {
                for (int i = column; i < column + shipLength; i++)
                {
                    board[row, i] = 'S';
                }
            }
            else if (orientation == 'v')
            {
                for (int i = row; i < row + shipLength; i++)
                {
                    board[i, column] = 'S';
                }
            }
        }

        static void PlayGame(char[,] player1ShipBoard, char[,] player1TargetBoard, char[,] player2ShipBoard, char[,] player2TargetBoard)
        {
            int player1Hits = 0;
            int player2Hits = 0;
            bool isPlayer1Turn = true;

            while (player1Hits < 20 && player2Hits < 20)
            {
                if (isPlayer1Turn)
                {
                    Console.WriteLine("Player 1, your turn:");
                    PlayerMove(player1TargetBoard, player2ShipBoard, ref player1Hits, player1ShipBoard);
                    isPlayer1Turn = false;
                }
                else
                {
                    Console.WriteLine("Player 2, your turn:");
                    PlayerMove(player2TargetBoard, player1ShipBoard, ref player2Hits, player2ShipBoard);
                    isPlayer1Turn = true;
                }
            }

            Console.WriteLine("Game over.");
        }

        static void PlayerMove(char[,] targetBoard, char[,] shipBoard, ref int hits, char[,] opponentShipBoard)
        {
            DisplayBoards(opponentShipBoard, targetBoard);

            Console.Write("Enter row to shoot (0-9): ");
            int shotRow = Convert.ToInt32(Console.ReadLine());

            Console.Write("Enter column to shoot (0-9): ");
            int shotColumn = Convert.ToInt32(Console.ReadLine());

            if (targetBoard[shotRow, shotColumn] != '-' && targetBoard[shotRow, shotColumn] != 'S')
            {
                Console.WriteLine("You've already shot here!");
                return;
            }

            if (shipBoard[shotRow, shotColumn] == 'S')
            {
                Console.WriteLine("Hit!");
                targetBoard[shotRow, shotColumn] = 'X';
                hits++;

                if (CheckSunkShip(shipBoard, shotRow, shotColumn))
                {
                    Console.WriteLine("You've sunk a ship!");
                }
            }
            else
            {
                Console.WriteLine("Miss!");
                targetBoard[shotRow, shotColumn] = 'O';
            }

            Console.Clear();
        }

        static bool CheckSunkShip(char[,] shipBoard, int row, int column)
        {
            int shipLength = 1;

            int left = column - 1;
            while (left >= 0 && shipBoard[row, left] == 'S')
            {
                shipLength++;
                left--;
            }

            int right = column + 1;
            while (right < 10 && shipBoard[row, right] == 'S')
            {
                shipLength++;
                right++;
            }

            if (shipLength >= 5)
            {
                return true;
            }

            shipLength = 1;

            int up = row - 1;
            while (up >= 0 && shipBoard[up, column] == 'S')
            {
                shipLength++;
                up--;
            }

            int down = row + 1;
            while (down < 10 && shipBoard[down, column] == 'S')
            {
                shipLength++;
                down++;
            }

            return shipLength >= 5;
        }

        static void DisplayBoards(char[,] shipBoard, char[,] targetBoard)
        {
            Console.WriteLine("Your target board:");
            DisplayBoard(targetBoard);

            Console.WriteLine("\nYour ship board:");
            DisplayBoard(shipBoard);
        }

        static void DisplayPlacementBoard(char[,] board)
        {
            Console.WriteLine("\nYour ship placement board:");
            DisplayBoard(board);
        }

        static void DisplayBoard(char[,] board)
        {
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    Console.Write(board[i, j] + " ");
                }
                Console.WriteLine();
            }
        }
    }
}
