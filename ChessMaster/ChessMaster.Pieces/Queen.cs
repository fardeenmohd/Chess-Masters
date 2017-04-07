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

        public override List<PiecePossibleMove> GetPossibleMoves(List<BasePiece> board)
        {
            return GetDiagonalMoves(board, BOARDLENGTH).Union(GetVerticalMoves(board, BOARDLENGTH)).Union(GetHorizontalMoves(board, BOARDLENGTH)).ToList();
        }

        public override BasePiece CopyPiece()
        {
            return new Queen((int)Position.X, (int)Position.Y, IsWhite)
            {
                IsFirstMove = IsFirstMove
            };
        }
    }
}
