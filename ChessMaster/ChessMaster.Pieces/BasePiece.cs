using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows;

namespace ChessMaster.Pieces
{
    /// <summary>
    /// Base class for all pieces 
    /// </summary>
    public class BasePiece
    {
        #region Constants
        protected const int BOARDLENGTH = 8;
        #endregion

        public Point Position { get; set; }

        public BitmapImage PieceImage { get; set; }

        public BasePiece(int x, int y)
        {
            Position = new Point(x, y);
        }

        /// <summary>
        /// Method should be overriden in the derived classes
        /// </summary>
        /// <returns></returns>
        public virtual List<Point> GetPossibleMoves()
        {
            return new List<Point>();
        }

        /// <summary>
        /// Method returns the list of possible horizontal moves made by piece
        /// moveLength denotes how far the piece can move on the horizontal line 
        /// Usage: King, Queen
        /// </summary>
        /// <param name="moveLength"></param>
        /// <returns></returns>
        protected List<Point> GetHorizontalMoves(int moveLength = 1)
        {
            List<Point> moves = new List<Point>();
            int leftborder = (int)Position.X - moveLength > 0 ? (int)Position.X - moveLength : 0;
            int rightborder = (int)Position.X + moveLength < 7 ? (int)Position.X + moveLength : 7;
            for (int i = leftborder; i < Position.X; i++)
                moves.Add(new Point(i, Position.Y));
            for (int i = rightborder; i > Position.X; i--)
                moves.Add(new Point(i, Position.Y));
            return moves;
        }

        /// <summary>
        /// Method returns the list of possible vertical moves made by piece
        /// moveLength denotes how far the piece can move on the vertical line 
        /// Usage: King, Queen, Rock
        /// </summary>
        /// <param name="moveLength"></param>
        /// <returns></returns>
        protected List<Point> GetVerticalMoves(int moveLength = 1)
        {
            List<Point> moves = new List<Point>();
            int upborder = (int)Position.Y - moveLength > 0 ? (int)Position.X - moveLength : 0;
            int bottomborder = (int)Position.X + moveLength < 7 ? (int)Position.X + moveLength : 7;
            for (int i = upborder; i < Position.Y; i++)
                moves.Add(new Point(Position.X, i));
            for (int i = bottomborder; i > Position.Y; i--)
                moves.Add(new Point(Position.X, i));
            return moves;
        }

        /// <summary>
        /// Method returns the list of possible diagonal moves made by piece
        /// moveLength denotes how far the piece can move on the diagonal 
        /// Usage: King, Queen, Bishop
        /// </summary>
        /// <param name="moveLength"></param>
        /// <returns></returns>
        protected List<Point> GetDiagonalMoves(int moveLength = 1)
        {
            List<Point> moves = new List<Point>();
            for (int i = (int)Position.Y - 1, step = 1; step <= moveLength && i >= 0; step++, i--)
            {
                if (step + Position.X <= 7)
                    moves.Add(new Point(Position.X + step, i));
                if (Position.X - step >= 0)
                    moves.Add(new Point(Position.X - step, i));
            }
            for (int i = (int)Position.Y + 1, step = 1; step <= moveLength && i < 8; step++, i++)
            {
                if (step + Position.X <= 7)
                    moves.Add(new Point(Position.X + step, i));
                if (Position.X - step >= 0)
                    moves.Add(new Point(Position.X - step, i));
            }
            return moves;
        }


        /// <summary>
        /// Method returns a list of possible moves made by Knight
        /// </summary>
        /// <returns></returns>
        protected List<Point> GetKnightMoves()
        {
            List<Point> moves = new List<Point>();
            if (Position.X - 2 >= 0)
            {
                if (Position.Y < 7)
                    moves.Add(new Point(Position.X - 2, Position.Y + 1));
                if (Position.Y > 0)
                    moves.Add(new Point(Position.X - 2, Position.Y - 1));
            }
            if (Position.X < 6)
            {
                if (Position.Y < 7)
                    moves.Add(new Point(Position.X + 2, Position.Y + 1));
                if (Position.Y > 0)
                    moves.Add(new Point(Position.X + 2, Position.Y - 1));
            }
            if (Position.Y - 2 >= 0)
            {
                if (Position.X < 7)
                    moves.Add(new Point(Position.X + 1, Position.Y - 2));
                if (Position.X > 0)
                    moves.Add(new Point(Position.X - 1, Position.Y - 2));
            }
            if (Position.Y < 6)
            {
                if (Position.X < 7)
                    moves.Add(new Point(Position.X + 1, Position.Y + 2));
                if (Position.X > 0)
                    moves.Add(new Point(Position.X - 1, Position.Y + 2));
            }
            return moves;
        }

        /// <summary>
        /// This method returns basic moves of the pawn
        /// isFirstMove indicates whether the move is first one done by the pawn
        /// isUserPawn indicates who is the owner of the pawn (AI or User)
        /// </summary>
        /// <param name="isFirstMove"></param>
        /// <param name="isUserPawn"></param>
        /// <returns></returns>
        protected List<Point> GetPawnMoves(bool isFirstMove = false, bool isUserPawn = true)
        {
            List<Point> moves = new List<Point>();
            if (isUserPawn)
                moves.Add(new Point(Position.X, Position.Y - 1));
            else
                moves.Add(new Point(Position.X, Position.Y + 1));
            if(isFirstMove)
            {
                if(isUserPawn)
                    moves.Add(new Point(Position.X, Position.Y - 2));
                else
                    moves.Add(new Point(Position.X, Position.Y + 2));
            }
            return moves;
        }
    }
}
