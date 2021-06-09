using System;
using System.Collections.Generic;
using System.Text;
using ChessGame;

namespace ChessEngineLogic
{
    /// <summary>
    /// Chess Game event arguments that will be sent to event handlers.
    /// </summary>
    public class GameEventArgs : FigureMoveEventArgs
    {
        public GameEventArgs(FigureMoveEventArgs e)
        {
            MovedFigureName = e.MovedFigureName;
            BeatenFigureName = e.BeatenFigureName;
            BeatenFigureCell = e.BeatenFigureCell;
            CellFrom = e.CellFrom;
            CellTo = e.CellTo;
            CastelingCellFrom = e.CastelingCellFrom;
            CastelingCellTo = e.CastelingCellTo;
            GameStatus = e.GameStatus;
            CurrentPlayer = e.CurrentPlayer;
            WinnerPlayer = e.WinnerPlayer;
        }
    }
}
