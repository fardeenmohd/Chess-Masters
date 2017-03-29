using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.IO;
using ChessMaster.ViewModel;
using ChessMaster.Pieces;

namespace ChessMaster.Dialogs
{
    /// <summary>
    /// Interaction logic for PromotionWindow.xaml
    /// </summary>
    public partial class PromotionWindow : Window
    {
        #region BindableProperties
        public string RookImage { get; set; }
        public string KnightImage { get; set; }
        public string BishopImage { get; set; }
        public string QueenImage { get; set; }
        #endregion

        private bool _isWhite;

        public BasePiece SelectedPiece;

        #region Commands
        private RelayCommand _rookCommand;
        private RelayCommand _knightCommand;
        private RelayCommand _bishopCommand;
        private RelayCommand _queenCommand;

        public RelayCommand RookCommand => _rookCommand ?? (_rookCommand = new RelayCommand(ExecuteRookCommand));
        public RelayCommand KnightCommand => _knightCommand ?? (_knightCommand = new RelayCommand(ExecuteKnightCommand));
        public RelayCommand BishopCommand => _bishopCommand ?? (_bishopCommand = new RelayCommand(ExecuteBishopCommand));
        public RelayCommand QueenCommand => _queenCommand ?? (_queenCommand = new RelayCommand(ExecuteQueenCommand));
        #endregion

        public PromotionWindow()
        {
            RookImage = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../Images/white_rook.png");
            KnightImage = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../Images/white_knight.png");
            BishopImage = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../Images/white_bishop.png");
            QueenImage = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../Images/white_queen.png");
            InitializeComponent();
            Owner = Application.Current.MainWindow;
            DataContext = this;
        }

        public PromotionWindow(bool isWhite) : this()
        {
            _isWhite = isWhite;
        }

        public void ExecuteRookCommand(object obj)
        {
            SelectedPiece = new Rook(0, 0, _isWhite);
            DialogResult = true;
        }

        public void ExecuteKnightCommand(object obj)
        {
            SelectedPiece = new Knight(0, 0, _isWhite);
            DialogResult = true;
        }

        public void ExecuteQueenCommand(object obj)
        {
            SelectedPiece = new Queen(0, 0, _isWhite);
            DialogResult = true;
        }

        public void ExecuteBishopCommand(object obj)
        {
            SelectedPiece = new Bishop(0, 0, _isWhite);
            DialogResult = true;
        }
    }
}
