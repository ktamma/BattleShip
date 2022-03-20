using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public class Ship
    {
        public int ShipId { get; set; }
        
        [MaxLength(20)]
        public string? Name { get;  set; } 
        public int? Lenght { get;  set; } 
        public int? Height { get;  set; } 
        
        public string? Coordinates  { get;  set; }
        
        public int? Ships1GameId { get; set; }
        public Game? Ships1 { get; set; }
        
        public int? Ships2GameId { get; set; }
        public Game? Ships2 { get; set; }
    }
}