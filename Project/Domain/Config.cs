using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public class Config
    {
        public int ConfigId { get; set; }

        public int BoardSizeX { get; set; } = 10;
        public int BoardSizeY { get; set; } = 10;
        
        [MaxLength(20)]
        public string Player1Name { get; set; } = "Player 1";
        [MaxLength(20)]
        public string Player2Name { get; set; } = "Player 2";

        public List<ShipConfig>? ShipConfigs { get; set; } = default!;
        
        public string Name { get; set; } = null!;
        
        public int TouchRuleId { get; set; }
        public TouchRule? TouchRule { get; set; }


    }
}