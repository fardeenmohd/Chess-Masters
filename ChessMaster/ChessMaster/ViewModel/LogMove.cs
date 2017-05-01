using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessMaster.ViewModel
{
    public class LogMove
    { 
        public int MoveNumber { get; set; }

        public string WhiteMove { get; set; }

        public string BlackMove { get; set; }

        public LogMove()
        {
            WhiteMove = BlackMove ="";
        }
    }
}
