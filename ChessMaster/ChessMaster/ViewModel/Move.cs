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
        protected int _fromCell;

        protected int _toCell;

        protected BasePiece _actualPiece;

        protected bool _isPieceTaken;

        protected BasePiece _takenPiece;

        public Move(int startIndex, int endIndex, BasePiece piece)
        {
            _fromCell = startIndex;
            _toCell = endIndex;
            _actualPiece = piece;
            _isPieceTaken = false;
            _takenPiece = null;
        }

        public Move(int startIndex, int endIndex, BasePiece piece, BasePiece takenPiece)
        {
            _fromCell = startIndex;
            _toCell = endIndex;
            _actualPiece = piece;
            _isPieceTaken = true;
            _takenPiece = takenPiece;
        }

        public void UnMakeMove(ref List<ChessCell> board)
        {
            if (_isPieceTaken)
                board[_toCell].Piece = _takenPiece;
            else
                board[_toCell].Piece = null;
            int y = _fromCell / 8;
            int x = _fromCell % 8;
            _actualPiece.Position = new Point(x, y);
            board[_fromCell].Piece = _actualPiece;
        }
    }
}
