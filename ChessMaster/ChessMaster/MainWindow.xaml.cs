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
            Cells = new List<ChessCell>();
            for (int y = 0; y < 8; y++)
            {
                for (int x = 0; x < 8; x++)
                {
                    ChessCell c = new ChessCell
                    {
                        Background = new SolidColorBrush((x + y) % 2 == 1 ? Colors.Black : Colors.WhiteSmoke),
                        BorderColor = new SolidColorBrush(Colors.Black),
                        //Piece = new BasePiece(x, y),
                        Position = new Point(x, y)
                    };
                    Cells.Add(c);
                }
            }
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
            MessageBox.Show("Current Position: " + (Point)obj);
            //if (obj != null)
            //{
            //    Point currentPosition = (Point)obj;
            //    int index = (int)currentPosition.Y * 8 + (int)currentPosition.X;
            //    if (Cells[index].BorderColor.Color == Colors.Red)
            //    {
            //        MessageBox.Show("Move Piece");
            //    }
            //    else if (Cells[index].Piece != null)
            //    {
            //        List<Point> possiblemoves = Cells[index].Piece.GetPossibleMoves();
            //        foreach (Point p in possiblemoves)
            //        {
            //            index = (int)p.Y * 8 + (int)p.X;
            //            Cells[index].BorderColor = new SolidColorBrush(Colors.Red);
            //        }
            //    }
            //    Cells = new List<ChessCell>(Cells);
            //}
        }

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
