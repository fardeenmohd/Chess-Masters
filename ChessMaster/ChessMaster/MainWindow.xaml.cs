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

        public RelayCommand ExitCommand => _exitCommand ?? (_exitCommand = new RelayCommand(ExecuteExitCommand));
        public RelayCommand NewGameCommand => _newGameCommand ?? (_newGameCommand = new RelayCommand(ExecuteNewGameCommand, CanExecuteNewGameCommand));
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
                        Background = new SolidColorBrush((x + y) % 2 == 1 ? Colors.Black : Colors.WhiteSmoke)
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

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
