using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.Json;
using DAL;
using Domain;

namespace BLL.Config

{
    public class GameConfigManager
    {
        private readonly string _configPath;
        private readonly GameConfig _conf = new();


        public GameConfigManager(string configPath)
        {
            _configPath = configPath;
        }


        public string InsertSize()
        {
            Console.Clear();

            Console.WriteLine("Enter board width(1-20)");
            var x = Console.ReadLine();

            Console.WriteLine("Enter board lenght(1-20)");
            var y = Console.ReadLine();


            var outPutX = 0;
            var outPutY = 0;
            try
            {
                if (x != null) outPutX = Convert.ToInt32(x);
                if (y != null) outPutY = Convert.ToInt32(y);
                
            }
            catch (FormatException)
            {
                InsertSize();
            }
            if (outPutX >= 1 && outPutX <= 20) _conf.BoardSizeX = outPutX;
            if (outPutY >= 1 && outPutY <= 20) _conf.BoardSizeY = outPutY;

            Console.Clear();

            return "";
        }

        public string InsertPlayerNames()
        {
            Console.Clear();
            Console.WriteLine("Enter player 1 Name(max 20 characters)");
            var player1 = Console.ReadLine();

            Console.WriteLine("Enter player 2 Name(max 20 characters)");
            var player2 = Console.ReadLine();

            if (player1 != null && player1.Length <= 20)
                _conf.Player1Name = player1; //double ampersand ignores rest if first condition fails
            if (player2 != null && player2.Length <= 20) _conf.Player2Name = player2;
            Console.Clear();

            return "";
        }

        public string Save()
        {
            Console.Clear();
            var defaultName = "config_" + DateTime.Now.ToString("yyyy-MM-dd") + ".json";
            Console.WriteLine($"Current config name ({defaultName})");
            Console.WriteLine("Enter new config name or press enter:");
            var fileName = Console.ReadLine();
            fileName = string.IsNullOrWhiteSpace(fileName) ? defaultName : WebUtility.UrlEncode(fileName + ".json");


            var path = _configPath + Path.DirectorySeparatorChar + "Configs" + Path.DirectorySeparatorChar + fileName;
            var jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            var confJsonStr = JsonSerializer.Serialize(_conf, jsonOptions);


            File.WriteAllText(path, confJsonStr);


            Console.Clear();
            return "";
        }

        public string SaveToDb()
        {
            var defaultName = "config_" + DateTime.Now.ToString("yyyy-MM-dd") + ".json";
            Console.WriteLine($"Current save name ({defaultName})");
            Console.WriteLine("Enter new save name or press enter:");
            var fileName = Console.ReadLine();
            fileName = string.IsNullOrWhiteSpace(fileName) ? defaultName : WebUtility.UrlEncode(fileName + ".json");

            using var db = new ApplicationDbContext();
            var dbConf = new Domain.Config
            {
                Name = fileName,
                Player1Name = _conf.Player1Name,
                Player2Name = _conf.Player2Name,
                BoardSizeX = _conf.BoardSizeX,
                BoardSizeY = _conf.BoardSizeY,
                TouchRuleId = FindTouchRuleId(_conf.EShipTouchRule)
            };


            db.Configs.Add(dbConf);
            db.SaveChanges();

            var shipConfigs = ShipsConfigToDbShipsConfig(_conf.ShipConfigs, dbConf.ConfigId);

            foreach (var shipConfig in shipConfigs!)
            {
                db.ShipConfigs.Add(shipConfig);
            }

            db.SaveChanges();


            return "";
        }

        private int FindTouchRuleId(EShipTouchRule confEShipTouchRule)
        {
            using var db = new ApplicationDbContext();
            try
            {
                Console.WriteLine(confEShipTouchRule.ToString());
                return db.TouchRules.FirstOrDefault(t => t.Rule.Equals(confEShipTouchRule.ToString()))!.TouchRuleId;
            }
            catch (Exception)
            {
                return 0;
            }
        }


        private static IEnumerable<ShipConfig> ShipsConfigToDbShipsConfig(
            IEnumerable<ShipObjects.ShipConfig> shipConfigs,
            int dbConfConfigId)
        {
            var outPutShips = new List<ShipConfig>();

            foreach (var shipConfig in shipConfigs)
            {
                outPutShips.Add(new ShipConfig
                {
                    ShipSizeX = shipConfig.ShipSizeX,
                    ShipSizeY = shipConfig.ShipSizeY,
                    Quantity = shipConfig.Quantity,
                    Name = shipConfig.Name,
                    ConfigId = dbConfConfigId
                });
            }

            return outPutShips;
        }

        public string EditPatrol()
        {
            Console.Clear();

            var ships = _conf.ShipConfigs;
            var ship = ships.Single(s => s.Name!.Equals("Patrol"));
            Console.WriteLine($"Enter lenght(number) for ships of type: {ship.Name} (recommended: 1)");
            var lenght = Console.ReadLine();
            Console.WriteLine($"Enter width(number) for ships of type: {ship.Name} (recommended: 1)");
            var width = Console.ReadLine();
            Console.WriteLine($"Enter amount(number) of ships of type: {ship.Name} (recommended: 1)");
            var quantity = Console.ReadLine();
            try
            {
                if (lenght != null) ship.ShipSizeX = Convert.ToInt32(lenght);

                if (width != null) ship.ShipSizeY = Convert.ToInt32(width);
                
                if (quantity != null) ship.Quantity = Convert.ToInt32(quantity);

            }
            catch (FormatException)
            {
                EditPatrol();
            }

            Console.WriteLine(ship.Name);
            Console.WriteLine(ship.ShipSizeX);
            Console.WriteLine(ship.ShipSizeY);
            Console.WriteLine(ship.Quantity);
            Console.ReadLine();

            return "";
        }

        public string EditCruiser()
        {
            Console.Clear();

            var ships = _conf.ShipConfigs;
            var ship = ships.Single(s => s.Name!.Equals("Cruiser"));
            Console.WriteLine($"Enter lenght(number) for ships of type: {ship.Name} (recommended: 2)");
            var lenght = Console.ReadLine();
            Console.WriteLine($"Enter width(number) for ships of type: {ship.Name} (recommended: 1)");
            var width = Console.ReadLine();
            Console.WriteLine($"Enter amount(number) of ships of type: {ship.Name} (recommended: 1)");
            var quantity = Console.ReadLine();
            

            try
            {
                if (lenght != null) ship.ShipSizeX = Convert.ToInt32(lenght);

                if (width != null) ship.ShipSizeY = Convert.ToInt32(width);
                
                if (quantity != null) ship.Quantity = Convert.ToInt32(quantity);

            }
            catch (FormatException)
            {
                EditCruiser();
            }

            return "";
        }

        public string EditSubmarine()
        {
            Console.Clear();

            var ships = _conf.ShipConfigs;
            var ship = ships.Single(s => s.Name!.Equals("Submarine"));
            Console.WriteLine($"Enter lenght(number) for ships of type: {ship.Name} (recommended: 3)");
            var lenght = Console.ReadLine();
            Console.WriteLine($"Enter width(number) for ships of type: {ship.Name} (recommended: 1)");
            var width = Console.ReadLine();
            Console.WriteLine($"Enter amount(number) of ships of type: {ship.Name} (recommended: 1)");
            var quantity = Console.ReadLine();

            try
            {
                if (lenght != null) ship.ShipSizeX = Convert.ToInt32(lenght);

                if (width != null) ship.ShipSizeY = Convert.ToInt32(width);
                
                if (quantity != null) ship.Quantity = Convert.ToInt32(quantity);

            }
            catch (FormatException)
            {
                EditSubmarine();
            }

            return "";
        }

        public string EditBattleship()
        {
            Console.Clear();

            var ships = _conf.ShipConfigs;
            var ship = ships.Single(s => s.Name!.Equals("Battleship"));
            Console.WriteLine($"Enter lenght(number) for ships of type: {ship.Name} (recommended: 4)");
            var lenght = Console.ReadLine();
            Console.WriteLine($"Enter width(number) for ships of type: {ship.Name} (recommended: 1)");
            var width = Console.ReadLine();
            Console.WriteLine($"Enter amount(number) of ships of type: {ship.Name} (recommended: 1)");
            var quantity = Console.ReadLine();

            try
            {
                if (lenght != null) ship.ShipSizeX = Convert.ToInt32(lenght);

                if (width != null) ship.ShipSizeY = Convert.ToInt32(width);
                
                if (quantity != null) ship.Quantity = Convert.ToInt32(quantity);

            }
            catch (FormatException)
            {
                EditBattleship();
            }

            return "";
        }

        public string EditCarrier()
        {
            Console.Clear();

            var ships = _conf.ShipConfigs;
            var ship = ships.Single(s => s.Name!.Equals("Carrier"));
            Console.WriteLine($"Enter lenght(number) for ships of type: {ship.Name} (recommended: 5)");
            var lenght = Console.ReadLine();
            Console.WriteLine($"Enter width(number) for ships of type: {ship.Name} (recommended: 1)");
            var width = Console.ReadLine();
            Console.WriteLine($"Enter amount(number) of ships of type: {ship.Name} (recommended: 1)");
            var quantity = Console.ReadLine();

            try
            {
                if (lenght != null) ship.ShipSizeX = Convert.ToInt32(lenght);

                if (width != null) ship.ShipSizeY = Convert.ToInt32(width);
                
                if (quantity != null) ship.Quantity = Convert.ToInt32(quantity);

            }
            catch (FormatException)
            {
                EditCarrier();
            }

            return "";
        }

        public string SelectShipTouchRule()
        {
            Console.Clear();
            Console.WriteLine("Select ship touch rule");
            Console.WriteLine("Corners can touch - C");
            Console.WriteLine("Sides can touch - S");
            Console.WriteLine("No touching allowed - N");

            var touchRule = Console.ReadLine();
            switch (touchRule?.ToLower())
            {
                case "c":
                    _conf.EShipTouchRule = EShipTouchRule.CornerTouch;
                    break;
                case "s":
                    _conf.EShipTouchRule = EShipTouchRule.SideTouch;
                    break;
                case "n":
                    _conf.EShipTouchRule = EShipTouchRule.NoTouch;
                    break;
                default:
                    SelectShipTouchRule();
                    return "";
            }

            return "";
        }
    }
}