namespace BLL.BoardObjects
{
    public class RollBackObject
    {
        public RollBackObject(CellState[,] player1Board, CellState[,] player2Board, bool moveByPlayer1)
        {
            Player1Board = player1Board;
            Player2Board = player2Board;
            MoveByPlayer1 = moveByPlayer1;
        }



        public CellState[,] Player1Board { get; set; }
        
        public CellState[,] Player2Board { get; set; }
        
        public  bool MoveByPlayer1 { get; set; }

    }
}