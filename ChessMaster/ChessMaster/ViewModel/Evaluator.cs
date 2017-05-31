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
        public const int MAX_DEPTH = 3;
        public const int MAX_INT = int.MaxValue;
        public const int MIN_INT = int.MinValue;
        public List<Move> BestMoves = new List<Move>();
        public virtual double Max(ChessBoard board, bool isWhite, int depth = MAX_DEPTH)
        {
            if (depth == 0) return EvaluatePosition(board, isWhite);
            double max = MIN_INT;
            List<PiecePossibleMove> moves = board.GetEveryLegalMove(isWhite);
            foreach (PiecePossibleMove move in moves)
            {              
                board.MakeFakeMove(move, board.Board[(int)move.FromPosition.Y * 8 + (int)move.FromPosition.X].Piece.CopyPiece());
                double score = Min(board, !isWhite, depth - 1);
                if (score > max)
                {
                    max = score;
                    //BestMoves.Add(new Move(move.CopyPiecePossibleMove(), originalPiece));
                    BestMoves.Add(board.LastMadeMove);
                }
                board.UnMakeLastMove();                                
            }
            return max;
        }
        public virtual double Min(ChessBoard board, bool isWhite, int depth = MAX_DEPTH)
        {
            if (depth == 0) return EvaluatePosition(board, isWhite); 
            double min = MAX_INT;
            List<PiecePossibleMove> moves = board.GetEveryLegalMove(isWhite);
            foreach (PiecePossibleMove move in moves)
            {
                board.MakeFakeMove(move, board.Board[(int)move.FromPosition.Y * 8 + (int)move.FromPosition.X].Piece.CopyPiece());
                double score = Max(board, !isWhite, depth - 1);
                board.UnMakeLastMove();
                if (score < min)
                {
                    min = score;
                }
                    
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
