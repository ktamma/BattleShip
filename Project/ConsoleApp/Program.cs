using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.Json;
using BLL;
using BLL.Config;
using DAL;
using GameConsoleUI;
using MenuSystem;

namespace ConsoleApp
{
    internal static class Program
    {
        private static string? _basePath;
        private static GameConfig _gameConfig = new GameConfig();

        private static void Main(string[] args)
        {
            _basePath = args.Length == 1 ? args[0] : Environment.CurrentDirectory;
            if (_basePath.Contains("bin"))
            {
                _basePath = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, @"..\..\..\"));
            }
            // _basePath = args.Length == 1 ? args[0] : Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, @"..\..\..\"));
            
            GameConfigManager configManager = new GameConfigManager(_basePath);


            var presetCommands = new MenuCommands();


            var menuA = new Menu(Menu.MenuLevel.Level0, presetCommands, "Main Menu");

            var gameConfigMenu = new Menu(Menu.MenuLevel.Level1, presetCommands, "Settings");

            var shipConfigMenu = new Menu(Menu.MenuLevel.Level1Plus, presetCommands, "Ship menu");

            menuA.AddMenuItem(new MenuItem("New Game", Battleship));
            menuA.AddMenuItem(new MenuItem("New Game with randomized ships", RandomBattleship));
            menuA.AddMenuItem(new MenuItem("Game settings", gameConfigMenu.RunMenu));
            menuA.AddMenuItem(new MenuItem("Choose current configuration", SetConfiguration));
            menuA.AddMenuItem(new MenuItem("Choose current configuration db", SetConfigurationFromDb));

            menuA.AddMenuItem(new MenuItem("Load game from JSON", LoadGameAction));
            menuA.AddMenuItem(new MenuItem("Load game from DB", LoadGameFromDb));

            menuA.AddMenuItem(new MenuItem("Exit Menu", presetCommands.Exit));


            gameConfigMenu.AddMenuItem(new MenuItem("Edit board size", configManager.InsertSize));
            gameConfigMenu.AddMenuItem(new MenuItem("Edit player names", configManager.InsertPlayerNames));
            gameConfigMenu.AddMenuItem(new MenuItem("Edit ship touch rule", configManager.SelectShipTouchRule));
            gameConfigMenu.AddMenuItem(new MenuItem("Ship settings", shipConfigMenu.RunMenu));
            gameConfigMenu.AddMenuItem(new MenuItem("Save", configManager.Save));
            gameConfigMenu.AddMenuItem(new MenuItem("Save to db", configManager.SaveToDb));

            gameConfigMenu.AddMenuItem(new MenuItem("Return", presetCommands.MainMenu));


            shipConfigMenu.AddMenuItem(new MenuItem("Edit Patrol", configManager.EditPatrol));
            shipConfigMenu.AddMenuItem(new MenuItem("Edit Cruiser", configManager.EditCruiser));
            shipConfigMenu.AddMenuItem(new MenuItem("Edit Submarine", configManager.EditSubmarine));
            shipConfigMenu.AddMenuItem(new MenuItem("Edit Battleship", configManager.EditBattleship));
            shipConfigMenu.AddMenuItem(new MenuItem("Edit Carrier", configManager.EditCarrier));
            shipConfigMenu.AddMenuItem(new MenuItem("Return", presetCommands.Return));


            menuA.RunMenu();
        }


        private static string SetConfigurationFromDb()
        {
            var presetCommands = new MenuCommands();
            var menuA = new Menu(Menu.MenuLevel.Level0, presetCommands, "Choose configuration");
            Console.Clear();
            using var db = new ApplicationDbContext();
            var configs = db.Configs.ToList();
            for (int i = 0; i < configs.Count; i++)
            {
                var arr = configs[i].Name.Split($"{Path.DirectorySeparatorChar}");
                var len = arr.Length;
                var i1 = i;
                menuA.AddMenuItem(
                    new MenuItem($"{i}) {WebUtility.UrlDecode(arr[len - 1])}", () => LoadConfigFromDb(i1)));
            }

            menuA.AddMenuItem(new MenuItem("Return", presetCommands.Exit));


            menuA.RunMenu();
            return "";
        }

        private static string LoadConfigFromDb(int i)
        {
            Console.Clear();
            using var db = new ApplicationDbContext();
            var configs = db.Configs.ToList();
            var conf = configs[i];
            var shipConfigs = db.ShipConfigs.ToList().FindAll(c => c.ConfigId == conf.ConfigId);
            var touchRule = db.TouchRules.FirstOrDefault(t => t.TouchRuleId == conf.TouchRuleId);
            _gameConfig = new GameBrain(_gameConfig).GetConfigFromDbObject(conf, shipConfigs, touchRule);
            return new MenuCommands().Exit();
        }

        private static string SetConfiguration()
        {
            var presetCommands = new MenuCommands();
            var menuA = new Menu(Menu.MenuLevel.Level0, presetCommands, "Choose configuration");
            Console.Clear();
            var configPath = _basePath + Path.DirectorySeparatorChar + "Configs";

            var files = Directory.GetFiles(configPath).ToList();
            for (int i = 0; i < files.Count; i++)
            {
                var arr = files[i].Split($"{Path.DirectorySeparatorChar}");
                var len = arr.Length;
                var i1 = i;
                menuA.AddMenuItem(new MenuItem($"{i}) {WebUtility.UrlDecode(arr[len - 1])}",
                    () => LoadConfigFromJson(i1)));
            }

            menuA.AddMenuItem(new MenuItem("Return", presetCommands.Exit));

            menuA.RunMenu();
            return "";
        }

        private static string LoadConfigFromJson(int i)
        {
            Console.Clear();
            var configPath = _basePath + Path.DirectorySeparatorChar + "Configs";

            var files = Directory.GetFiles(configPath).ToList();
            var fileName = files[i];

            var confText = File.ReadAllText(fileName);

            _gameConfig = JsonSerializer.Deserialize<GameConfig>(confText) ?? new GameConfig();

            return new MenuCommands().Exit();
        }


        static String Battleship()
        {
            var game = new GameBrain(_gameConfig);
            try
            {
                GameUi.PlaceShips(game, game.Player1State);
                game.ReloadShips();
                GameUi.PlaceShips(game, game.Player2State);
            }
            catch (InvalidConstraintException)
            {
                return "";
            }
            
            GameUi.DrawBoards(game.GetBoardStates().defendingPlayerState, game.GetBoardStates().attackingPlayerState);
            GameUi.Play(game);

            return "";
        }

        private static string RandomBattleship()
        {
            var game = new GameBrain(_gameConfig);
            try
            {
                game.RandomShipPlacement(game.Player1State, 0);
                game.ReloadShips();
                game.RandomShipPlacement(game.Player2State, 0);
            }
            catch (Exception)
            {
                Console.WriteLine("Unable to place ships, please change to a bigger board or reduce ships sizes/nr of ships!");
                Console.ReadLine();
                return "";
            }

            GameUi.DrawBoards(game.GetBoardStates().defendingPlayerState, game.GetBoardStates().attackingPlayerState);
            GameUi.Play(game);
            return "";
        }

        private static string LoadGameFromDb()
        {
            var presetCommands = new MenuCommands();
            var menuA = new Menu(Menu.MenuLevel.Level0, presetCommands, "Saved games");
            Console.Clear();
            using var db = new ApplicationDbContext();
            var games = db.Games.ToList();
            for (var i = 0; i < games.Count; i++)
            {
                var arr = games[i].GameName.Split($"{Path.DirectorySeparatorChar}");
                var len = arr.Length;
                var i1 = i;
                menuA.AddMenuItem(new MenuItem($"{i}) {WebUtility.UrlDecode(arr[len - 1])}",
                    () => LoadGameFromDbMenu(i1)));
            }

            menuA.AddMenuItem(new MenuItem("Return", presetCommands.Exit));

            menuA.RunMenu();
            return "";
        }

        private static string LoadGameFromDbMenu(int i)
        {
            var battleShip = new GameBrain(_gameConfig);
            using var db = new ApplicationDbContext();
            var games = db.Games.ToList();
            var ships = db.Ships.ToList();
            var game = games[i];
            var player1Ships = ships.FindAll(s => s.Ships1GameId == game.GameId);
            var player2Ships = ships.FindAll(s => s.Ships2GameId == game.GameId);
            if (game != null)
            {
                battleShip.SetGameStateFromDbObject(game, player1Ships, player2Ships);
            }


            Console.Clear();
            GameUi.DrawBoards(battleShip.GetBoardStates().defendingPlayerState,
                battleShip.GetBoardStates().attackingPlayerState);
            GameUi.Play(battleShip);
            return new MenuCommands().Exit();
        }

        static string LoadGameAction()
        {
            var presetCommands = new MenuCommands();
            var menuA = new Menu(Menu.MenuLevel.Level0, presetCommands, "Saved games");
            Console.Clear();


            var saveGamesPath = _basePath + Path.DirectorySeparatorChar + "SaveGames";


            var files = Directory.GetFiles(saveGamesPath).ToList();
            for (int i = 0; i < files.Count; i++)
            {
                var arr = files[i].Split($"{Path.DirectorySeparatorChar}");
                var len = arr.Length;
                var i1 = i;
                menuA.AddMenuItem(new MenuItem($"{i}) {WebUtility.UrlDecode(arr[len - 1])}",
                    () => LoadGameFromJsonMenu(i1)));
            }

            menuA.AddMenuItem(new MenuItem("Return", presetCommands.Exit));


            menuA.RunMenu();
            return "";
        }

        private static string LoadGameFromJsonMenu(int i)
        {
            var game = new GameBrain(_gameConfig);
            var saveGamesPath = _basePath + Path.DirectorySeparatorChar + "SaveGames";
            var files = Directory.GetFiles(saveGamesPath).ToList();

            var fileName = files[i];

            var jsonString = File.ReadAllText(fileName);

            game.SetGameStateFromJsonString(jsonString);

            Console.Clear();
            GameUi.DrawBoards(game.GetBoardStates().defendingPlayerState, game.GetBoardStates().attackingPlayerState);
            GameUi.Play(game);
            return new MenuCommands().Exit();
        }
    }
}