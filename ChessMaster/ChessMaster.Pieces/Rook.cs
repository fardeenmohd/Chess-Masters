using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ChessMaster.Pieces
{
    public class Rook : BasePiece
    {
        public Rook(int x, int y, bool isWhite = true) : base(x, y, isWhite)
        {
            if (IsWhite)
            {
                PieceImage = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../Images/white_rook.png");
            }
            else
            {
                PieceImage = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../Images/black_rook.png");
            }
        }
        public override List<Point> GetPossibleMoves()
        {
            return base.GetVerticalMoves(7).Union(GetHorizontalMoves(7)).ToList();
        }
    }
}
