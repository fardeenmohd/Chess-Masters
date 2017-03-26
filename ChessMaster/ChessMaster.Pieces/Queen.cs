using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ChessMaster.Pieces
{
    public class Queen : BasePiece
    {
        public Queen(int x, int y, bool isWhite = true) : base(x, y, isWhite)
        {
            if (IsWhite)
            {
                PieceImage = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../Images/white_queen.png");
            }
            else
            {
                PieceImage = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../Images/black_queen.png");
            }
        }

        public override List<Point> GetPossibleMoves(List<BasePiece> board)
        {
            return GetDiagonalMoves(AddMove, board, BOARDLENGTH).Union(GetVerticalMoves(AddMove, board, BOARDLENGTH)).Union(GetHorizontalMoves(AddMove, board, BOARDLENGTH)).ToList();
        }

        public override List<Point> GetCellsUnderAttack(List<BasePiece> board)
        {
            return GetDiagonalMoves(AddCellUnderAttack, board, BOARDLENGTH).Union(GetVerticalMoves(AddCellUnderAttack, board, BOARDLENGTH)).Union(GetHorizontalMoves(AddCellUnderAttack, board, BOARDLENGTH)).ToList();
        }
    }
}
