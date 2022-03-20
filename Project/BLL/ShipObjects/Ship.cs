using System.Collections.Generic;
using System.Linq;
using BLL.BoardObjects;

namespace BLL.ShipObjects
{
    public class Ship
    {
        public string? Name { get;  set; } 
        public int? Lenght { get;  set; } 
        public int? Height { get;  set; } 
        public List<Coordinate>? Coordinates  { get;  set; }

        public Ship()
        {
            
        }
        public Ship(string name, int length, int height)
        {
            Name = name;
            Lenght = length;
            Height = height;
            Coordinates = new List<Coordinate>();

        }
        

        public int GetShipSize() => Coordinates!.Count;
        
        public int GetShipDamageCount(CellState[,] board) =>
            Coordinates!.Count(coordinate => board[coordinate.Y, coordinate.X] == CellState.Hit );

        public bool IsShipSunk(CellState[,] board) =>
            Coordinates!.All(coordinate => board[coordinate.Y, coordinate.X] == CellState.Hit);

        public bool IsAtCoordinates(int row, int col)
        {
            return Coordinates!.Any(coordinate => coordinate.X == row && coordinate.Y == col);
        }
    }
}