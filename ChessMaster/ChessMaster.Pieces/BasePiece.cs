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

        public string PieceImage { get; set; }

        public bool IsWhite { get; set; }

        //Used for pawns only
        public bool IsFirstMove { get; set; }

        public BasePiece(int x, int y, bool isWhite = true)
        {
            Position = new Point(x, y);
            IsWhite = isWhite;
            IsFirstMove = true;
        }

        /// <summary>
        /// Method should be overriden in the derived classes
        /// </summary>
        /// <returns></returns>
        public virtual List<Point> GetPossibleMoves(List<BasePiece> board)
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
        protected List<Point> GetHorizontalMoves(List<BasePiece> board, int moveLength = 1)
        {
            List<Point> moves = new List<Point>();
            int leftborder = (int)Position.X - moveLength > 0 ? (int)Position.X - moveLength : 0;
            int rightborder = (int)Position.X + moveLength < 7 ? (int)Position.X + moveLength : 7;
            for (int i = (int) Position.X - 1; i >= leftborder; i--)
            {
                if (AddMove(board, new Point(i, Position.Y), ref moves))
                    break;
            }
            for (int i = (int)Position.X + 1; i <= rightborder; i++)
            {
                if (AddMove(board, new Point(i, Position.Y), ref moves))
                    break;
            }
            return moves;
        }

        /// <summary>
        /// Method returns the list of possible vertical moves made by piece
        /// moveLength denotes how far the piece can move on the vertical line 
        /// Usage: King, Queen, Rook
        /// </summary>
        /// <param name="moveLength"></param>
        /// <returns></returns>
        protected List<Point> GetVerticalMoves(List<BasePiece> board, int moveLength = 1)
        {
            List<Point> moves = new List<Point>();
            int upborder = (int)Position.Y - moveLength > 0 ? (int)Position.Y - moveLength : 0;
            int bottomborder = (int)Position.Y + moveLength < 7 ? (int)Position.Y + moveLength : 7;
            for (int i = (int) Position.Y - 1; i >= upborder; i--)
            {
                if (AddMove(board, new Point(Position.X, i), ref moves))
                    break;
            }
            for (int i = (int)Position.Y + 1;  i <= bottomborder; i++)
            {
                if (AddMove(board, new Point(Position.X, i), ref moves))
                    break;
            }
            return moves;
        }

        /// <summary>
        /// Method returns the list of possible diagonal moves made by piece
        /// moveLength denotes how far the piece can move on the diagonal 
        /// Usage: King, Queen, Bishop
        /// </summary>
        /// <param name="moveLength"></param>
        /// <returns></returns>
        protected List<Point> GetDiagonalMoves(List<BasePiece> board, int moveLength = 1)
        {
            List<Point> moves = new List<Point>();
            for (int i = (int)Position.Y - 1, step = 1; step + Position.X <= 7 && step <= moveLength && i >= 0; step++, i--)
            {
                    if (AddMove(board, new Point(Position.X + step, i), ref moves))
                        break;
            }
            for (int i = (int)Position.Y - 1, step = 1; Position.X - step >= 0 && step <= moveLength && i >= 0; step++, i--)
            {
                if (AddMove(board, new Point(Position.X - step, i), ref moves))
                    break;
            }
            for (int i = (int)Position.Y + 1, step = 1; step + Position.X <= 7 && step <= moveLength && i < 8; step++, i++)
            {
                if (AddMove(board, new Point(Position.X + step, i), ref moves))
                    break;
            }
            for (int i = (int)Position.Y + 1, step = 1; Position.X - step >= 0 && step <= moveLength && i < 8; step++, i++)
            {
                if (AddMove(board, new Point(Position.X - step, i), ref moves))
                    break;
            }
            return moves;
        }


        /// <summary>
        /// Method returns a list of possible moves made by Knight
        /// </summary>
        /// <returns></returns>
        protected List<Point> GetKnightMoves(List<BasePiece> board)
        {
            List<Point> moves = new List<Point>();
            if (Position.X - 2 >= 0)
            {
                if (Position.Y < 7)
                    AddMove(board, new Point(Position.X - 2, Position.Y + 1), ref moves);
                if (Position.Y > 0)
                    AddMove(board, new Point(Position.X - 2, Position.Y - 1), ref moves);
            }
            if (Position.X < 6)
            {
                if (Position.Y < 7)
                    AddMove(board, new Point(Position.X + 2, Position.Y + 1), ref moves);
                if (Position.Y > 0)
                    AddMove(board, new Point(Position.X + 2, Position.Y - 1), ref moves);
            }
            if (Position.Y - 2 >= 0)
            {
                if (Position.X < 7)
                    AddMove(board, new Point(Position.X + 1, Position.Y - 2), ref moves);
                if (Position.X > 0)
                    AddMove(board, new Point(Position.X - 1, Position.Y - 2), ref moves);
            }
            if (Position.Y < 6)
            {
                if (Position.X < 7)
                    AddMove(board, new Point(Position.X + 1, Position.Y + 2), ref moves);
                if (Position.X > 0)
                    AddMove(board, new Point(Position.X - 1, Position.Y + 2), ref moves);
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
        protected List<Point> GetPawnMoves(List<BasePiece> board, bool isFirstMove = false, bool isUserPawn = true)
        {
            List<Point> moves = new List<Point>();
            int offset = isUserPawn ? -1 : 1;
            int doubleOffset = isUserPawn ? -2 : 2;
            if (board[(int)Position.X + 8 * ((int)Position.Y + offset)] == null)
            {
                moves.Add(new Point(Position.X, Position.Y + offset));
                if (IsFirstMove && board[(int)Position.X + 8 * ((int)Position.Y + doubleOffset)] == null)
                    moves.Add(new Point(Position.X, Position.Y + doubleOffset));
            }
            AddPawnAttackMoves(board, ref moves, isUserPawn);
            return moves;
        }
        
        /// <summary>
        /// Method adds moves to the list if this is possible
        /// Method returns true, when there is a piece which blocks its way
        /// </summary>
        /// <param name="board"></param>
        /// <param name="point"></param>
        /// <param name="moves"></param>
        /// <returns></returns>
        private bool AddMove(List<BasePiece> board, Point point, ref List<Point> moves)
        {
            int index = 8*(int)point.Y + (int)point.X;
            if(board[index]==null)
            {
                moves.Add(point);
                return false;
            }
            else if(board[index].IsWhite != IsWhite)
            {
                moves.Add(point);
                return true;
            }
            return true;
        }

        /// <summary>
        /// Method adds diagonal attacking moves for the Pawn
        /// </summary>
        /// <param name="board"></param>
        /// <param name="moves"></param>
        /// <param name="isUserPawn"></param>
        private void AddPawnAttackMoves(List<BasePiece> board, ref List<Point> moves, bool isUserPawn)
        {
            int offset = isUserPawn ? -1 : 1;
            BasePiece leftCell = board[((int)Position.Y + offset) * 8 + (int)Position.X - 1];
            BasePiece rightCell = board[((int)Position.Y + offset) * 8 + (int)Position.X + 1];
            if (leftCell != null && leftCell.IsWhite != IsWhite)
                moves.Add(new Point(Position.X - 1, Position.Y + offset));
            if (rightCell != null && rightCell.IsWhite != IsWhite)
                moves.Add(new Point(Position.X + 1, Position.Y + offset));
        }
    }
}
