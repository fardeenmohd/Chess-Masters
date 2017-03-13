using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ChessMaster.Pieces
{
    public class King : BasePiece
    {
        public King(int x, int y, bool isWhite = true) : base(x, y, isWhite)
        {
            if (IsWhite)
            {
                PieceImage = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../Images/white_king.png");
            }
            else
            {
                PieceImage = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../Images/black_king.png");
            }
        }
        public override List<Point> GetPossibleMoves(List<BasePiece> board)
        {
            //by default move length is 1
            return GetDiagonalMoves(board).Union(GetHorizontalMoves(board)).Union(GetVerticalMoves(board)).ToList();
        }
    }
}
