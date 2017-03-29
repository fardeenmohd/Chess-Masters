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

        public BasePiece LastPiece = null;

        public BasePiece LastTakenPiece = null;

        public int LastIndex = 0;

        public int LastMoveIndex = 0; //The index of the last place we moved to, so we can UnmakeMove()

        public BasePiece ActualCurrentPiece = null;

        public int ActualLastIndex = 0;

        public BasePiece ActualLastTakenPiece = null;

        public BasePiece ActualLastPiece = null;

        public int ActualLastMoveIndex = 0;


        public List<ChessCell> Board;

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
                    {
                        c.Piece = blackPieces[x];
                    }
                    else if (y == 1)
                    {
                        //black pawns
                        c.Piece = new Pawn(x, y, false);
                    }
                    else if (y == 6)
                    {
                        //white pawns
                        c.Piece = new Pawn(x, y);
                    }
                    else if (y == 7)
                    {
                        c.Piece = whitePieces[x];
                    }
                    Board.Add(c);
                }
            }
        }
        /// <summary>
        /// Makes a move on chess board. If searchingForMove is true, it means we are using this method to simply check for legal moves
        /// instead of actually trying to move
        /// If it is false, the move will be shown on the board and we check for game over
        /// </summary>
        /// <returns></returns>
        public void MakeMove(int index, bool searchingForMove = true)
        {           
            if (CurrentPiece != null)
            {
                AssignCellBlackBorder();
                if(searchingForMove)
                {
                    if (CurrentPiece is Pawn && (Board[index].Position.Y == 0 || Board[index].Position.Y == 7))
                    {
                        //TODO somehow all possible pawn promotions should be searched for
                    }
                    LastPiece = CopyPiece(CurrentPiece);
                    CurrentPiece.Position = Board[index].Position;
                    CurrentPiece.IsFirstMove = false;
                    LastTakenPiece = (Board[index].Piece == null ? null : CopyPiece(Board[index].Piece));
                    LastMoveIndex = index;
                    Board[index].Piece = CurrentPiece;
                    Board[LastIndex].Piece = null;
                    CurrentPiece = null;
                }                               
                else
                {
                    if (CurrentPiece is Pawn && (Board[index].Position.Y == 0 || Board[index].Position.Y == 7))
                    {
                        PromotionWindow dialog = new PromotionWindow(CurrentPiece.IsWhite);
                        if (dialog.ShowDialog() == true)
                        {
                            CurrentPiece = dialog.SelectedPiece;
                        }
                    }
                    ActualLastPiece = CopyPiece(CurrentPiece);
                    CurrentPiece.Position = Board[index].Position;
                    CurrentPiece.IsFirstMove = false;
                    ActualLastTakenPiece = (Board[index].Piece == null ? null : CopyPiece(Board[index].Piece));
                    ActualLastMoveIndex = index;
                    Board[index].Piece = CurrentPiece;
                    Board[LastIndex].Piece = null;
                    bool isWhite = CurrentPiece.IsWhite;
                    CurrentPiece = null;
                    if (IsGameOver(!isWhite))
                    {
                        MessageBox.Show("Game over! " + (isWhite ? "White" : "Black") + " wins!");
                    }
                }
                
            }
        }
        /// <summary>
        /// Unmakes the last move and sets CurrentPiece back to its previous value
        /// </summary>
        /// <returns></returns>
        public void UnmakeLastMove(bool searchingForMove=true)
        {
            if(searchingForMove)
            {
                if (LastPiece != null)
                {
                    //Putting the piece back to the previous square
                    Board[LastIndex].Piece = LastPiece;
                    Board[LastIndex].Position = LastPiece.Position;
                    CurrentPiece = CopyPiece(LastPiece);
                    CurrentPiece.Position = LastPiece.Position;
                    //Handling of the square we moved back from
                    if (LastTakenPiece != null)
                    {
                        Board[LastMoveIndex].Piece = LastTakenPiece;
                        Board[LastMoveIndex].Position = LastTakenPiece.Position;
                    }
                    else
                    {
                        Board[LastMoveIndex].Piece = null;
                    }

                    LastTakenPiece = null;
                    LastPiece = null;
                }
            }
            else
            {
                if(ActualLastPiece != null)
                {
                    //Putting the piece back to the previous square
                    Board[ActualLastIndex].Piece = ActualLastPiece;
                    Board[ActualLastIndex].Position = ActualLastPiece.Position;
                    CurrentPiece = CopyPiece(ActualLastPiece);
                    CurrentPiece.Position = ActualLastPiece.Position;
                    //Handling of the square we moved back from
                    if (ActualLastTakenPiece != null)
                    {
                        Board[ActualLastMoveIndex].Piece = ActualLastTakenPiece;
                        Board[ActualLastMoveIndex].Position = ActualLastTakenPiece.Position;
                    }
                    else
                    {
                        Board[ActualLastMoveIndex].Piece = null;
                    }

                    ActualLastTakenPiece = null;
                    ActualLastPiece = null;
                }              
            }
            
        }
        public void ShowPossibleMoves(int index)
        {
            if (ActualCurrentPiece == null)
            {
                ActualCurrentPiece = Board[index].Piece;
                ActualLastIndex = index;
                foreach (Point p in GetOnlyLegalMoves(ActualCurrentPiece))
                {
                    int moveIndex = (int)p.Y * 8 + (int)p.X;
                    Board[moveIndex].BorderColor = new SolidColorBrush(Colors.Red);
                }
            }
            else if (ActualCurrentPiece != null && ActualCurrentPiece != Board[index].Piece)
            {
                AssignCellBlackBorder();
                ActualCurrentPiece = Board[index].Piece;
                ActualLastIndex = index;
                foreach (Point p in GetOnlyLegalMoves(ActualCurrentPiece))
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
                MakeMove((int)(p.Y * 8 + p.X));
                Point kingLocation = FindKingLocation(pieceToBeChecked.IsWhite);
                if (!IsAttacked(kingLocation, pieceToBeChecked.IsWhite))
                {
                    legalMoves.Add(p);
                }
                UnmakeLastMove();
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
