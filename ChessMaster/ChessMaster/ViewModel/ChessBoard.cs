using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using ChessMaster.Pieces;
using System.Windows;
using ChessMaster.Dialogs;

namespace ChessMaster.ViewModel
{
    public class ChessBoard
    {
        public BasePiece CurrentPiece = null;

        public int LastIndex = 0;

        public List<ChessCell> Board;

        public List<Move> HistoryOfMoves;

        public bool GameFinished = false;

        public ChessBoard()
        {
            List<BasePiece> blackPieces = new List<BasePiece>
                              { new Rook(0,0, false), new Knight(1,0, false),
                                new Bishop(2,0, false), new Queen(3,0, false),
                                new King(4,0, false), new Bishop(5,0, false),
                                new Knight(6,0, false), new Rook(7,0, false)};
            List<BasePiece> whitePieces = new List<BasePiece>
                              { new Rook(0,7), new Knight(1,7),
                                new Bishop(2,7), new Queen(3,7),
                                new King(4,7), new Bishop(5,7),
                                new Knight(6,7), new Rook(7,7)};
            Board = new List<ChessCell>();
            HistoryOfMoves = new List<Move>();
            for (int y = 0; y < 8; y++)
            {
                for (int x = 0; x < 8; x++)
                {
                    ChessCell c = new ChessCell
                    {
                        Background = new SolidColorBrush((x + y) % 2 == 1 ? Colors.Gray : Colors.WhiteSmoke),
                        BorderColor = new SolidColorBrush(Colors.Black),
                        Position = new Point(x, y)
                    };
                    if (y == 0)
                        c.Piece = blackPieces[x];
                    else if (y == 1)
                        //black pawns
                        c.Piece = new Pawn(x, y, false);
                    else if (y == 6)
                        //white pawns
                        c.Piece = new Pawn(x, y);
                    else if (y == 7)
                        c.Piece = whitePieces[x];
                    Board.Add(c);
                }
            }
        }

        /// <summary>
        /// Makes a move on chess board
        /// </summary>
        /// <returns></returns>
        public void MakeMove(int index)
        {           
            if (CurrentPiece != null)
            {
                AssignCellBlackBorder();
                BasePiece takenPiece = (Board[index].Piece == null ? null : CopyPiece(Board[index].Piece));
                Move madeMove;
                if (takenPiece == null)
                    madeMove = new Move(LastIndex, index, CopyPiece(CurrentPiece));
                else
                    madeMove = new Move(LastIndex, index, CopyPiece(CurrentPiece), takenPiece);
                HistoryOfMoves.Add(madeMove);
                if (CurrentPiece is Pawn && (Board[index].Position.Y == 0 || Board[index].Position.Y == 7))
                {
                    PromotionWindow dialog = new PromotionWindow(CurrentPiece.IsWhite);
                    if (dialog.ShowDialog() == true)
                    {
                        CurrentPiece = dialog.SelectedPiece;
                    }
                }
                bool isWhite = CurrentPiece.IsWhite;
                CurrentPiece.Position = Board[index].Position;
                CurrentPiece.IsFirstMove = false;
                Board[index].Piece = CurrentPiece;
                Board[LastIndex].Piece = null;
                LastIndex = 0;
                CurrentPiece = null;
                if (IsGameOver(!isWhite))
                {
                    MessageBox.Show("Game over! " + (isWhite ? "White" : "Black") + " wins!");
                    GameFinished = true;
                }
            }
        }

        public void MakeFakeMove(int index)
        {
            BasePiece takenPiece = (Board[index].Piece == null ? null : CopyPiece(Board[index].Piece));
            Move madeMove;
            if (takenPiece == null)
                madeMove = new Move(LastIndex, index, CopyPiece(CurrentPiece));
            else
                madeMove = new Move(LastIndex, index, CopyPiece(CurrentPiece), takenPiece);
            HistoryOfMoves.Add(madeMove);
            CurrentPiece.Position = Board[index].Position;
            Board[index].Piece = CurrentPiece;
            Board[LastIndex].Piece = null;
        }

        public void UnMakeLastMove()
        {
            AssignCellBlackBorder();
            HistoryOfMoves[HistoryOfMoves.Count - 1].UnMakeMove(ref Board);
            HistoryOfMoves.RemoveAt(HistoryOfMoves.Count - 1);
        }

        public void ShowPossibleMoves(int index)
        {
            if (CurrentPiece == null)
            {
                CurrentPiece = Board[index].Piece;
                LastIndex = index;
                foreach (Point p in GetOnlyLegalMoves(CurrentPiece))
                {
                    int moveIndex = (int)p.Y * 8 + (int)p.X;
                    Board[moveIndex].BorderColor = new SolidColorBrush(Colors.Red);
                }
            }
            else if (CurrentPiece != null && CurrentPiece != Board[index].Piece)
            {
                AssignCellBlackBorder();
                CurrentPiece = Board[index].Piece;
                LastIndex = index;
                foreach (Point p in GetOnlyLegalMoves(CurrentPiece))
                {
                    int moveIndex = (int)p.Y * 8 + (int)p.X;
                    Board[moveIndex].BorderColor = new SolidColorBrush(Colors.Red);
                }
            }
        }

        /// <summary>
        /// Returns a list of legal moves based on possibleMoves
        /// </summary>
        /// <returns></returns>
        private List<Point> GetOnlyLegalMoves(BasePiece pieceToBeChecked)
        {
            List<Point> legalMoves = new List<Point>();
            List<Point> possibleMoves = pieceToBeChecked.GetPossibleMoves(ToBasePieceList());           
            CurrentPiece = pieceToBeChecked;
            LastIndex = (int)pieceToBeChecked.Position.Y * 8 + (int)pieceToBeChecked.Position.X;
            foreach (Point p in possibleMoves)
            {
                MakeFakeMove((int)(p.Y * 8 + p.X));
                Point kingLocation = FindKingLocation(pieceToBeChecked.IsWhite);
                if (!IsAttacked(kingLocation, pieceToBeChecked.IsWhite))
                {
                    legalMoves.Add(p);
                }
                UnMakeLastMove();
            }
            return legalMoves;
        }

        /// <summary>
        /// Creates a new object of type piece identical to "bp" in order to avoid referencing the same piece in the code, which causes unwanted behavior and conflicts
        /// </summary>
        /// <returns></returns>
        public BasePiece CopyPiece(BasePiece bp)
        {
            if (bp is Pawn)
                return new Pawn((int)bp.Position.X, (int)bp.Position.Y, bp.IsWhite)
                {
                    IsFirstMove = bp.IsFirstMove
                };
            if (bp is Knight)
                return new Knight((int)bp.Position.X, (int)bp.Position.Y, bp.IsWhite);
            if (bp is Bishop)
                return new Bishop((int)bp.Position.X, (int)bp.Position.Y, bp.IsWhite);
            if (bp is Rook)
                return new Rook((int)bp.Position.X, (int)bp.Position.Y, bp.IsWhite);
            if (bp is Queen)
                return new Queen((int)bp.Position.X, (int)bp.Position.Y, bp.IsWhite);
            if (bp is King)
                return new King((int)bp.Position.X, (int)bp.Position.Y, bp.IsWhite);
            return null;
        }

        public Point FindKingLocation(bool isWhite)
        {
            return Board.Find(p => p.Piece is King && p.Piece.IsWhite == isWhite).Position;      
        }
        
        protected void AssignCellBlackBorder()
        {
            foreach (var cell in Board)
            {
                cell.BorderColor = new SolidColorBrush(Colors.Black);
            }
        }

        public bool IsGameOver(bool isWhite)
        {
            foreach (ChessCell c in Board)
            {
                if (c.Piece != null && c.Piece.IsWhite == isWhite)
                {
                    if (GetOnlyLegalMoves(c.Piece).Count > 0)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public List<BasePiece> ToBasePieceList()
        {
            return Board.Select(b => b.Piece).ToList();
        }

        public bool IsAttacked(Point p, bool isWhite)
        {
            int index = (int)p.Y * 8 + (int)p.X;
            var cellsUnderAttack = Board.Where(cell => cell.Piece != null && cell.Piece.IsWhite != isWhite)
                                .SelectMany(cell => cell.Piece.GetPossibleMoves(ToBasePieceList()));
            return cellsUnderAttack.Contains(p);
        }
    }
}
