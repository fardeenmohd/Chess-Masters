﻿using System;
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
using ChessMaster.ViewModel;
using System.ComponentModel;
using ChessMaster.Dialogs;
using System.Windows.Threading;


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
        private TimeSpan _blackSideTime;
        private TimeSpan _whiteSideTime;
        private SolidColorBrush _blackSideBorderColor;
        private SolidColorBrush _whiteSideBorderColor;
        private List<LogMove> _logs;

        public List<LogMove> Logs
        {
            get { return _logs; }
            set
            {
                _logs = value;
                OnPropertyChanged(nameof(Logs));
            }
        }

        public SolidColorBrush BlackSideBorderColor
        {
            get { return _blackSideBorderColor; }
            set
            {
                _blackSideBorderColor = value;
                OnPropertyChanged(nameof(BlackSideBorderColor));
            }
        }

        public SolidColorBrush WhiteSideBorderColor
        {
            get { return _whiteSideBorderColor; }
            set
            {
                _whiteSideBorderColor = value;
                OnPropertyChanged(nameof(WhiteSideBorderColor));
            }
        }

        public TimeSpan BlackSideTime
        {
            get { return _blackSideTime; }
            set
            {
                _blackSideTime = value;
                OnPropertyChanged(nameof(BlackSideTime));
            }
        }
        public TimeSpan WhiteSideTime
        {
            get { return _whiteSideTime; }
            set
            {
                _whiteSideTime = value;
                OnPropertyChanged(nameof(WhiteSideTime));
            }
        }
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

        private DispatcherTimer _commonTimer { get; set; }

        private bool _isWhiteMove;

        private bool _versusAI;

        public ChessBoard ChessBoard;

        public event PropertyChangedEventHandler PropertyChanged;

        #region Commands
        private RelayCommand _exitCommand;
        private RelayCommand _newGameAgainstAICommand;
        private RelayCommand _newGameAgainstHumanCommand;
        private RelayCommand _cellCommand;
        private RelayCommand _unmakeMoveCommand;

        public RelayCommand ExitCommand => _exitCommand ?? (_exitCommand = new RelayCommand(ExecuteExitCommand));
        public RelayCommand NewGameAgainstAICommand => _newGameAgainstAICommand ?? (_newGameAgainstAICommand = new RelayCommand(ExecuteNewGameAgainstAICommand));
        public RelayCommand NewGameAgainstHumanCommand => _newGameAgainstHumanCommand ?? (_newGameAgainstHumanCommand = new RelayCommand(ExecuteNewGameAgainstHumanCommand));
        public RelayCommand CellCommand => _cellCommand ?? (_cellCommand = new RelayCommand(ExecuteCellCommand));
        public RelayCommand UnmakeMoveCommand => _unmakeMoveCommand ?? (_unmakeMoveCommand = new RelayCommand(ExecuteUnmakeMoveCommand, CanExecuteUnmakeMoveCommand));

        private Evaluator Evaluator = new Evaluator();
        #endregion

        public MainWindow()
        {
            InitializeLists();
            InitializeTimers();
            InitializeComponent();
            DataContext = this;
            _versusAI = false;
        }

        private void InitializeLists()
        {
            Numbers = Enumerable.Range(1, 8).Reverse().ToList();
            Letters = new List<char> { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h' };
            ChessBoard = new ChessBoard();
            Logs = new List<LogMove>();
            Cells = ChessBoard.Board;
            _isWhiteMove = true;
            ChangeTimersBorderColor();      
        }

        private void InitializeTimers()
        {
            _commonTimer = new DispatcherTimer();
            _commonTimer.Interval = new TimeSpan(0, 0, 1);
            _commonTimer.Tick += new EventHandler(Each_Tick);
            _commonTimer.Start();
        }

        public void ExecuteExitCommand(object obj)
        {
            Close();
        }

        public void ExecuteNewGameAgainstHumanCommand(object obj)
        {
            _versusAI = false;
            ChessBoard = new ChessBoard();
            Logs = new List<LogMove>();
            Cells = ChessBoard.Board;
            WhiteSideTime = new TimeSpan();
            BlackSideTime = new TimeSpan();
            _isWhiteMove = true;
            ChangeTimersBorderColor();
        }

        public void ExecuteNewGameAgainstAICommand(object obj)
        {
            _versusAI = true;
            ChessBoard = new ChessBoard();
            Cells = ChessBoard.Board;
            WhiteSideTime = new TimeSpan();
            BlackSideTime = new TimeSpan();
            _isWhiteMove = true;
            ChangeTimersBorderColor();
            //MessageBox.Show("You are not ready to face with superior machine!", "Human vs AI game", MessageBoxButton.OK, MessageBoxImage.Stop);
        }

        public void ExecuteCellCommand(object obj)
        {
            if (obj != null && !ChessBoard.GameFinished)
            {
                Point currentPosition = (Point)obj;
                int index = (int)currentPosition.Y * 8 + (int)currentPosition.X;
                if (Cells[index].BorderColor.Color == Colors.Red)
                {
                    ChessBoard.MakeMove(index, true);               
                    _isWhiteMove = !_isWhiteMove;
                    ChangeTimersBorderColor();
                    Cells = new List<ChessCell>(ChessBoard.Board);
                    Logs = new List<LogMove>(ChessBoard.Logs);
                    if (_versusAI)
                    {
                        DispatcherFrame frame = new DispatcherFrame();
                        Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Render, new DispatcherOperationCallback(delegate (object parameter)
                        {
                            frame.Continue = false;
                            return null;
                        }), null);

                        Dispatcher.PushFrame(frame);
                        //EDIT:
                        Application.Current.Dispatcher.Invoke(DispatcherPriority.Background,
                                                      new Action(delegate { }));
                        AIMove();
                    }
                }
                else if (Cells[index].Piece != null && Cells[index].Piece.IsWhite == _isWhiteMove)
                {
                    ChessBoard.ShowPossibleMoves(index);
                }
                Cells = new List<ChessCell>(ChessBoard.Board);
                Logs = new List<LogMove>(ChessBoard.Logs);
            }
        }

        public void AIMove()
        {
            DateTime start = DateTime.Now;
            Evaluator.BestMove = null;
            double value = Evaluator.Max(ChessBoard, _isWhiteMove, 0, 0, 0, 0);
            //MessageBox.Show("Evaluation for " + (_isWhiteMove ? "white: " + value : "black: " + value)
            //                                  + "\n Best Move: " + Evaluator.BestMove.ToString());
            ChessBoard.MakeSpecificMove(Evaluator.BestMove.CopyMove(), _isWhiteMove, true);
            _isWhiteMove = !_isWhiteMove;
            ChangeTimersBorderColor();
            BlackSideTime = BlackSideTime.Add(new TimeSpan(0, 0, (DateTime.Now - start).Seconds));
        }

        public void ExecuteUnmakeMoveCommand(object obj)
        {
            if (!_versusAI)
            {
                ChessBoard.UnMakeLastMove(true);
                _isWhiteMove = !_isWhiteMove;
                ChangeTimersBorderColor();
            }
            else
            {
                ChessBoard.UnMakeLastMove(true);
                ChessBoard.UnMakeLastMove(true);
            }
            Cells = new List<ChessCell>(ChessBoard.Board);
            Logs = new List<LogMove>(ChessBoard.Logs); // This updates the GUI
        }

        public bool CanExecuteUnmakeMoveCommand(object obj)
        {
            return (ChessBoard.HistoryOfMoves.Count > 0 && !ChessBoard.GameFinished && !_versusAI) 
                || (ChessBoard.HistoryOfMoves.Count % 2 == 0  && ChessBoard.HistoryOfMoves.Count > 1 && !ChessBoard.GameFinished);
        }

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Each_Tick(object o, EventArgs sender)
        {
            if (!ChessBoard.GameFinished)
            {
                if (_isWhiteMove)
                    WhiteSideTime = WhiteSideTime.Add(new TimeSpan(0, 0, 1));
                else
                    BlackSideTime = BlackSideTime.Add(new TimeSpan(0, 0, 1));
            }
        }

        private void ChangeTimersBorderColor()
        {
            if (_isWhiteMove)
            {
                WhiteSideBorderColor = new SolidColorBrush(Colors.Green);
                BlackSideBorderColor = new SolidColorBrush(Colors.White);
            }
            else
            {
                WhiteSideBorderColor = new SolidColorBrush(Colors.White);
                BlackSideBorderColor = new SolidColorBrush(Colors.Green);
            }
        }
    }
}
