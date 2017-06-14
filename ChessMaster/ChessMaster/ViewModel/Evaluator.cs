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
        public Move BestMove;

        public virtual double Max(ChessBoard board, bool isWhite, int currentNumOfMoves, int prevNumOfMoves, int currentNumOfAttackedPieces, int prevNumOfAttackedPieces, int depth = MAX_DEPTH)
        {
            if (depth == 0) return EvaluatePosition(board, isWhite, currentNumOfMoves, prevNumOfMoves, currentNumOfAttackedPieces, prevNumOfAttackedPieces);
            double max = MIN_INT;
            int counter = 0;
            List<PiecePossibleMove> moves = board.GetEveryLegalMove(isWhite);
            prevNumOfMoves = currentNumOfMoves;
            currentNumOfMoves = moves.Count;
            prevNumOfAttackedPieces = currentNumOfAttackedPieces;
            currentNumOfAttackedPieces = 0;      
            foreach(PiecePossibleMove move in moves)
            {
                int idx = (int)(move.MoveToPosition.Y * 8 + move.MoveToPosition.X);
                BasePiece piece = board.Board[idx].Piece;
                if (piece != null && piece.IsWhite != isWhite)
                {
                    currentNumOfAttackedPieces++;
                }
            }
            foreach (PiecePossibleMove move in moves)
            {              
                Move madeMove = board.MakeFakeMove(move, board.Board[(int)move.FromPosition.Y * 8 + (int)move.FromPosition.X].Piece.CopyPiece());
                double score = MAX_INT;
                if (depth == MAX_DEPTH && counter == 0)
                    BestMove = madeMove.CopyMove();
                if (board.FindKingLocation(!isWhite) != null)
                {
                    score = Min(board, !isWhite, currentNumOfMoves, prevNumOfMoves,currentNumOfAttackedPieces, prevNumOfAttackedPieces, depth - 1);
                }
                if (score > max)
                {
                    max = score;
                    if (depth == MAX_DEPTH)
                        BestMove = madeMove.CopyMove();
                }
                counter++;
                board.UnMakeLastMove(false);                                
            }
            return max;
        }

        public virtual double Min(ChessBoard board, bool isWhite, int currentNumOfMoves, int prevNumOfMoves, int currentNumOfAttackedPieces, int prevNumOfAttackedPieces, int depth = MAX_DEPTH)
        {
            if (depth == 0) return (-1)*EvaluatePosition(board, isWhite, currentNumOfMoves, prevNumOfMoves, currentNumOfAttackedPieces, prevNumOfAttackedPieces); 
            double min = MAX_INT;
            List<PiecePossibleMove> moves = board.GetEveryLegalMove(isWhite);
            prevNumOfMoves = currentNumOfMoves;
            currentNumOfMoves = moves.Count;
            prevNumOfAttackedPieces = currentNumOfAttackedPieces;
            currentNumOfAttackedPieces = 0;
            foreach (PiecePossibleMove move in moves)
            {
                int idx = (int)(move.MoveToPosition.Y * 8 + move.MoveToPosition.X);
                BasePiece piece = board.Board[idx].Piece;
                if (piece != null && piece.IsWhite != isWhite)
                {
                    currentNumOfAttackedPieces++;
                }
            }
            foreach (PiecePossibleMove move in moves)
            {
                board.MakeFakeMove(move, board.Board[(int)move.FromPosition.Y * 8 + (int)move.FromPosition.X].Piece.CopyPiece());
                double score = MIN_INT;
                if (board.FindKingLocation(!isWhite) != null)
                {
                    score = Max(board, !isWhite, currentNumOfMoves, prevNumOfMoves, currentNumOfAttackedPieces, prevNumOfAttackedPieces, depth - 1);
                }
                board.UnMakeLastMove(false);
                if (score < min)
                {
                    min = score;
                }                  
            }
            return min;
        }

        public virtual double EvaluatePosition(ChessBoard board, bool isWhite, int ourMoves, int opponentNumOfMoves, int numOfPiecesWeAttack, int numOfOurPiecesAttacked)
        {
            double eval = 0;
            const double CASTLE_BONUS = 10;
      
            foreach(BasePiece bp in board.ToBasePieceList())
            {
                if(bp != null)
                {
                    if (bp.IsWhite == isWhite)
                    {
                        if (bp is Pawn)
                        {
                            
                            eval += 1;
                                                      
                        }                          
                        if (bp is Knight)
                            eval += 3;
                        if (bp is Bishop)
                            eval += 3.3;
                        if (bp is Rook)
                            eval += 5;
                        if (bp is Queen)
                            eval += 9;                                            
                    }
                    else
                    {
                        if (bp is Pawn)
                        {
                            eval -= 1;
                        }
                        if (bp is Knight)
                            eval -= 3;
                        if (bp is Bishop)
                            eval -= 3.3;
                        if (bp is Rook)
                            eval -= 5;
                        if (bp is Queen)
                            eval -= 9;
                    }
                    
                }               
            }
            eval *= 2;
            // Check if we castled and if our opponent has castled
            if (board.hasCastled[isWhite])
            {
                eval += CASTLE_BONUS;
            }
            if (board.hasCastled[!isWhite])
            {
                eval -= CASTLE_BONUS;
            }
            // The more moves we can make the better
            eval += (ourMoves - opponentNumOfMoves) / 8;

            // The more pieces we attack the better
            eval += (numOfPiecesWeAttack - numOfOurPiecesAttacked) / 2;
            return eval;
        }
    }
}
