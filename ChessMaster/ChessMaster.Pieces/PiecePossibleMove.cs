using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ChessMaster.Pieces
{
    public class PiecePossibleMove
    {
        public Point MoveToPosition { get; set; }

        public bool IsCastlingMove { get; set; }

        public BasePiece CastlingRook { get; set; }

        public Point RookPosition { get; set; }

        public PiecePossibleMove()
        {
            IsCastlingMove = false;
        }

        public PiecePossibleMove(Point p)
        {
            MoveToPosition = p;
            IsCastlingMove = false;
        }

        public PiecePossibleMove(Point p, bool isCastlingMove, BasePiece castlingRook, Point rookPosition)
        {
            MoveToPosition = p;
            IsCastlingMove = isCastlingMove;
            CastlingRook = castlingRook;
            RookPosition = rookPosition;
        }
    }
}
