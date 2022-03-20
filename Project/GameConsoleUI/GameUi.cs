using System;
using System.Data;
using System.IO;
using System.Net;
using BLL;
using BLL.BoardObjects;
using BLL.ShipObjects;
using DAL;
using MenuSystem;

namespace GameConsoleUI
{
    public class GameUi
    {
        public static GameBrain? GameBrain;

        
        
        public static void DrawBoard(BoardState boardState, bool hideShips)
        {
            CellState[,] board = boardState.Board;
            var width = board.GetUpperBound(0) + 1; // x
            var height = board.GetUpperBound(1) + 1; // y
            char[] letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();


            Console.Write(" ");
            for (int colIndex = 0; colIndex < width; colIndex++)
            {
                Console.Write($"  {letters[colIndex]}  ");
            }

            Console.WriteLine();
            Console.Write(" ");
            for (int colIndex = 0; colIndex < width; colIndex++)
            {
                Console.Write($"+---+");
            }

            Console.WriteLine();

            for (int rowIndex = 0; rowIndex < height; rowIndex++)
            {
                Console.Write($" ");
                for (int colIndex = 0; colIndex < width; colIndex++)
                {
                    if (hideShips && board[colIndex, rowIndex] == CellState.Ship)
                    {
                        Console.Write($"|   |");
                    }
                    else
                    {
                        Console.Write($"| {CellString(board[colIndex, rowIndex])} |");
                    }
                }

                Console.Write($" {rowIndex + 1}");

                Console.WriteLine();
                Console.Write(" ");
                for (int colIndex = 0; colIndex < width; colIndex++)
                {
                    Console.Write($"+---+");
                }

                Console.WriteLine();
            }

            Console.WriteLine(boardState.PlayerName + " Board");
        }

        public static void DrawBoards(BoardState board1, BoardState board2)
        {
            Console.Clear();
            DrawBoard(board1, true);
            DrawBoard(board2, false);
        }


        public static string Play(GameBrain gameBrain)
        {
            var rowIndex = 2;
            var colIndex = 3;

            // var noMovesYetMade = true;
            // if (noMovesYetMade)
            // {
            //     Console.SetCursorPosition(colIndex, rowIndex);
            //     Console.BackgroundColor = ConsoleColor.Green;
            //     Console.Write(" ");
            //     Console.ResetColor();
            //     noMovesYetMade = false;
            // }

            var pressedKey = Console.ReadKey().Key;

            var firstKeyPress = true;
            do
            {
                var lastColIndex = colIndex;
                var lastRowIndex = rowIndex;
                if (!firstKeyPress)
                {
                    pressedKey = Console.ReadKey().Key;
                }

                firstKeyPress = false;

                switch (pressedKey)
                {
                    case ConsoleKey.DownArrow:
                        rowIndex += 2;
                        break;
                    case ConsoleKey.UpArrow:
                        rowIndex += -2;
                        break;
                    case ConsoleKey.LeftArrow:
                        colIndex += -5;
                        break;
                    case ConsoleKey.RightArrow:
                        colIndex += 5;
                        break;
                    case ConsoleKey.Enter:
                        
                        gameBrain.SaveCurrentState();

                        gameBrain.MakeAMove(colIndex / 5, (rowIndex / 2) - 1);


                        // gameBrain.SaveCurrentState();
                        
                        
                        if (gameBrain.AllShipsSunk(gameBrain.GetBoardStates().attackingPlayerState))
                        {
                            DisplayEndScreen(gameBrain);
                            return "";
                        }


                        DisplayMoveResult(gameBrain, colIndex / 5, (rowIndex / 2) - 1);

                        rowIndex = 2;
                        colIndex = 3;

                        DrawBoards(gameBrain.GetBoardStates().defendingPlayerState,
                            gameBrain.GetBoardStates().attackingPlayerState);
                        break;
                    case ConsoleKey.S:
                        GameBrain = gameBrain; 
                        Console.Clear();
                        var presetCommands = new MenuCommands();
                        var menuA = new Menu(Menu.MenuLevel.Level0, presetCommands, "Pause Menu");
                        menuA.AddMenuItem(new MenuItem("Save game to JSON", SaveGameAction));
                        menuA.AddMenuItem(new MenuItem("Save game to Database", SaveGameDb));
                        menuA.AddMenuItem(new MenuItem("Exit Menu", presetCommands.Exit));
                        menuA.RunMenu();

                        // SaveGameAction(gameBrain);
                        // SaveGameDb(gameBrain);
                        // Console.Clear();
                        DrawBoards(gameBrain.GetBoardStates().defendingPlayerState,
                            gameBrain.GetBoardStates().attackingPlayerState);
                        break;
                    // case ConsoleKey.Backspace:
                    //     Console.Clear();
                    //     return "";
                    case ConsoleKey.A:
                        try
                        {
                            gameBrain.GetLastGameState();
                        }
                        catch (Exception)
                        {
                            Console.WriteLine("Nothing more to rollback!");
                        }
                        
                        DrawBoards(gameBrain.GetBoardStates().defendingPlayerState,
                            gameBrain.GetBoardStates().attackingPlayerState);
                        break;
                }

                if (colIndex < 3 || colIndex / 5 >= gameBrain.GetCurrentPlayerBoard().GetLength(0))
                {
                    colIndex = lastColIndex;
                }

                if (rowIndex < 2 || rowIndex / 2 > gameBrain.GetCurrentPlayerBoard().GetLength(1))
                {
                    rowIndex = lastRowIndex;
                }

                var cellState = (gameBrain.GetCurrentPlayerBoard()[colIndex / 5, (rowIndex / 2) - 1] == CellState.Ship)
                    ? CellString(CellState.Empty)
                    : CellString(gameBrain.GetCurrentPlayerBoard()[colIndex / 5, (rowIndex / 2) - 1]);
                var lastCellState =
                    (gameBrain.GetCurrentPlayerBoard()[lastColIndex / 5, (lastRowIndex / 2) - 1] == CellState.Ship)
                        ? CellString(CellState.Empty)
                        : CellString(gameBrain.GetCurrentPlayerBoard()[lastColIndex / 5, (lastRowIndex / 2) - 1]);
                if (pressedKey != ConsoleKey.Enter)
                {
                    Console.ResetColor();
                    Console.SetCursorPosition(lastColIndex, lastRowIndex);
                    Console.Write(lastCellState);
                    Console.SetCursorPosition(colIndex, rowIndex);
                    Console.BackgroundColor = ConsoleColor.Green;
                    Console.Write(cellState);
                    Console.ResetColor();
                }
            } while (pressedKey != ConsoleKey.Backspace);

            Console.Clear();
            return "";
        }

        private static void DisplayMoveResult(GameBrain gameBrain, int col, int row)
        {
            Console.Clear();
            var board = gameBrain.GetBoardStates().attackingPlayerState.Board;
            if (board[col, row] == CellState.Hit) //hit
            {
                Console.WriteLine("HIT!");
                var hitShip = gameBrain.GetShipFromPosition(row, col, gameBrain.GetBoardStates().attackingPlayerState);
                Console.WriteLine($"Hit ship stats:");
                Console.WriteLine($"Ship name: {hitShip.Name}");
                Console.WriteLine($"Ship lenght: {hitShip.Lenght}");
                Console.WriteLine($"Ship height: {hitShip.Height}");
                Console.WriteLine($"Ship damage count: {hitShip.GetShipDamageCount(board)}");
                Console.WriteLine($"Ship sunk: {hitShip.IsShipSunk(board)}");
                Console.WriteLine($"Number of ships sunk: {GameBrain.GetNrOfSunkShips(gameBrain.GetBoardStates().attackingPlayerState)}");
                Console.WriteLine($"Number of ships left: {GameBrain.GetNrOfShipsLeft(gameBrain.GetBoardStates().attackingPlayerState)}");


            }
            else
            {
                Console.WriteLine("MISS!");
            }


            Console.WriteLine("Change players:");
            Console.WriteLine($"{gameBrain.GetBoardStates().defendingPlayerState.PlayerName} please give computer " +
                              $"over to {gameBrain.GetBoardStates().attackingPlayerState.PlayerName}");
            Console.WriteLine("Once ready, press enter >");
            Console.ReadLine();
        }

        private static void DisplayEndScreen(GameBrain gameBrain)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(">");
            Console.WriteLine($"{gameBrain.GetBoardStates().defendingPlayerState.PlayerName} is the winner!");
            Console.WriteLine(">");
            DrawBoard(gameBrain.GetBoardStates().defendingPlayerState, false);
            DrawBoard(gameBrain.GetBoardStates().attackingPlayerState, false);
            Console.WriteLine("Press enter to return to main menu >");
            Console.ResetColor();
            Console.ReadLine();
        }

        static string SaveGameAction()
        {
            Console.Clear();
            string workingDirectory = Environment.CurrentDirectory;
            if (workingDirectory.Contains("bin"))
            {
                workingDirectory = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, @"..\..\..\"));
            }
            var defaultName = "save_" + DateTime.Now.ToString("yyyy-MM-dd") + ".json";
            Console.WriteLine($"Current save name ({defaultName})");
            Console.WriteLine("Enter new save name or press enter:");
            var fileName = Console.ReadLine();
            fileName = string.IsNullOrWhiteSpace(fileName) ? defaultName : WebUtility.UrlEncode(fileName + ".json");

            string path = workingDirectory + Path.DirectorySeparatorChar + "SaveGames" + Path.DirectorySeparatorChar +
                          fileName;
            Console.WriteLine(path);

            var serializedGame = GameBrain!.GetSerializedGameState();

            System.IO.File.WriteAllText(path, serializedGame);
            return "";
        
        }

        private static string SaveGameDb()
        {
            Console.Clear();
            var defaultName = "save_" + DateTime.Now.ToString("yyyy-MM-dd") + ".json";
            Console.WriteLine($"Current save name ({defaultName})");
            Console.WriteLine("Enter new save name or press enter:");
            var fileName = Console.ReadLine();
            fileName = string.IsNullOrWhiteSpace(fileName) ? defaultName : WebUtility.UrlEncode(fileName + ".json");

            using var db = new ApplicationDbContext();
            var dto = GameBrain!.GetSaveDto();
            dto.GameName = fileName;
            db.Games.Add(dto);
            db.SaveChanges();


            var shipsDto = GameBrain!.GetShipsDto(GameBrain!.Player1State.Ships, GameBrain!.Player2State.Ships , dto.GameId);

            foreach (var ship in shipsDto)
            {
                db.Ships.Add(ship);
            }
            db.SaveChanges();
            
            return "";

        }

        private static string CellString(CellState cellState)
        {
            return cellState switch
            {
                CellState.Empty => " ",
                CellState.Bomb => "O",
                CellState.Ship => "A",
                CellState.Hit => "X",
                _ => ""
            };
        }

        public static void PlaceShips(GameBrain gameBrain, BoardState board, bool displayError = false)
        {
            do
            {
                try
                {
                    var shipConfig = gameBrain.GetShip();
                    PlaceShip(gameBrain, shipConfig, board, displayError);
                    displayError = false;
                }
                catch (InvalidOperationException)
                {
                    Console.Clear();
                    return;
                }
                catch (InvalidConstraintException)
                {
                    Console.Clear();
                    throw new InvalidConstraintException();
                }
                catch (Exception)
                {
                    PlaceShips(gameBrain, board, true);
                }

                try
                {
                    gameBrain.PopShip();
                }
                catch (InvalidOperationException)
                {
                    Console.Clear();
                    return;
                }
            } while (true);
        }

        private static void PlaceShip(GameBrain gameBrain, ShipConfig shipConfig, BoardState board, bool displayError)
        {
            Console.Clear();
            DrawBoard(board, false);
            Console.WriteLine(
                $"Currently placing ship of type: {shipConfig.Name}, lenght: {shipConfig.ShipSizeX}, width: {shipConfig.ShipSizeY}");
            Console.WriteLine(
                "Use arrow keys to select ship starting point. Press enter and choose direction");
            if (displayError)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(
                    "Invalid position for ship!");
                Console.ResetColor();
            }

            var rowIndex = 2;
            var colIndex = 3;

            var noMovesYetMade = true;
            if (noMovesYetMade)
            {
                Console.SetCursorPosition(colIndex, rowIndex);
                Console.BackgroundColor = ConsoleColor.Green;
                Console.Write(" ");
                Console.ResetColor();
                noMovesYetMade = false;
            }

            var pressedKey = Console.ReadKey().Key;

            var firstKeyPress = true;
            do
            {
                var lastColIndex = colIndex;
                var lastRowIndex = rowIndex;
                if (!firstKeyPress)
                {
                    pressedKey = Console.ReadKey().Key;
                }

                firstKeyPress = false;

                switch (pressedKey)
                {
                    case ConsoleKey.DownArrow:
                        rowIndex += 2;
                        break;
                    case ConsoleKey.UpArrow:
                        rowIndex += -2;
                        break;
                    case ConsoleKey.LeftArrow:
                        colIndex += -5;
                        break;
                    case ConsoleKey.RightArrow:
                        colIndex += 5;
                        break;
                    case ConsoleKey.Enter:
                        var coordinates = new Coordinate();
                        coordinates.X = (rowIndex / 2) - 1;
                        coordinates.Y = colIndex / 5;
                        var direction = GetDirection(Console.ReadKey().Key);
                        gameBrain.PlaceAShip(shipConfig, colIndex / 5, (rowIndex / 2) - 1, direction, board);
                        rowIndex = 2;
                        colIndex = 3;
                        break;
                    case ConsoleKey.Backspace:
                        Console.Clear();
                        throw new InvalidConstraintException();
                }

                if (colIndex < 3 || colIndex / 5 >= gameBrain.GetCurrentPlayerBoard().GetLength(0))
                {
                    colIndex = lastColIndex;
                }

                if (rowIndex < 2 || rowIndex / 2 > gameBrain.GetCurrentPlayerBoard().GetLength(1))
                {
                    rowIndex = lastRowIndex;
                }

                var cellState = CellString(board.Board[colIndex / 5, (rowIndex / 2) - 1]);
                var lastCellState = CellString(board.Board[lastColIndex / 5, (lastRowIndex / 2) - 1]);
                if (pressedKey != ConsoleKey.Enter)
                {
                    Console.ResetColor();
                    Console.SetCursorPosition(lastColIndex, lastRowIndex);
                    Console.Write(lastCellState);
                    Console.SetCursorPosition(colIndex, rowIndex);
                    Console.BackgroundColor = ConsoleColor.Green;
                    Console.Write(cellState);
                    Console.ResetColor();
                }
            } while (pressedKey != ConsoleKey.Enter);

            Console.Clear();
        }

        private static Direction GetDirection(ConsoleKey key)
        {
            return key switch
            {
                ConsoleKey.DownArrow => Direction.Down,
                ConsoleKey.UpArrow => Direction.Up,
                ConsoleKey.LeftArrow => Direction.Left,
                ConsoleKey.RightArrow => Direction.Right,
                _ => Direction.Up
            };
        }
    }
}