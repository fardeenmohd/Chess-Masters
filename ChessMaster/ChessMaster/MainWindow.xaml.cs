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
using System.Windows.Navigation;
using System.Windows.Shapes;
using ChessMaster.Pieces;
using ChessMaster.AI;
using ChessMaster.ViewModel;
using System.ComponentModel;
using ChessMaster.Dialogs;


namespace ChessMaster
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// The GUI of the ChessMaster
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    { 
        #region BindableProperties
        private List<ChessCell> _cells;

        public List<int> Numbers { get; set; }
        public List<char> Letters { get; set; }
        public List<ChessCell> Cells
        {
            get
            {
                return _cells;
            }
            set
            {
                _cells = value;
                OnPropertyChanged(nameof(Cells));
            }
        }
        #endregion

        public ChessBoard ChessBoard;

        public event PropertyChangedEventHandler PropertyChanged;

        #region Commands
        private RelayCommand _exitCommand;
        private RelayCommand _newGameCommand;
        private RelayCommand _cellCommand;

        public RelayCommand ExitCommand => _exitCommand ?? (_exitCommand = new RelayCommand(ExecuteExitCommand));
        public RelayCommand NewGameCommand => _newGameCommand ?? (_newGameCommand = new RelayCommand(ExecuteNewGameCommand, CanExecuteNewGameCommand));
        public RelayCommand CellCommand => _cellCommand ?? (_cellCommand = new RelayCommand(ExecuteCellCommand));
        #endregion

        public MainWindow()
        {
            InitializeLists();
            InitializeComponent();
            DataContext = this;
        }

        private void InitializeLists()
        {
            Numbers = Enumerable.Range(1, 8).Reverse().ToList();
            Letters = new List<char> { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h' };
            ChessBoard = new ChessBoard();
            Cells = ChessBoard.Board;          
        }

        public bool CanExecuteNewGameCommand(object obj)
        {
            return false;
        }

        public void ExecuteExitCommand(object obj)
        {
            Close();
        }

        public void ExecuteNewGameCommand(object obj)
        {
            //TODO
        }

        public void ExecuteCellCommand(object obj)
        {
            //MessageBox.Show("Current Position: " + (Point)obj);
            if (obj != null)
            {
                Point currentPosition = (Point)obj;
                int index = (int)currentPosition.Y * 8 + (int)currentPosition.X;
                if (Cells[index].BorderColor.Color == Colors.Red)
                {
                    ChessBoard.MakeMove(index);
                }
                else if (Cells[index].Piece != null && Cells[index].Piece.IsWhite)
                {
                    ChessBoard.ShowPossibleMoves(index);
                }
                Cells = new List<ChessCell>(ChessBoard.Board);
            }
        }

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
