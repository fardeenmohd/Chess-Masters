using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using ChessMaster.Pieces;
using System.Windows;

namespace ChessMaster.ViewModel
{
    /// <summary>
    /// Represents a cell on the chess board
    /// </summary>
    public class ChessCell
    {
        public Point Position { get; set; }

        public SolidColorBrush Background { get; set; }

        public SolidColorBrush BorderColor { get; set; }

        public BasePiece Piece { get; set; }
      
    }
}
