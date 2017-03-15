﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ChessMaster.Pieces
{
    public class Pawn : BasePiece
    { 
        public Pawn(int x, int y, bool isWhite = true) : base(x, y, isWhite)
        {
            if (IsWhite)
            {
                PieceImage = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../Images/white_pawn.png");
            }
            else
            {
                PieceImage = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../Images/black_pawn.png");
            }
        }
        public override List<Point> GetPossibleMoves(List<BasePiece> board)
        {
            return GetPawnMoves(board, IsFirstMove, IsWhite);
        }
    }
}
