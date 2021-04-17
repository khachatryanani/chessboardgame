using System;
using System.Collections.Generic;
using System.Text;

namespace ChessEngineLogic
{
    /// <summary>
    /// Chess Game event arguments that will be sent to event handlers.
    /// </summary>
    public class GameEventArgs : EventArgs
    {
        public string MovedFigure { get; set; }
        public string CellTo { get; set; }
        public int GameStatus { get; set; }
        public string CurrentPlayer { get; set; }
        public string WinnerPlayer { get; set; }

        public GameEventArgs() { }
    }
}
