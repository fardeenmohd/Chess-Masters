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

        public delegate int AddFunctionDelegate(List<BasePiece> board, Point point, ref List<Point> moves);

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
        /// Method should be overriden in the derived classes
        /// Return the list of point which are under attack for the given piece
        /// </summary>
        /// <param name="board"></param>
        /// <returns></returns>
        public virtual List<Point> GetCellsUnderAttack(List<BasePiece> board)
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
        protected List<Point> GetHorizontalMoves(AddFunctionDelegate addFunction, List<BasePiece> board, int moveLength = 1)
        {
            List<Point> moves = new List<Point>();
            int leftborder = (int)Position.X - moveLength > 0 ? (int)Position.X - moveLength : 0;
            int rightborder = (int)Position.X + moveLength < 7 ? (int)Position.X + moveLength : 7;
            for (int i = (int) Position.X - 1; i >= leftborder; i--)
            {
                int res = addFunction(board, new Point(i, Position.Y), ref moves);
                if (res == 1)
                    break;
                if (res == 2 && i != leftborder &&
                    (board[8 * (int)Position.Y + i - 1] == null || board[8 * (int)Position.Y + i - 1].IsWhite == IsWhite))
                {
                    moves.Add(new Point(i - 1, Position.Y));
                    break;
                }
            }
            for (int i = (int)Position.X + 1; i <= rightborder; i++)
            {
                int res = addFunction(board, new Point(i, Position.Y), ref moves);
                if (res == 1)
                    break;
                if (res == 2 && i != rightborder &&
                    (board[8 * (int)Position.Y + i + 1] == null || board[8 * (int)Position.Y + i + 1].IsWhite == IsWhite))
                {
                    moves.Add(new Point(i + 1, Position.Y));
                    break;
                }
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
        protected List<Point> GetVerticalMoves(AddFunctionDelegate addFunction, List<BasePiece> board, int moveLength = 1)
        {
            List<Point> moves = new List<Point>();
            int upborder = (int)Position.Y - moveLength > 0 ? (int)Position.Y - moveLength : 0;
            int bottomborder = (int)Position.Y + moveLength < 7 ? (int)Position.Y + moveLength : 7;
            for (int i = (int) Position.Y - 1; i >= upborder; i--)
            {
                int res = addFunction(board, new Point(Position.X, i), ref moves);
                if (res == 1)
                    break;
                if(res == 2 && i != upborder
                    && (board[8 * (i - 1) + (int)Position.X] == null || board[8 * (i - 1) + (int)Position.X].IsWhite == IsWhite))
                 {
                    moves.Add(new Point(Position.X, i - 1));
                    break;
                }
            }
            for (int i = (int)Position.Y + 1;  i <= bottomborder; i++)
            {
                int res = addFunction(board, new Point(Position.X, i), ref moves);
                if (res == 1)
                    break;
                if (res == 2 && i != bottomborder
                    && (board[8 * (i + 1) + (int)Position.X] == null || board[8 * (i + 1) + (int)Position.X].IsWhite == IsWhite))
                {
                    moves.Add(new Point(Position.X, i + 1));
                    break;
                }
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
        protected List<Point> GetDiagonalMoves(AddFunctionDelegate addFunction, List<BasePiece> board, int moveLength = 1)
        {
            List<Point> moves = new List<Point>();
            for (int i = (int)Position.Y - 1, step = 1; step + Position.X <= 7 && step <= moveLength && i >= 0; step++, i--)
            {
                int res = addFunction(board, new Point(Position.X + step, i), ref moves);
                if (res == 1)
                    break;
                if (res == 2 && step + Position.X != 7 && step != moveLength && i != 0
                    && (board[8 *  (i - 1) + (int)Position.X + step + 1] == null || board[8 * (i - 1) + (int)Position.X + step + 1].IsWhite == IsWhite))
                {
                    moves.Add(new Point(Position.X + step + 1, i - 1));
                    break;
                }
            }
            for (int i = (int)Position.Y - 1, step = 1; Position.X - step >= 0 && step <= moveLength && i >= 0; step++, i--)
            {
                int res = addFunction(board, new Point(Position.X - step, i), ref moves);
                if (res == 1)
                    break;
                if (res == 2 && Position.X - step != 0 && step != moveLength && i != 0
                    && (board[8 * (i - 1) + (int)Position.X - step - 1] == null || board[8 * (i - 1) + (int)Position.X - step - 1].IsWhite == IsWhite))
                {
                    moves.Add(new Point(Position.X - step - 1, i - 1));
                    break;
                }
            }
            for (int i = (int)Position.Y + 1, step = 1; step + Position.X <= 7 && step <= moveLength && i < 8; step++, i++)
            {
                int res = addFunction(board, new Point(Position.X + step, i), ref moves);
                if (res == 1)
                    break;
                if (res == 2 && step + Position.X != 7 && step != moveLength && i != 7
                    && (board[8 * (i + 1) + (int)Position.X + step + 1] == null || board[8 * (i + 1) + (int)Position.X + step + 1].IsWhite == IsWhite))
                {
                    moves.Add(new Point(Position.X + step + 1, i + 1));
                    break;
                }
            }
            for (int i = (int)Position.Y + 1, step = 1; Position.X - step >= 0 && step <= moveLength && i < 8; step++, i++)
            {
                int res = addFunction(board, new Point(Position.X - step, i), ref moves);
                if (res == 1)
                    break;
                if (res == 2 && Position.X - step != 0 && step != moveLength && i != 7
                    && (board[8 * (i + 1) + (int)Position.X - step - 1] == null || board[8 * (i + 1) + (int)Position.X - step - 1].IsWhite == IsWhite))
                {
                    moves.Add(new Point(Position.X - step - 1, i + 1));
                    break;
                }
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
        /// isWhitePawn indicates which pawn it is (White pawn moves up and black pawn moves down)
        /// </summary>
        /// <param name="isFirstMove"></param>
        /// <param name="isWhitePawn"></param>
        /// <returns></returns>
        protected List<Point> GetPawnMoves(List<BasePiece> board, bool isFirstMove = false, bool isWhitePawn = true)
        {
            List<Point> moves = new List<Point>();
            int offset = isWhitePawn ? -1 : 1;
            int doubleOffset = isWhitePawn ? -2 : 2;
            if (board[(int)Position.X + 8 * ((int)Position.Y + offset)] == null)
            {
                moves.Add(new Point(Position.X, Position.Y + offset));
                if (IsFirstMove && board[(int)Position.X + 8 * ((int)Position.Y + doubleOffset)] == null)
                    moves.Add(new Point(Position.X, Position.Y + doubleOffset));
            }
            AddPawnAttackMoves(board, ref moves, isWhitePawn);
            return moves;
        }
        
        /// <summary>
        /// Method adds moves to the list if this is possible
        /// Method returns 1, when there is a piece which blocks its way
        /// </summary>
        /// <param name="board"></param>
        /// <param name="point"></param>
        /// <param name="moves"></param>
        /// <returns></returns>
        protected int AddMove(List<BasePiece> board, Point point, ref List<Point> moves)
        {
            int index = 8*(int)point.Y + (int)point.X;
            if(board[index] == null)
            {
                moves.Add(point);
                return 0;
            }
            else if(board[index].IsWhite != IsWhite)
            {
                moves.Add(point);
                return 1;
            }
            return 1;
        }

        /// <summary>
        /// Method adds the cells under attack 
        /// Returns:
        /// 0 - nothing blocks, 1 - enemy piece is ahead, 2 - enemy king is ahead
        /// </summary>
        /// <param name="board"></param>
        /// <param name="point"></param>
        /// <param name="moves"></param>
        /// <returns></returns>
        protected int AddCellUnderAttack(List<BasePiece> board, Point point, ref List<Point> moves)
        {
            int index = 8 * (int)point.Y + (int)point.X;
            moves.Add(point);
            if (board[index] == null)
                return 0;
            else if (board[index] != null && board[index] is King && board[index].IsWhite != IsWhite)
            { 
                return 2;
            }
            return 1;
        }

        /// <summary>
        /// Method adds diagonal attacking moves for the Pawn
        /// </summary>
        /// <param name="board"></param>
        /// <param name="moves"></param>
        /// <param name="isWhitePawn"></param>
        protected void AddPawnAttackMoves(List<BasePiece> board, ref List<Point> moves, bool isWhitePawn)
        {
            int offset = isWhitePawn ? -1 : 1;
            BasePiece leftCell = board[((int)Position.Y + offset) * 8 + (int)Position.X - 1];
            BasePiece rightCell = board[((int)Position.Y + offset) * 8 + (int)Position.X + 1];
            if (leftCell != null && leftCell.IsWhite != IsWhite && (int)Position.X != 0)
                moves.Add(new Point(Position.X - 1, Position.Y + offset));
            if (rightCell != null && rightCell.IsWhite != IsWhite && (int)Position.X != 7)
                moves.Add(new Point(Position.X + 1, Position.Y + offset));
        }

        protected List<Point> GetPawnCellsUnderAttack(List<BasePiece> board, bool isWhitePawn = true)
        {
            int offset = isWhitePawn ? -1 : 1;
            List<Point> cells = new List<Point>();
            if((int)Position.X != 0)
                cells.Add(new Point(Position.X - 1, Position.Y + offset));
            if((int)Position.X != 7)
                cells.Add(new Point(Position.X + 1, Position.Y + offset));
            return cells;
        }
    }
}
