using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChessMaster.Pieces;
using System.Windows;

namespace ChessMaster.ViewModel
{
    public class Move
    {
        protected PiecePossibleMove _move; //Set so we can call CopyMove() and pass the same move

        protected int _fromPosition;

        protected int _toPosition;

        protected BasePiece _actualPiece;

        protected bool _isPieceTaken;

        protected BasePiece _takenPiece;

        protected int _takenPiecePosition;

        protected bool _isCastling;

        protected BasePiece _castlingRook;

        protected int _castlingRookFromPosition;

        protected int _castlingRookToPosition;

        protected bool _isPromotionMove;

        protected BasePiece _promotionPiece;

        protected bool _hasMadeMove = false;

        public bool IsCastlingMove { get { return _isCastling; } /*set { _isCastling = value; }*/ }
        public bool IsWhiteMove { get { return _actualPiece.IsWhite; } }
        public Move(PiecePossibleMove move, BasePiece piece, BasePiece takenPiece = null, BasePiece promotionPiece = null)
        {
            _move = move;
            _fromPosition = (int)piece.Position.X + (int)piece.Position.Y * 8;
            _toPosition = (int)move.MoveToPosition.X + (int)move.MoveToPosition.Y * 8;
            _actualPiece = piece.CopyPiece();
            _isPieceTaken = takenPiece == null ? false : true;
            _takenPiece = takenPiece == null ? null : takenPiece.CopyPiece();
            _takenPiecePosition = takenPiece == null ? 0 : (int)takenPiece.Position.X + (int)takenPiece.Position.Y * 8;
            _isCastling = move.IsCastlingMove;
            _castlingRook = move.IsCastlingMove ? move.CastlingRook.CopyPiece() : null;
            _castlingRookFromPosition = move.IsCastlingMove ? (int)move.CastlingRook.Position.X + (int)move.CastlingRook.Position.Y * 8 : 0;
            _castlingRookToPosition = move.IsCastlingMove ? (int)move.RookPosition.X + (int)move.RookPosition.Y * 8 : 0;
            _isPromotionMove = promotionPiece == null ? false : true;
            _promotionPiece = promotionPiece == null ? null : promotionPiece.CopyPiece();
            _hasMadeMove = true;
        }

        public void MakeMove(ref List<ChessCell> board)
        {
            if(_isPieceTaken)
                board[_takenPiecePosition].Piece = null;
            if(_isCastling)
            {
                board[_castlingRookFromPosition].Piece = null;
                BasePiece rook = _castlingRook.CopyPiece();
                rook.Position = board[_castlingRookToPosition].Position;
                board[_castlingRookToPosition].Piece = rook;
                board[_castlingRookToPosition].Piece.IsFirstMove = false;
            }
            BasePiece movablePiece = _isPromotionMove ? _promotionPiece.CopyPiece() : _actualPiece.CopyPiece();
            movablePiece.Position = board[_toPosition].Position;
            board[_toPosition].Piece = movablePiece;
            board[_toPosition].Piece.IsFirstMove = false;
            board[_fromPosition].Piece = null;
            _hasMadeMove = true;
        }

        public void UnMakeMove(ref List<ChessCell> board)
        {
            if(_isPieceTaken)
                board[_takenPiecePosition].Piece = _takenPiece;
            else
                board[_toPosition].Piece = null;
            _actualPiece.Position = board[_fromPosition].Position;
            board[_fromPosition].Piece = _actualPiece;
            if(_isCastling)
            {
                board[_castlingRookToPosition].Piece = null;
                _castlingRook.Position = board[_castlingRookFromPosition].Position;
                board[_castlingRookFromPosition].Piece = _castlingRook; 
            }
            _hasMadeMove = false;
        }
        //Shows the move in algebraic notation. Example: Ne4, d4, Qb3
        //Should only be called after MakeMove()
        public override string ToString()
        {
            StringBuilder moveStr = new StringBuilder();
            if (_isCastling && _castlingRookFromPosition % 8 == 0)
                return "0-0-0";
            if (_isCastling && _castlingRookFromPosition % 8 != 0)
                return "0-0";
            if(_hasMadeMove)
            {
                //First we check what piece
                moveStr.Append(RecognisePiece(_actualPiece));
                //If piece was taken we add an "x" which means the move involves capturing a piece
                if (_takenPiece != null)
                {
                    if(_actualPiece is Pawn)
                    {
                        int previousX = _fromPosition % 8;
                        moveStr.Append((char)(Convert.ToUInt16('a') + previousX));
                    }                   
                    moveStr.Append("x");
                }
                              
            }
            char file = (char)(Convert.ToUInt16('a') + _toPosition % 8);
            moveStr.Append(file);
            moveStr.Append(8 - _toPosition / 8);
            if(_isPromotionMove)
            {
                moveStr.Append("=");
                moveStr.Append(RecognisePiece(_promotionPiece));
            }
            return moveStr.ToString();
        }

        protected string RecognisePiece(BasePiece piece)
        {
            if (piece is Knight)
                return "N";
            if (piece is Bishop)
                return "B";
            if (piece is Rook)
                return "R";
            if (piece is Queen)
                return "Q";
            if (piece is King)
                return "K";
            return "";
        }

        public Move CopyMove()
        {
            PiecePossibleMove piecePossibleMove = new PiecePossibleMove(new Point(_move.MoveToPosition.X, _move.MoveToPosition.Y), new Point(_move.FromPosition.X, _move.FromPosition.Y))
            {
                IsCastlingMove = _move.IsCastlingMove,
                CastlingRook = _move.CastlingRook,
                RookPosition = new Point(_move.RookPosition.X, _move.RookPosition.Y)
            };

            return new Move(piecePossibleMove, _actualPiece, _takenPiece, _promotionPiece);
        }
    }
}
