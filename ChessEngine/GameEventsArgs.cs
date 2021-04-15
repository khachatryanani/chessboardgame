using System;
using System.Collections.Generic;
using System.Text;

namespace ChessEngineLogic
{
    public class GameEventArgs : EventArgs
    {
        public string MovedFigure { get; set; }
        public string CellFrom { get; set; }
        public string CellTo { get; set; }

        public int GameStatus { get; set; }

        public GameEventArgs(string figure, string cellFrom, string cellTo, int status)
        {
            this.MovedFigure = figure;
            this.CellFrom = cellFrom;
            this.CellTo = cellTo;
            this.GameStatus = status;
        }
    }
}
