using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ChessMaster.Pieces
{
    public class Bishop : BasePiece
    {
        public Bishop(int x, int y, bool isWhite = true) : base(x, y, isWhite)
        {
            if (IsWhite)
            {
                PieceImage = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../Images/white_bishop.png");
            }
            else
            {
                PieceImage = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../Images/black_bishop.png");
            }

        }

        public override List<PiecePossibleMove> GetPossibleMoves(List<BasePiece> board)
        {
            return base.GetDiagonalMoves(board, BOARDLENGTH);
        }

        public override BasePiece CopyPiece()
        {
            return new Bishop((int)Position.X, (int)Position.Y, IsWhite)
            {
                IsFirstMove = IsFirstMove
            };
        }
    }
}
