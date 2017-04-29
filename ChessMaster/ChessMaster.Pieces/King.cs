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

        public override List<PiecePossibleMove> GetPossibleMoves(List<BasePiece> board)
        {
            //by default move length is 1
            return GetDiagonalMoves(board).Union(GetHorizontalMoves(board)).Union(GetVerticalMoves(board)).Union(GetCastlingMoves(board)).ToList();
        }

        public override BasePiece CopyPiece()
        {
            return new King((int)Position.X, (int)Position.Y, IsWhite)
            {
                IsFirstMove = IsFirstMove
            };
        }

        protected List<PiecePossibleMove> GetCastlingMoves(List<BasePiece> board)
        {
            int y = IsWhite ? 7 : 0;
            List<PiecePossibleMove> moves = new List<PiecePossibleMove>();
            int leftRookIndex = 8 * y;
            int rightRookIndex = 8 * y + 7;
            if (IsFirstMove && board[leftRookIndex] != null && board[leftRookIndex] is Rook && board[leftRookIndex].IsFirstMove)
            {
                bool isMovePossible = true;
                int x = (int)Position.X;
                while (--x > 0)
                {
                    if (board[8 * y + x] != null)
                    {
                        isMovePossible = false;
                        break;
                    }
                }
                if (isMovePossible)
                    moves.Add(new PiecePossibleMove(new Point(2, y), new Point(Position.X, Position.Y), true, board[leftRookIndex], new Point(3, y)));
            }
            if (IsFirstMove && board[rightRookIndex] != null && board[rightRookIndex] is Rook && board[rightRookIndex].IsFirstMove)
            {
                bool isMovePossible = true;
                int x = (int)Position.X;
                while (++x < 7)
                {
                    if (board[8 * y + x] != null)
                    {
                        isMovePossible = false;
                        break;
                    }
                }
                if (isMovePossible)
                    moves.Add(new PiecePossibleMove(new Point(6, y), new Point(Position.X, Position.Y), true, board[rightRookIndex], new Point(5, y)));
            }
            return moves;
        }
    }
}
