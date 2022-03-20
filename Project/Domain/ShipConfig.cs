using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public class ShipConfig
    {
        public int ShipConfigId { get; set; }
        
        [MaxLength(20)]
        public string? Name { get; set; }

        public int Quantity { get; set; }

        public int ShipSizeY { get; set; }
        public int ShipSizeX { get; set; }
        
        
        public int ConfigId { get; set; }
        public Config? Config { get; set; } 

    }
}