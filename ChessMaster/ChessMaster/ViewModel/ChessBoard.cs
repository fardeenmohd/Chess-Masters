﻿using System;
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

        public List<PiecePossibleMove> CurrentPiecePossibleMoves;

        public List<ChessCell> Board;

        public List<Move> HistoryOfMoves;

        public List<LogMove> Logs;

        public int LastLogPosition;

        public bool GameFinished = false;

        public Dictionary<bool, bool> hasCastled;
        public Move LastMadeMove { get; set; }
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
            CurrentPiecePossibleMoves = new List<PiecePossibleMove>();
            hasCastled = new Dictionary<bool, bool>();
            hasCastled.Add(true, false);
            hasCastled.Add(false, false);
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
            Logs = new List<LogMove>();
            LastLogPosition = -1;
        }

        /// <summary>
        /// Makes a move on chess board
        /// </summary>
        /// <returns></returns>
        public void MakeMove(int index, bool useLog)
        {
            if (CurrentPiece != null)
            {
                AssignCellBlackBorder();
                PiecePossibleMove move = CurrentPiecePossibleMoves.Where(m => (int)(m.MoveToPosition.Y * 8 + m.MoveToPosition.X) == index).Single();
                BasePiece promotionPiece = null;
                if (CurrentPiece is Pawn && (Board[index].Position.Y == 0 || Board[index].Position.Y == 7))
                {
                    PromotionWindow dialog = new PromotionWindow(CurrentPiece.IsWhite);
                    if (dialog.ShowDialog() == true)
                    {
                        promotionPiece = dialog.SelectedPiece;
                    }
                }
                Move madeMove = new Move(move, CurrentPiece, Board[index].Piece, promotionPiece);
                HistoryOfMoves.Add(madeMove);
                madeMove.MakeMove(ref Board);
                LastMadeMove = madeMove.CopyMove();
                if(madeMove.IsCastlingMove)
                {
                    hasCastled[madeMove.IsWhiteMove] = true;
                }
                bool isWhite = CurrentPiece.IsWhite;
                CurrentPiece = null;
                bool isGameOver = IsGameOver(!isWhite);
                bool isChecked = IsAttacked((Point)FindKingLocation(!isWhite), !isWhite);
                if (isGameOver)
                {
                    MessageBox.Show("Game over! " + (isWhite ? "White" : "Black") + " wins!");
                    GameFinished = true;
                }
                if (useLog)
                    WriteLogMove(isGameOver, isChecked);
            }
        }

        public void MakeSpecificMove(Move move, bool isWhite, bool useLog)
        {
            HistoryOfMoves.Add(move);
            move.MakeMove(ref Board);
            LastMadeMove = move.CopyMove();
            if (move.IsCastlingMove)
            {
                hasCastled[move.IsWhiteMove] = true;
            }
            bool isGameOver = IsGameOver(!isWhite);
            bool isChecked = IsAttacked((Point)FindKingLocation(!isWhite), !isWhite);
            if (isGameOver)
            {
                MessageBox.Show("Game over! " + (isWhite ? "White" : "Black") + " wins!");
                GameFinished = true;
            }
            if (useLog)
                WriteLogMove(isGameOver, isChecked);
        }

        public Move MakeFakeMove(PiecePossibleMove move, BasePiece actualPiece)
        {
            int index = (int)(move.MoveToPosition.Y * 8 + move.MoveToPosition.X);
            Move madeMove;
            //TODO if piece is a pawn then promote to queen if it's at row 0 or row 7
            if (actualPiece is Pawn && (move.MoveToPosition.Y == 0 || move.MoveToPosition.Y == 7))
            {
                madeMove = new Move(move, actualPiece, Board[index].Piece, new Queen((int)move.MoveToPosition.X, (int)move.MoveToPosition.Y, actualPiece.IsWhite));
            }
            else
            {
                madeMove = new Move(move, actualPiece, Board[index].Piece);
            }
            HistoryOfMoves.Add(madeMove);
            madeMove.MakeMove(ref Board);
            if (madeMove.IsCastlingMove)
            {
                hasCastled[madeMove.IsWhiteMove] = true;
            }
            LastMadeMove = madeMove.CopyMove();
            return madeMove;
        }

        public void UnMakeLastMove(bool useLog)
        {
            AssignCellBlackBorder();
            if(HistoryOfMoves[HistoryOfMoves.Count - 1].IsCastlingMove)
            {
                hasCastled[HistoryOfMoves[HistoryOfMoves.Count - 1].IsWhiteMove] = false;
            }
            HistoryOfMoves[HistoryOfMoves.Count - 1].UnMakeMove(ref Board);
            HistoryOfMoves.RemoveAt(HistoryOfMoves.Count - 1);
            if(useLog)
            {
                if (Logs[LastLogPosition].BlackMove == "")
                    Logs.RemoveAt(LastLogPosition--);
                else
                    Logs[LastLogPosition].BlackMove = "";
            }
        }

        public void ShowPossibleMoves(int index)
        {
            if (CurrentPiece == null)
            {
                CurrentPiece = Board[index].Piece;
                CurrentPiecePossibleMoves = GetOnlyLegalMoves(CurrentPiece);
                foreach (Point p in CurrentPiecePossibleMoves.Select(move => move.MoveToPosition))
                {
                    int moveIndex = (int)p.Y * 8 + (int)p.X;
                    Board[moveIndex].BorderColor = new SolidColorBrush(Colors.Red);
                }
            }
            else if (CurrentPiece != null && !CurrentPiece.Equals(Board[index].Piece))
            {
                AssignCellBlackBorder();
                CurrentPiece = Board[index].Piece;
                CurrentPiecePossibleMoves = GetOnlyLegalMoves(CurrentPiece);
                foreach (Point p in CurrentPiecePossibleMoves.Select(move => move.MoveToPosition))
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
        private List<PiecePossibleMove> GetOnlyLegalMoves(BasePiece pieceToBeChecked)
        {
            List<PiecePossibleMove> legalMoves = new List<PiecePossibleMove>();
            List<PiecePossibleMove> possibleMoves = pieceToBeChecked.GetPossibleMoves(ToBasePieceList());
            foreach (PiecePossibleMove move in possibleMoves)
            {
                MakeFakeMove(move, pieceToBeChecked);
                if (IsValidMove(move, pieceToBeChecked.IsWhite))
                {
                    legalMoves.Add(move);
                }
                UnMakeLastMove(false);
            }
            return legalMoves;
        }

        public List<PiecePossibleMove> GetEveryLegalMove(bool isWhite)
        {
            return this.Board.Where(bp => bp.Piece != null && bp.Piece.IsWhite == isWhite)
                .SelectMany(p => GetOnlyLegalMoves(p.Piece)).Where(ppm => ppm != null)
                .Select(ppm => new PiecePossibleMove(new Point(ppm.MoveToPosition.X, ppm.MoveToPosition.Y), new Point(ppm.FromPosition.X, ppm.FromPosition.Y))
                {
                    IsCastlingMove = ppm.IsCastlingMove,
                    CastlingRook = ppm.CastlingRook,
                    RookPosition = new Point(ppm.RookPosition.X, ppm.RookPosition.Y)
                }).ToList();
        }

        public bool IsValidMove(PiecePossibleMove move, bool isWhite)
        {           
            Point? location = FindKingLocation(isWhite);
            if (location.HasValue)
            {
                Point kingLocation = location.Value;
                if (move.IsCastlingMove)
                {
                    if (move.RookPosition.X == 0)
                    {
                        for (int x = 4; x > 1; x--)
                        {
                            if (IsAttacked(new Point(x, kingLocation.Y), isWhite))
                                return false;
                        }
                        return true;
                    }
                    else
                    {
                        for (int x = 4; x < 7; x++)
                        {
                            if (IsAttacked(new Point(x, kingLocation.Y), isWhite))
                                return false;
                        }
                        return true;
                    }
                }
                if (IsAttacked(kingLocation, isWhite))
                    return false;
                else return true;
            }
            else
                return true;
        }

        public Point? FindKingLocation(bool isWhite)
        {
            return Board.Find(p => p.Piece is King && p.Piece.IsWhite == isWhite)?.Position;
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
        public int GetNumberOfAttackedPieces(BasePiece attackingPiece)
        {
            List<PiecePossibleMove> moves = GetOnlyLegalMoves(attackingPiece);
            int count = 0;
            foreach(PiecePossibleMove move in moves)
            {
                int index = (int)move.MoveToPosition.Y * 8 + (int)move.MoveToPosition.X;
                if(Board[index].Piece != null)
                {
                    if(Board[index].Piece.IsWhite != attackingPiece.IsWhite)
                    {
                        count++;
                    }
                }
            }
            return count;
        }
        public int GetTotalNumOfAttackedPieces(bool isWhite)
        {
            int count = 0;
            foreach(BasePiece bp in ToBasePieceList())
            {
                if(bp != null && bp.IsWhite == isWhite)
                {
                    count += GetNumberOfAttackedPieces(bp);
                }
            }
            return count;
        }
        public int GetTotalNumberOfMoves(bool isWhite)
        {
            int count = 0;
            foreach (BasePiece bp in ToBasePieceList())
            {
                if (bp != null && bp.IsWhite == isWhite)
                {
                    count += GetOnlyLegalMoves(bp).Count;
                }
            }
            return count;
        }
        public bool IsAttacked(Point p, bool isWhite)
        {
            int index = (int)p.Y * 8 + (int)p.X;
            var cellsUnderAttack = Board.Where(cell => cell.Piece != null && cell.Piece.IsWhite != isWhite)
                                .SelectMany(cell => cell.Piece.GetPossibleMoves(ToBasePieceList()))
                                .Where(cell => !cell.IsCastlingMove).Select(cell => cell.MoveToPosition);
            return cellsUnderAttack.Contains(p);
        }

        private void WriteLogMove(bool isGameOver, bool isChecked)
        {
            bool isBlackLog = false;
            if (LastLogPosition!=-1 && Logs[LastLogPosition].BlackMove == "")
            {
                Logs[LastLogPosition].BlackMove = HistoryOfMoves[HistoryOfMoves.Count - 1].ToString();
                isBlackLog = true;
            }
            else
            {
                Logs.Add(new LogMove()
                {
                    MoveNumber = ++LastLogPosition + 1,
                    WhiteMove = HistoryOfMoves[HistoryOfMoves.Count - 1].ToString()
                });
                isBlackLog = false;
            }
            if(isGameOver)
            {
                if (isBlackLog)
                    Logs[LastLogPosition].BlackMove += "++";
                else
                    Logs[LastLogPosition].WhiteMove += "++";
            }
            else if(isChecked)
            {
                if (isBlackLog)
                    Logs[LastLogPosition].BlackMove += "+";
                else
                    Logs[LastLogPosition].WhiteMove += "+";
            }
        }
    }
}