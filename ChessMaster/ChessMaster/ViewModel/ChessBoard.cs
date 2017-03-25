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

        public void MakeMove(int index)
        {
            if (CurrentPiece != null)
            {
                AssignCellBlackBorder();
                if(CurrentPiece is Pawn && (Board[index].Position.Y == 0 || Board[index].Position.Y == 7))
                {
                    PromotionWindow dialog = new PromotionWindow(CurrentPiece.IsWhite);
                    if (dialog.ShowDialog() == true)
                    {
                        CurrentPiece = dialog.SelectedPiece;
                    }
                }
                CurrentPiece.Position = Board[index].Position;
                CurrentPiece.IsFirstMove = false;
                Board[index].Piece = CurrentPiece;
                Board[LastIndex].Piece = null;
                LastIndex = 0;
                CurrentPiece = null;
            }
        }

        public void ShowPossibleMoves(int index)
        {
            if (CurrentPiece == null)
            {
                CurrentPiece = Board[index].Piece;
                LastIndex = index;
                List<Point> possibleMoves = CurrentPiece.GetPossibleMoves(ToBasePieceList());
                foreach (Point p in possibleMoves)
                {
                    if (CurrentPiece is King)
                    {
                        if (!IsAttacked(p))
                        {
                            index = (int)p.Y * 8 + (int)p.X;
                            Board[index].BorderColor = new SolidColorBrush(Colors.Red);
                        }
                    }
                    else
                    {
                        index = (int)p.Y * 8 + (int)p.X;
                        Board[index].BorderColor = new SolidColorBrush(Colors.Red);
                    }
                }
            }
            else if (CurrentPiece != null && CurrentPiece != Board[index].Piece)
            {
                AssignCellBlackBorder();
                CurrentPiece = Board[index].Piece;
                LastIndex = index;
                List<Point> possibleMoves = CurrentPiece.GetPossibleMoves(ToBasePieceList());
                foreach (Point p in possibleMoves)
                {
                    if(CurrentPiece is King)
                    {
                        if (!IsAttacked(p))
                        {
                            index = (int)p.Y * 8 + (int)p.X;
                            Board[index].BorderColor = new SolidColorBrush(Colors.Red);
                        }
                    }                   
                    else
                    {
                        index = (int)p.Y * 8 + (int)p.X;
                        Board[index].BorderColor = new SolidColorBrush(Colors.Red);
                    }

                }
            }
        }

        protected void AssignCellBlackBorder()
        {
            foreach(var cell in Board)
            {
                cell.BorderColor = new SolidColorBrush(Colors.Black);
            }
        }

        public List<BasePiece> ToBasePieceList()
        {
            return Board.Select(b => b.Piece).ToList();
        }
        public bool IsAttacked(Point p)
        {
            int index = (int)p.Y * 8 + (int)p.X;
            foreach(ChessCell square in Board)
            {
                if(square.Piece != null && square.Piece.IsWhite != CurrentPiece.IsWhite)
                {
                    foreach(Point attackedSquare in square.Piece.GetPossibleMoves(ToBasePieceList()))
                    {
                        if (attackedSquare.Equals(p))
                        {
                             return true;
                        }                                                
                    }
                }
            }
            return false;
        }
            
    }
}
