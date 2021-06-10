using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessGame
{
    public class FigureMoveEventArgs
    {
        public string MovedFigureName { get; set; }
        public string BeatenFigureName { get; set; }
        public string BeatenFigureCell { get; set; }
        public string CellFrom { get; set; }
        public string CellTo { get; set; }
        public string CastelingCellFrom { get; set; }
        public string CastelingCellTo { get; set; }
        public bool IsPawnUpgrade { get; set; }

        public int GameStatus { get; set; }
        public bool CurrentPlayer { get; set; }
        public bool? WinnerPlayer { get; set; }

        public FigureMoveEventArgs() { }
    }
}
