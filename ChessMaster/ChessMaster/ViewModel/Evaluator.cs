using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ChessMaster.Pieces;

namespace ChessMaster.ViewModel
{

    /// <summary>
    /// The minmax implementation
    /// </summary>
    public class Evaluator
    {
        public const int MAX_INT = int.MaxValue;
        public const int MIN_INT = int.MinValue;
        public virtual double Max(ChessBoard board, bool isWhite, int depth = 4)
        {
            if (depth == 0) return EvaluatePosition(board, isWhite);
            double max = MIN_INT;
            foreach(Point move in board.GetEveryLegalMove(isWhite))
            {
                board.MakeFakeMove((int)move.Y * 8 + (int)move.X);
                double score = Min(board, !isWhite, depth - 1);
                board.UnMakeLastMove();
                if (score > max)
                    max = score;
            }
            return max;
        }
        public virtual double Min(ChessBoard board, bool isWhite, int depth = 4)
        {
            if (depth == 0) return -EvaluatePosition(board, isWhite);
            double min = MAX_INT;
            foreach(Point move in board.GetEveryLegalMove(isWhite))
            {
                board.MakeFakeMove((int)move.Y * 8 + (int)move.X);
                double score = Max(board, !isWhite, depth - 1);
                board.UnMakeLastMove();
                if (score < min)
                    min = score;
            }
            return min;
        }
        public virtual double EvaluatePosition(ChessBoard board, bool isWhite)
        {
            double eval = 0;
            foreach(BasePiece bp in board.ToBasePieceList())
            {
                if(bp != null && bp.IsWhite == isWhite)
                {
                    if (bp is Pawn)
                        eval += 1;
                    if (bp is Knight)
                        eval += 3;
                    if (bp is Bishop)
                        eval += 3.3;
                    if (bp is Rook)
                        eval += 5;
                    if (bp is Queen)
                        eval += 9;
                }               
            }
            return eval;
        }
    }
}
