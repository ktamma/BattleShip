using System.Collections.Generic;
using BLL.BoardObjects;
using BLL.ShipObjects;

namespace BLL
{
    public class SaveGameDto
    {
        public bool MoveByPlayer { get; set; }
        public CellState[][] Board1 { get; set; } = null!;
        public CellState[][] Board2 { get; set; } = null!;

        public int Width { get; set; }
        public int Height { get; set; }
        public string? Player1Name { get; set; }
        public string? Player2Name { get; set; }

        public List<Ship>? Player1Ships { get; set; }
        public List<Ship>? Player2Ships { get; set; }

    }
}