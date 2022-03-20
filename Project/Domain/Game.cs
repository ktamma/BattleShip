using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain
{
    public class Game
    {
        public int GameId { get; set; }
        public bool MoveByPlayer { get; set; }
        public string Board1 { get; set; } = default!;
        public string Board2 { get; set; } = default!;
        public int Width { get; set; }
        public int Height { get; set; }
        public string Player1Name { get; set; } = default!;
        public string Player2Name { get; set; } = default!;
        
        public string GameName { get; set; } = default!;

        [InverseProperty("Ships1")]
        public ICollection<Ship>? Player1Ships { get; set; } = default!;
        
        [InverseProperty("Ships2")]
        public ICollection<Ship>? Player2Ships { get; set; } = default!;


    }
}