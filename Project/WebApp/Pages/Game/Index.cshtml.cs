using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using BLL;
using BLL.BoardObjects;
using BLL.Config;
using BLL.ShipObjects;
using DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace WebApp.Pages.Game
{
    public class Index : PageModel
    {
        public static GameConfig GameConfig { get; set; } = new GameConfig();
        public static global::BLL.GameBrain GameBrain { get; set; } = new global::BLL.GameBrain(GameConfig);
        public static bool FirstGet = true;
        public static ArrayList Errors { get; set; } = new ArrayList();
        public static ArrayList MoveResult { get; set; } = new ArrayList();

        public static Direction Direction = Direction.Down;

        public  static char[] Letters { get; set; } = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();


        public IActionResult Reload()
        {
            Console.WriteLine("Im reloading");
            return Redirect("./Index");
        }

        public IActionResult RedirectTo(string url)
        {
            Console.WriteLine("Redirecting");

            return Redirect("../ShipPlacement");
        }

        public IActionResult BackToMain()
        {
            return Redirect("../../Index");
        }

        public async void OnGet(int x, int y, string? action, int? id, Direction? direction)
        {
            Errors.Clear();
            switch (action)
            {
                case "load":
                {
                    if (id != null)
                    {
                        using var db = new ApplicationDbContext();
                        var player1Ships = db.Ships.Where(m => m.Ships1GameId == id).ToList();
                        var player2Ships = db.Ships.Where(m => m.Ships2GameId == id).ToList();
                        var game = db.Games.FirstOrDefault(m => m.GameId == id);
                        if (game != null)
                        {
                            GameBrain.SetGameStateFromDbObject(game, player1Ships, player2Ships);
                        }
                    }

                    Reload();
                    break;
                }
                case "loadFromJson":
                {
                    string workingDirectory = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, @"..\"));
                    String path = workingDirectory + Path.DirectorySeparatorChar + "ConsoleApp" +
                                  Path.DirectorySeparatorChar + "SaveGames";
                    var files = Directory.GetFiles(path).ToList();

                    var fileName = files[id!.Value];


                    var jsonString = System.IO.File.ReadAllText(fileName);

                    GameBrain.SetGameStateFromJsonString(jsonString);

                    Reload();
                    break;
                }
                case "randomShips":
                    GameBrain = new GameBrain(GameConfig);
                    try
                    {
                        GameBrain.RandomShipPlacement(GameBrain.Player1State, 0);
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("unable to place ships");
                        Errors.Add("Unable to place all ships!");
                    }

                    try
                    {
                        GameBrain.ReloadShips();
                        GameBrain.RandomShipPlacement(GameBrain.Player2State, 0);
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("unable to place ships");
                        Errors.Add("Unable to place all ships!");
                    }

                    break;

                case "setConfigFromJSON":
                {
                    string workingDirectory = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, @"..\"));
                    String path = workingDirectory + Path.DirectorySeparatorChar + "ConsoleApp" +
                                  Path.DirectorySeparatorChar + "Configs";
                    var files = Directory.GetFiles(path).ToList();


                    var fileName = files[id!.Value];

                    var confText = System.IO.File.ReadAllText(fileName);
                    GameConfig = JsonSerializer.Deserialize<GameConfig>(confText) ?? new GameConfig();
                    GameBrain = new BLL.GameBrain(GameConfig);


                    Reload();

                    BackToMain();
                    break;
                }
                case "newGame":
                {
                    GameBrain = new GameBrain(GameConfig);
                    break;
                }
                case "setConfig":
                {
                    if (id != null)
                    {
                        await using var db = new ApplicationDbContext();
                        var conf = db.Configs.First(m => m.ConfigId == id);
                        var touchRule = db.TouchRules.First(m => m.TouchRuleId == conf.TouchRuleId);
                        var shipConfigs = db.ShipConfigs.Where(m => m.ConfigId == conf.ConfigId).ToList();


                        var config = GameBrain.GetConfigFromDbObject(conf, shipConfigs, touchRule);
                        GameConfig = config;


                        GameBrain = new BLL.GameBrain(config);
                    }


                    Reload();

                    RedirectTo("/ShipPlacement");
                    break;
                }
                case "firstLoad":
                    GameBrain.SwapPlayers();
                    break;
                case "move":
                {
                    MoveResult.Clear();
                    GameBrain.SaveCurrentState();
                    GameBrain.MakeAMove(x, y);
                    var board = GameBrain.GetBoardStates().attackingPlayerState.Board;
                    if (board[x, y] == CellState.Hit) //hit
                    {
                        MoveResult.Add("HIT!");
                        var hitShip =
                            GameBrain.GetShipFromPosition(y, x, GameBrain.GetBoardStates().attackingPlayerState);
                        MoveResult.Add($"Hit ship stats:");
                        MoveResult.Add($"Ship name: {hitShip.Name}");
                        MoveResult.Add($"Ship lenght: {hitShip.Lenght}");
                        MoveResult.Add($"Ship height: {hitShip.Height}");
                        MoveResult.Add($"Ship damage count: {hitShip.GetShipDamageCount(board)}");
                        MoveResult.Add($"Ship sunk: {hitShip.IsShipSunk(board)}");
                        MoveResult.Add($"Number of ships sunk: {GameBrain.GetNrOfSunkShips(GameBrain.GetBoardStates().attackingPlayerState)}");
                        MoveResult.Add(
                            $"Number of ships left: {GameBrain.GetNrOfShipsLeft(GameBrain.GetBoardStates().attackingPlayerState)}");

                    }
                    else
                    {
                        MoveResult.Add("MISS!");
                    }

                    break;
                }
                case "chooseDirection":
                    Direction = (Direction) direction!;
                    break;
                case "back":
                    try
                    {
                        GameBrain.GetLastGameState();
                    }
                    catch (Exception)
                    {
                        Errors.Add("Nothing more to roll back!");
                    }

                    break;
                case "placeShip":
                {
                    try
                    {
                        Errors.Clear();
                        var shipConfig = GameBrain.GetShip();
                        GameBrain.PlaceAShip(shipConfig, x, y, Direction,
                            GameBrain.GetBoardStates().attackingPlayerState);
                        GameBrain.PopShip();
                    }

                    catch (Exception) // player is an idiot
                    {
                        Errors.Add("Cannot place ship at position!");
                    }

                    if (GameBrain.NoMoreShips() && GameBrain.Player2State.PlayerName !=
                        GameBrain.GetBoardStates().attackingPlayerState.PlayerName)
                    {
                        Console.WriteLine("!!!");
                        GameBrain.ReloadShips();
                        foreach (var shipConfig in GameBrain.Config.ShipConfigs)
                        {
                            Console.WriteLine(shipConfig.Quantity);
                        }

                        GameBrain.SwapPlayers();
                        return;
                    }

                    break;
                }
            }

            // BLL.GetBoardStates().currentBoardState.Board[x, y] = CellState.Bomb;
        }

        public void OnPost()
        {
            var saveName = Request.Form["saveName"];


            if (Request.Form["DBsave"].Equals("save")) // DB save
            {
                using var db = new ApplicationDbContext();
                var dto = GameBrain.GetSaveDto();
                dto.GameName = saveName;
                db.Games.Add(dto);
                db.SaveChanges();


                var shipsDto = GameBrain.GetShipsDto(GameBrain.Player1State.Ships, GameBrain.Player2State.Ships,
                    dto.GameId);

                foreach (var ship in shipsDto)
                {
                    db.Ships.Add(ship);
                }


                Console.WriteLine(dto.GameId);
                db.SaveChanges();
            }

            if (Request.Form["JSONsave"].Equals("save")) //JSON save
            {
                var serializedGame = GameBrain.GetSerializedGameState();
                string workingDirectory = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, @"..\"));
                string path = workingDirectory + Path.DirectorySeparatorChar + "ConsoleApp" +
                              Path.DirectorySeparatorChar + "SaveGames" + Path.DirectorySeparatorChar +
                              WebUtility.UrlEncode(saveName) + ".json";
                Console.WriteLine(path);

                System.IO.File.WriteAllText(path, serializedGame);
            }
        }

        public static string CellString(CellState cellState)
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
    }
}