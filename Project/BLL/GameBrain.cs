using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.Json;
using BLL.BoardObjects;
using BLL.Config;
using BLL.ShipObjects;
using Domain;
using Coordinate = BLL.ShipObjects.Coordinate;
using Ship = BLL.ShipObjects.Ship;
using ShipConfig = BLL.ShipObjects.ShipConfig;

namespace BLL
{
    public class GameBrain
    {
        public GameConfig Config { get; }
        public BoardState Player1State;
        public BoardState Player2State;


        private bool _moveByPlayer1 = true;
        public int BoardWidth;
        public int BoardHeight;

        private Stack<ShipConfig> _ships;
        private readonly Stack<RollBackObject> _state = new Stack<RollBackObject>();


        public void SwapPlayers()
        {
            _moveByPlayer1 = !_moveByPlayer1;
        }

        public GameBrain(GameConfig config)
        {
            Config = config;
            _ships = ReadShipsFromConfig(config.ShipConfigs);
            BoardHeight = config.BoardSizeY;
            BoardWidth = config.BoardSizeX;
            Player1State = new BoardState(new CellState[BoardWidth, BoardHeight], config.Player1Name);
            Player2State = new BoardState(new CellState[BoardWidth, BoardHeight], config.Player2Name);
        }

        private static Stack<ShipConfig> ReadShipsFromConfig(IEnumerable<ShipConfig> configShipConfigs)
        {
            Stack<ShipConfig> shipsConfigs = new Stack<ShipConfig>();
            foreach (var shipConfig in configShipConfigs)
            {
                for (var i = 0; i < shipConfig.Quantity; i++)
                {
                    shipsConfigs.Push(shipConfig);
                }
            }

            return shipsConfigs;
        }

        public ShipConfig GetShip()
        {
            return _ships.Peek();
        }

        public void PopShip()
        {
            _ships.Pop();
        }

        public static int GetNrOfSunkShips(BoardState boardState)
        {
            return boardState.Ships.Count(ship => ship.IsShipSunk(boardState.Board));
        }

        public CellState[,] GetCurrentPlayerBoard()
        {
            CellState[,] currentBoard = _moveByPlayer1 ? Player2State.Board : Player1State.Board;
            var res = new CellState[BoardWidth, BoardHeight];
            Array.Copy(currentBoard, res, Player2State.Board.Length);
            return res;
        }


        public CellState MakeAMove(int col, int row)
        {
            CellState[,] currentBoard = _moveByPlayer1 ? Player2State.Board : Player1State.Board;

            if (currentBoard[col, row] == CellState.Empty)
            {
                currentBoard[col, row] = CellState.Bomb;
                _moveByPlayer1 = !_moveByPlayer1;
                return CellState.Bomb;
            }
            else if (currentBoard[col, row] == CellState.Ship)
            {
                currentBoard[col, row] = CellState.Hit;
                _moveByPlayer1 = !_moveByPlayer1;
                return CellState.Hit;
            }

            _moveByPlayer1 = !_moveByPlayer1;
            return CellState.Empty;
        }

        public Ship GetShipFromPosition(int row, int col, BoardState board)
        {
            var shipsList = board.Ships;
            foreach (var ship in shipsList)
            {
                if (ship.IsAtCoordinates(row, col))
                {
                    return ship;
                }
            }

            return null!;
        }

        public bool PlaceAShip(ShipConfig ship, int colStart, int rowStart, Direction direction, BoardState board)
        {
            int colEnd = colStart;
            int rowEnd = rowStart;
            switch (direction)
            {
                case Direction.Up:
                    rowStart += -ship.ShipSizeX + 1;
                    colEnd += ship.ShipSizeY - 1;

                    break;
                case Direction.Down:
                    rowEnd += ship.ShipSizeX - 1;
                    colEnd += ship.ShipSizeY - 1;

                    break;
                case Direction.Left:
                    colStart += -ship.ShipSizeX + 1;
                    rowEnd += ship.ShipSizeY - 1;

                    break;
                case Direction.Right:
                    colEnd += ship.ShipSizeX - 1;
                    rowEnd += ship.ShipSizeY - 1;
                    break;
            }

            if (!CanPlaceShip(colStart, colEnd, rowStart, rowEnd, board))
            {
                throw new EvaluateException();
            }

            var shipObj = new Ship(ship.Name!, ship.ShipSizeX, ship.ShipSizeY);
            board.Ships.Add(shipObj);

            for (int i = rowStart; i < rowEnd + 1; i++)
            {
                for (int j = colStart; j < colEnd + 1; j++)
                {
                    board.Board[j, i] = CellState.Ship;
                    shipObj.Coordinates!.Add(new Coordinate()
                    {
                        X = i,
                        Y = j
                    });
                }
            }

            // if (currentBoard[col, row] == CellState.Empty)
            // {
            //     currentBoard[col, row] = CellState.Bomb;
            //     _moveByPlayer1 = !_moveByPlayer1;
            //     return true;
            // }
            return true;
        }

        private bool CanPlaceShip(int colStart, int colEnd, int rowStart, int rowEnd, BoardState board)
        {
            for (int row = rowStart; row < rowEnd + 1; row++)
            {
                for (int col = colStart; col < colEnd + 1; col++)
                {
                    if (board.Board[col, row] != CellState.Empty)
                    {
                        return false;
                    }

                    if (Config.EShipTouchRule == EShipTouchRule.NoTouch)
                    {
                        if (SidesTouching(col, row, board))
                        {
                            return false;
                        }

                        if (CornersTouching(rowStart, rowEnd, colStart, colEnd, board))
                        {
                            return false;
                        }
                    }
                    else if (Config.EShipTouchRule == EShipTouchRule.CornerTouch)
                    {
                        if (SidesTouching(col, row, board))
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        private bool CornersTouching(int rowStart, int rowEnd, int colStart, int colEnd, BoardState board)
        {
            return !(IsEmptyCell(colStart - 1, rowStart - 1, board)
                     && IsEmptyCell(colStart - 1, rowStart + 1, board)
                     && IsEmptyCell(colStart + 1, rowStart - 1, board)
                     && IsEmptyCell(colStart + 1, rowStart + 1, board)
                     && IsEmptyCell(colEnd + 1, rowEnd - 1, board)
                     && IsEmptyCell(colEnd + 1, rowEnd + 1, board)
                     && IsEmptyCell(colEnd - 1, rowEnd - 1, board)
                     && IsEmptyCell(colEnd - 1, rowEnd + 1, board));
        }


        private bool SidesTouching(int col, int row, BoardState board)
        {
            return !(IsEmptyCell(col + 1, row, board)
                     && IsEmptyCell(col - 1, row, board)
                     && IsEmptyCell(col, row + 1, board)
                     && IsEmptyCell(col, row - 1, board));
        }

        private bool IsEmptyCell(int col, int row, BoardState board)
        {
            try
            {
                return board.Board[col, row] == CellState.Empty;
            }
            catch (Exception)
            {
                return true;
            }
        }


        public (BoardState defendingPlayerState, BoardState attackingPlayerState) GetBoardStates()
        {
            BoardState attackingPlayer = _moveByPlayer1 ? Player2State : Player1State;
            BoardState defendingPlayer = _moveByPlayer1 ? Player1State : Player2State;
            return (attackingPlayer, defendingPlayer);
        }

        public string GetSerializedGameState()
        {


            var state = new SaveGameDto
            {
                MoveByPlayer = _moveByPlayer1,
                Width = BoardWidth,
                Height = BoardHeight,
                Player1Name = Player1State.PlayerName,
                Player2Name = Player2State.PlayerName,
                Player1Ships = Player1State.Ships,
                Player2Ships = Player2State.Ships,
                Board1 = GetSerializedBoard(Player1State.Board),
                Board2 = GetSerializedBoard(Player2State.Board)
            };


            var jsonOptions = new JsonSerializerOptions()
            {
                WriteIndented = true,
                PropertyNameCaseInsensitive = true
            };
            return JsonSerializer.Serialize(state, jsonOptions);
        }

        private CellState[][] GetSerializedBoard(CellState[,] board)
        {
            var outputBoard = new CellState[BoardHeight][];
            for (var i = 0; i < BoardHeight; i++)
            {
                outputBoard[i] = new CellState[BoardWidth];
            }

            for (var row = 0; row < BoardHeight; row++)
            {
                for (var col = 0; col < BoardWidth; col++)
                {
                    outputBoard[row][col] = board[col, row];
                }
            }

            return outputBoard;
        }


        public void SetGameStateFromJsonString(string jsonString)
        {
            var state = JsonSerializer.Deserialize<SaveGameDto>(jsonString);

            _moveByPlayer1 = state!.MoveByPlayer;
            BoardHeight = state.Height;
            BoardWidth = state.Width;
            Player1State = new BoardState(GetDeserializedBoard(state.Board1), state.Player1Name!);
            Player2State = new BoardState(GetDeserializedBoard(state.Board2), state.Player2Name!);
            Player1State.Ships = state.Player1Ships!;
            Player2State.Ships = state.Player2Ships!;
        }


        // private List<Ship> GetDeserializedShips(List<Ship> statePlayer1Ships)
        private static CellState[,] GetDeserializedBoard(IReadOnlyList<CellState[]> dtoBoard)
        {
            var width = dtoBoard[0].Length;
            var height = dtoBoard.Count;

            CellState[,] board = new CellState[width, height];

            for (var row = 0; row < height; row++)
            {
                for (var col = 0; col < width; col++)
                {
                    board[col, row] = dtoBoard[row][col];
                }
            }

            return board;
        }

        public Game GetSaveDto()
        {
            var gameDto = new Game
            {
                MoveByPlayer = _moveByPlayer1,
                Board1 = GetStringFromBoard(Player1State.Board),
                Board2 = GetStringFromBoard(Player2State.Board),
                Width = BoardWidth,
                Height = BoardHeight,
                Player1Name = Player1State.PlayerName,
                Player2Name = Player2State.PlayerName,
                // Player1Ships = getShipsDto(_player1State.Ships),
                // Player2Ships = getShipsDto(_player2State.Ships)
            };
            return gameDto;
        }

        private static Domain.Ship GetShipDto1(Ship ship, int dtoGameId)
        {
            var shipDto = new Domain.Ship
            {
                Name = ship.Name,
                Lenght = ship.Lenght,
                Height = ship.Height,
                Ships1GameId = dtoGameId,
                Ships2GameId = null,
                Coordinates = GetCoordinatesDto(ship.Coordinates)
            };
            return shipDto;
        }

        private static Domain.Ship GetShipDto2(Ship ship, int dtoGameId)
        {
            var shipDto = new Domain.Ship
            {
                Name = ship.Name,
                Lenght = ship.Lenght,
                Height = ship.Height,
                Ships1GameId = null,
                Ships2GameId = dtoGameId,
                Coordinates = GetCoordinatesDto(ship.Coordinates)
            };
            return shipDto;
        }

        private static string GetCoordinatesDto(IEnumerable<Coordinate>? shipCoordinates)
        {
            var outPutCoordinates = shipCoordinates!.Aggregate("", (current, coordinate) => current + (coordinate.X + "," + coordinate.Y + ";"));

            outPutCoordinates = outPutCoordinates.Remove(outPutCoordinates.Length - 1);

            return outPutCoordinates;
        }
       

        public IEnumerable<Domain.Ship> GetShipsDto(IEnumerable<Ship> ships1, IEnumerable<Ship> ships2, int dtoGameId)
        {
            var outputShips = ships1.Select(ship => GetShipDto1(ship, dtoGameId)).ToList();
            outputShips.AddRange(ships2.Select(ship => GetShipDto2(ship, dtoGameId)));

            return outputShips;
        }

        private string GetStringFromBoard(CellState[,] board)
        {
            var outputString = "";
            for (var row = 0; row < BoardHeight; row++)
            {
                for (var col = 0; col < BoardWidth; col++)
                {
                    outputString += board[col, row] + ",";
                }
            }

            return outputString;
        }

        private CellState[,] GetBoardFromString(string board, int height, int width)
        {
            var splitText = board.Split(',');
            var res = new CellState[width, height];

            var i = 0;
            for (var row = 0; row < height; row++)
            {
                for (var col = 0; col < width; col++)
                {
                    res[col, row] = splitText[i] switch
                    {
                        "Empty" => CellState.Empty,
                        "Bomb" => CellState.Bomb,
                        "Ship" => CellState.Ship,
                        "Hit" => CellState.Hit,
                        _ => res[col, row]
                    };
                    i++;
                }
            }

            return res;
        }

        public void SetGameStateFromDbObject(Game? game, IEnumerable<Domain.Ship> player1Ships, IEnumerable<Domain.Ship> player2Ships)
        {
            _moveByPlayer1 = game!.MoveByPlayer;
            BoardHeight = game.Height;
            BoardWidth = game.Width;
            Player1State = new BoardState(GetBoardFromString(game.Board1, BoardHeight, BoardWidth), game.Player1Name);
            Player2State = new BoardState(GetBoardFromString(game.Board2, BoardHeight, BoardWidth), game.Player2Name);
            Player1State.Ships = GetShipsFromDbObject(player1Ships);
            Player2State.Ships = GetShipsFromDbObject(player2Ships);
        }

        private static List<Ship> GetShipsFromDbObject(IEnumerable<Domain.Ship> player1Ships)
        {
            var outputShipList = new List<Ship>();
            foreach (var domainShip in player1Ships!)
            {
                var Bllship = new Ship();
                Bllship.Name = domainShip.Name;
                Bllship.Height = domainShip.Height;
                Bllship.Lenght = domainShip.Lenght;
                Bllship.Coordinates = GetCoordinatesFromDbObject(domainShip.Coordinates);
                outputShipList.Add(Bllship);
            }

            return outputShipList;
        }

        private static List<Coordinate> GetCoordinatesFromDbObject(string? domainShipCoordinates)
        {
            var outPutCoordinates = new List<Coordinate>();
            Console.Write(domainShipCoordinates!);

            var coordinateString = domainShipCoordinates!.Split(";");
            Console.Write(coordinateString.ToString());

            foreach (var coordinate in coordinateString)
            {
                var coordinateStr = coordinate.Split(",");
                Console.WriteLine(coordinateStr[0]);
                Console.WriteLine(coordinateStr[1]);

                outPutCoordinates.Add(new Coordinate()
                {
                    X = int.Parse(coordinateStr[0]),
                    Y = int.Parse(coordinateStr[1])
                });
            }

            return outPutCoordinates;
        }

        public void SetGameStateFromDbConfig(Domain.Config config)
        {
            BoardWidth = config.BoardSizeY;
            BoardHeight = config.BoardSizeX;
            Player1State.PlayerName = config.Player1Name;
            Player2State.PlayerName = config.Player2Name;
        }

        public GameConfig GetConfigFromDbObject(Domain.Config conf, List<Domain.ShipConfig> shipConfigs,
            TouchRule? touchRule)
        {
            Console.Write(touchRule!.TouchRuleId);

            Console.Write(touchRule!.Rule);
            var config = new GameConfig
            {
                BoardSizeX = conf.BoardSizeX,
                BoardSizeY = conf.BoardSizeY,
                EShipTouchRule = GetTouchRuleFromDbObject(touchRule),
                Player1Name = conf.Player1Name,
                Player2Name = conf.Player2Name,
                ShipConfigs = GetShipConfigsFromDbObject(shipConfigs!)
            };
            return config;
        }

        private List<ShipConfig> GetShipConfigsFromDbObject(List<Domain.ShipConfig>? confShipConfigs)
        {
            return confShipConfigs!.Select(shipConfig => new ShipConfig {Name = shipConfig.Name,
                Quantity = shipConfig.Quantity,
                ShipSizeX = shipConfig.ShipSizeX,
                ShipSizeY = shipConfig.ShipSizeY})
                .ToList();
        }

        private static EShipTouchRule GetTouchRuleFromDbObject(Domain.TouchRule? confTouchRule)
        {
            return confTouchRule!.Rule switch
            {
                "NoTouch" => EShipTouchRule.NoTouch,
                "CornerTouch" => EShipTouchRule.CornerTouch,
                "SideTouch" => EShipTouchRule.SideTouch,
                _ => EShipTouchRule.NoTouch
            };
        }

        public void ReloadShips()
        {
            _ships = ReadShipsFromConfig(Config.ShipConfigs);
        }

        public void RandomShipPlacement(BoardState board, int? recursionCount)
        {
            Random rnd = new Random();
            Direction[] direction = {Direction.Down, Direction.Up, Direction.Left, Direction.Right};
            if (recursionCount != null && recursionCount > 100)
            {
                throw new InvalidExpressionException();
            }
            do
            {
                var row = rnd.Next(0, BoardHeight);
                var col = rnd.Next(0, BoardWidth);
                var dir = rnd.Next(0, direction.Length);
                try
                {
                    var shipConfig = GetShip();
                    PlaceAShip(GetShip(), col, row, direction[dir], board);
                }
                catch (InvalidOperationException)
                {
                    return;
                }
                catch (Exception)
                {
                    RandomShipPlacement(board, recursionCount += 1);
                }

                try
                {
                    PopShip();
                }
                catch (InvalidOperationException)
                {
                    return;
                }
            } while (true);
        }

        public bool AllShipsSunk(BoardState boardState)
        {
            CellState[,] board = boardState.Board;
            for (var row = 0; row < BoardHeight; row++)
            {
                for (var col = 0; col < BoardWidth; col++)
                {
                    if (board[col, row] == CellState.Ship)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public void GetLastGameState()
        {
            var rollBackObject = _state.Pop();
            Player1State.Board = rollBackObject.Player1Board;
            Player2State.Board = rollBackObject.Player2Board;
            _moveByPlayer1 = rollBackObject.MoveByPlayer1;
        }

        public void SaveCurrentState()
        {
            var rollBackObject = new RollBackObject(BoardCopy(Player1State.Board),
                BoardCopy(Player2State.Board),
                _moveByPlayer1);
            _state.Push(rollBackObject);
        }

        private CellState[,] BoardCopy(CellState[,] board)
        {
            var boardCopy = new CellState[BoardWidth, BoardHeight];
            for (var i = 0; i < BoardHeight; i++)
            {
                for (var j = 0; j < BoardWidth; j++)
                {
                    boardCopy[j, i] = board[j, i];
                }
            }

            return boardCopy;
        }

        public bool NoMoreShips()
        {
            return _ships.Count == 0;
        }

        public static int GetNrOfShipsLeft(BoardState boardState)
        {
            return boardState.Ships.Count - GetNrOfSunkShips(boardState);
        }
    }
}