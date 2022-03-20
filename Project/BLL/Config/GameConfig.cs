using System;
using System.Collections.Generic;
using BLL.ShipObjects;

namespace BLL.Config
{
    public class GameConfig
    {
        public int BoardSizeX { get; set; } = 10;
        public int BoardSizeY { get; set; } = 10;
        
        public String Player1Name { get; set; } = "Player 1";
        public String Player2Name { get; set; } = "Player 2";

        public List<ShipConfig> ShipConfigs { get; set; } = new List<ShipConfig>()
        {
            new ShipConfig()
            {
                Name = "Patrol",
                Quantity = 1,
                ShipSizeY = 1,
                ShipSizeX = 1,
            },
            new ShipConfig()
            {
                Name = "Cruiser",
                Quantity = 1,
                ShipSizeY = 1,
                ShipSizeX = 2,
            },
            new ShipConfig()
            {
                Name = "Submarine",
                Quantity = 1,
                ShipSizeY = 1,
                ShipSizeX = 3,
            },
            new ShipConfig()
            {
                Name = "Battleship",
                Quantity = 1,
                ShipSizeY = 1,
                ShipSizeX = 4
            },
            new ShipConfig()
            {
                Name = "Carrier",
                Quantity = 1,
                ShipSizeY = 1,
                ShipSizeX = 5,
            },
        };

        public EShipTouchRule EShipTouchRule { get; set; } = EShipTouchRule.CornerTouch;


    }
}