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
        public Point FromPosition { get; set; }
        public bool IsCastlingMove { get; set; }

        public BasePiece CastlingRook { get; set; }

        public Point RookPosition { get; set; }

        public PiecePossibleMove()
        {
            IsCastlingMove = false;
        }

        public PiecePossibleMove(Point toPosition, Point fromPosition)
        {
            MoveToPosition = toPosition;
            FromPosition = fromPosition;
            IsCastlingMove = false;
        }

        public PiecePossibleMove(Point toPosition, Point fromPosition, bool isCastlingMove, BasePiece castlingRook, Point rookPosition)
        {
            MoveToPosition = toPosition;
            FromPosition = fromPosition;
            IsCastlingMove = isCastlingMove;
            CastlingRook = castlingRook;
            RookPosition = rookPosition;
        }
        public PiecePossibleMove CopyPiecePossibleMove()
        {
            if(CastlingRook != null)
                return new PiecePossibleMove()
                {
                    MoveToPosition = new Point(MoveToPosition.X, MoveToPosition.Y),
                    FromPosition = new Point(FromPosition.X, FromPosition.Y),
                    IsCastlingMove = IsCastlingMove,
                    CastlingRook = CastlingRook.CopyPiece(),
                    RookPosition = new Point(RookPosition.X, RookPosition.Y)
                };
            else
                return new PiecePossibleMove()
                {
                    MoveToPosition = new Point(MoveToPosition.X, MoveToPosition.Y),
                    FromPosition = new Point(FromPosition.X, FromPosition.Y),
                    IsCastlingMove = IsCastlingMove,
                    RookPosition = new Point(RookPosition.X, RookPosition.Y)
                };
        }
    }
}
