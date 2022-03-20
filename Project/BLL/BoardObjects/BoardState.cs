using System.Collections.Generic;
using BLL.ShipObjects;

namespace BLL.BoardObjects
{
    public class BoardState
    {
        public BoardState(CellState[,] board, string playerName)
        {
            Board = board;
            PlayerName = playerName;
        }

        public CellState[,] Board { get; set; }
        public string PlayerName { get; set; }
        
        public  List<Ship> Ships = new List<Ship>();
    }
}