using System;
using ChessBoard.BoardAttributes;
using ChessBoard.Figures;
using ChessBoard.Extensions;
using System.Collections.Generic;

namespace ChessBoard
{
    /// <summary>
    /// ChessBoard class is responsible vor maintaining and displaying all the information on available figures on the board 
    /// their locations and verifies their moves.
    /// </summary>
    public static class ChessBoardManager
    {
        // Container for all the available figures on the board
        private static readonly Board _board = new Board();

        /// <summary>
        /// ChessManager can add figures to the board
        /// </summary>
        /// <param name="figure">Figure object to be added to the board</param>
        public static void AddFigure(Figure figure)
        {
            // String representation of the Cell object is the Key in board container
            string cell = figure.CurrentCell.ToString();
            _board[cell] = figure;
        }

        /// <summary>
        /// Chessboard interates throught its container of figures and determines if the given cell is free or is already occupied.
        /// </summary>
        /// <param name="cellTomove">Cell to determine the availability</param>
        /// <returns>True if the given cell is free to move to, false if its already occupied</returns>
        public static bool IsFreeCell(Cell cellTomove)
        {
            foreach (var item in _board)
            {
                if (item.CurrentCell == cellTomove)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Checks if the given cell might be under the Check of opposite figures
        /// </summary>
        /// <param name="cellToMove">Cell to verify for Chess Check</param>
        /// <param name="colorOfPlayer">Color of current player</param>
        /// <returns>True if the given cell falls under Check of opposite figures, False if it does not</returns>
        public static bool IsUnderCheckCell(Cell cellToMove, Color colorOfPlayer, out Figure influencingFigure)
        {
            foreach (var item in _board)
            {
                if (item.InfluencedCells.ContainsCell(cellToMove) && item.Color != colorOfPlayer && IsPossibleToMove(item, cellToMove))
                {
                    King king = GetTheKing(colorOfPlayer);
                    Cell kingCell = king.CurrentCell;
                    Cell placeHolder = new Cell();
                    king.PhantomMove(placeHolder);
                    if (IsPossibleToMove(item, cellToMove))
                    {
                        king.PhantomMove(kingCell);
                        influencingFigure = item;
                        return true;
                    }
                    king.PhantomMove(kingCell);
                }
            }
            influencingFigure = null;
            return false;
        }

        /// <summary>
        /// Checks if the current cell falls under an influence of any of opposite figure
        /// </summary>
        /// <param name="cellToCheck">Cell to check if there is an influence on</param>
        /// <param name="colorOfPlayer">Color of the current player</param>
        /// <returns>True if the cell is in the influeneced cells of the opposite player, False if it does not</returns>
        public static bool IsInfluencedCell(Cell cellToCheck, Color colorOfPlayer)
        {
            foreach (var item in _board)
            {
                if (item.InfluencedCells.ContainsCell(cellToCheck) && item.Color != colorOfPlayer)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Checks if the given figure can move to the given cell: if it is free and if the path to reach it is also free
        /// </summary>
        /// <param name="figure">Figure to move</param>
        /// <param name="cellToMove">Cell to move the figure to</param>
        /// <returns>True is it is possible to move the given figure to the given cell, False if not</returns>
        public static bool IsPossibleToMove(Figure figure, Cell cellToMove)
        {

            if (figure is Pawn) 
            {

                if (GetFigureByCell(cellToMove) != null) 
                {
                    return true;
                }
                return false;
               
            }

            if (GetFigureByCell(cellToMove) != null && GetFigureByCell(cellToMove).Color == figure.Color)
            {
                return false;
            }

           
            char letterMoveFrom = figure.CurrentCell.Letter;
            char letterMoveTo = cellToMove.Letter;

            int numberMoveFrom = figure.CurrentCell.Number;
            int numberMoveTo = cellToMove.Number;

            // Moving vertically
            if (letterMoveFrom == letterMoveTo)
            {
                Cell cell;
                if (numberMoveFrom < numberMoveTo)
                {
                    // Checks if the path from cell to move from to cell to move to is completly free
                    for (int i = numberMoveFrom + 1; i < numberMoveTo; i++)
                    {
                        cell = new Cell(figure.CurrentCell.Letter, i);

                        if (!IsFreeCell(cell))
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    for (int i = numberMoveFrom - 1; i > numberMoveTo; i--)
                    {
                        cell = new Cell(figure.CurrentCell.Letter, i);
                        if (!IsFreeCell(cell))
                        {
                            return false;
                        }
                    }
                }
                return true;
            }
            // Moving horizonally
            else if (numberMoveFrom == numberMoveTo)
            {
                Cell cell;
                if (letterMoveFrom < letterMoveTo)
                {
                    // Checks if the path from cell to move from to cell to move to is completle free
                    for (int i = letterMoveFrom + 1; i < letterMoveTo; i++)
                    {
                        cell = new Cell((char)i, numberMoveFrom);
                        if (!IsFreeCell(cell))
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    for (int i = letterMoveFrom - 1; i > letterMoveTo; i--)
                    {
                        cell = new Cell((char)i, numberMoveFrom);
                        if (!IsFreeCell(cell))
                        {
                            return false;
                        }
                    }
                }
                return true;
            }
            //Moving by diaganal
            else
            {
                switch (numberMoveFrom > numberMoveTo)
                {
                    //Case1: up-right
                    //Case2: up-left
                    case true when letterMoveFrom > letterMoveTo:
                        {
                            // Checks if the path from cell to move from to cell to move to is completely free
                            for (int i = numberMoveFrom - 1, j = letterMoveFrom - 1; i > numberMoveTo && j > letterMoveTo; i--, j--)
                            {
                                var cell = new Cell((char)j, i);
                                if (!IsFreeCell(cell))
                                {
                                    return false;
                                }
                            }
                            return true;
                        }
                    //Case3: down-right
                    case true when letterMoveFrom < letterMoveTo:
                        {
                            // Checks if the path from cell to move from to cell to move to is completely free
                            for (int i = numberMoveFrom - 1, j = letterMoveFrom + 1; i > numberMoveTo && j < letterMoveTo; i--, j++)
                            {
                                var cell = new Cell((char)j, i);
                                if (!IsFreeCell(cell))
                                {
                                    return false;
                                }
                            }
                            return true;
                        }
                    default:
                        {
                            if (numberMoveFrom < numberMoveTo && letterMoveFrom > letterMoveTo)
                            {
                                // Checks if the path from cell to move from to cell to move to is completely free
                                for (int i = numberMoveFrom + 1, j = letterMoveFrom - 1; i < numberMoveTo && j > letterMoveTo; i++, j--)
                                {
                                    var cell = new Cell((char)j, i);
                                    if (!IsFreeCell(cell))
                                    {
                                        return false;
                                    }
                                }
                                return true;
                            }
                            //Case4: down-left
                            else
                            {
                                // Checks if the path from cell to move from to cell to move to is completely free
                                for (int i = numberMoveFrom + 1, j = letterMoveFrom + 1; i < numberMoveTo && j < letterMoveTo; i++, j++)
                                {
                                    var cell = new Cell((char)j, i);
                                    if (!IsFreeCell(cell))
                                    {
                                        return false;
                                    }
                                }
                                return true;
                            }
                        }
                }
                ////Case1: up-right
                //if (numberMoveFrom > numberMoveTo && letterMoveFrom > letterMoveTo)
                //{
                //    Cell cell;
                //    // Checks if the path from cell to move from to cell to move to is completle free
                //    for (int i = numberMoveFrom - 1, j = letterMoveFrom - 1; i > numberMoveTo && j > letterMoveTo; i--, j--)
                //    {
                //        cell = new Cell((char)j, i);
                //        if (!IsFreeCell(cell))
                //        {
                //            return false;
                //        }
                //    }
                //    return true;
                //}
                ////Case2: up-left
                //else if (numberMoveFrom > numberMoveTo && letterMoveFrom < letterMoveTo)
                //{
                //    Cell cell;
                //    // Checks if the path from cell to move from to cell to move to is completle free
                //    for (int i = numberMoveFrom - 1, j = letterMoveFrom + 1; i > numberMoveTo && j < letterMoveTo; i--, j++)
                //    {
                //        cell = new Cell((char)j, i);
                //        if (!IsFreeCell(cell))
                //        {
                //            return false;
                //        }
                //    }
                //    return true;
                //}
                ////Case3: down-right
                //else if (numberMoveFrom < numberMoveTo && letterMoveFrom > letterMoveTo)
                //{
                //    Cell cell;
                //    // Checks if the path from cell to move from to cell to move to is completle free
                //    for (int i = numberMoveFrom + 1, j = letterMoveFrom - 1; i < numberMoveTo && j > letterMoveTo; i++, j--)
                //    {
                //        cell = new Cell((char)j, i);
                //        if (!IsFreeCell(cell))
                //        {
                //            return false;
                //        }
                //    }
                //    return true;
                //}
                ////Case4: down-left
                //else
                //{
                //    Cell cell;
                //    // Checks if the path from cell to move from to cell to move to is completle free
                //    for (int i = numberMoveFrom + 1, j = letterMoveFrom + 1; i < numberMoveTo && j < letterMoveTo; i++, j++)
                //    {
                //        cell = new Cell((char)j, i);
                //        if (!IsFreeCell(cell))
                //        {
                //            return false;
                //        }
                //    }
                //    return true;
                //}
            }
        }

        /// <summary>
        /// Get the Figure object from the figure container based on the location on the board
        /// </summary>
        /// <param name="cellOfFigure"></param>
        /// <param name="colorOfFigure"></param>
        /// <returns>The Figure object from the chess board container</returns>
        public static Figure GetFigureByCell(Cell cellOfFigure)
        {
            foreach (var item in _board)
            {
                if (item.CurrentCell == cellOfFigure)
                {
                    return item;
                }
            }

            return null;
        }

        /// <summary>
        /// Get the King object of specified color
        /// </summary>
        /// <param name="colorOfKing">Color of King object to return</param>
        /// <returns>King object if found, null if not</returns>
        public static King GetTheKing(Color colorOfKing)
        {
            foreach (var item in _board)
            {
                if (item.GetType() == typeof(King) && item.Color == colorOfKing)
                {
                    return item as King;
                }
            }

            return null;
        }

        /// <summary>
        /// Get the specific figure out of board collection
        /// </summary>
        /// <param name="typeOfFigure">Type of the figure to get</param>
        /// <param name="colorofFigure">Color of figure to get</param>
        /// <param name="order">Gives the figure of the order if there are more than one figures of a type</param>
        /// <returns>Figure object from the chess board container</returns>
        public static Figure GetFigure(Type typeOfFigure, Color colorofFigure, int order = 1)
        {
            foreach (var item in _board)
            {
                if (item.GetType() == typeOfFigure && item.Color == colorofFigure)
                {
                    if (order == 1)
                    {
                        return item;
                    }
                    order--;
                }
            }
            return null;
        }

        /// <summary>
        /// When a Move occures, updates the board with the new positions
        /// </summary>
        /// <param name="movedFrom">Cell the figure of which was moved</param>
        /// <param name="movedTo">Cell to which the figure has moved to</param>
        public static void UpdateBoard(Cell movedFrom, Cell movedTo)
        {
            string oldCell = movedFrom.ToString();
            string newCell = movedTo.ToString();

            Figure temp = _board[oldCell];
            _board.Remove(oldCell);
            _board[newCell] = temp;
        }

        public static bool IsPossibleMoveForCurrentPosition(Figure figureToMove, Cell cellTo)
        {

            return IsPossibleToMove(figureToMove, cellTo)&&

                   IsCheckDefenseCell(figureToMove, cellTo)&&
                   IsNotOpeningCheck(figureToMove, cellTo);
        }

        public static bool IsPawnInfuelncedFigureCell(Figure figureToMove, Cell cellTo) 
        {
            if (figureToMove is Pawn) 
            {
                return GetFigureByCell(cellTo) != null;
            }

            return true;
        }

        public static bool IsShortCastelingPossible(Color turn, out Cell castelingKingCell)
        {
            if (turn == Color.White)
            {
                if (_board["F1"] == null && _board["G1"] == null)
                {
                    var king = GetTheKing(turn);
                    var rook = GetFigureByCell(new Cell("H1"));

                    if (!king.HasMoved && !rook.HasMoved)
                    {
                        castelingKingCell = new Cell("G1");
                        return true;
                    }
                }
            }
            else 
            {
                if (_board["F8"] == null && _board["G8"] == null)
                {
                    var king = GetTheKing(turn);
                    var rook = GetFigureByCell(new Cell("H8"));

                    if (!king.HasMoved && !rook.HasMoved)
                    {
                        castelingKingCell = new Cell("G8");
                        return true;
                    }
                }
            }

            castelingKingCell = null;
            return false;
        }

        public static bool IsLongCastelingPossible(Color turn, out Cell castelingKingCell)
        {
            if (turn == Color.White)
            {
                if (_board["B1"] == null && _board["C1"] == null && _board["D1"] == null)
                {
                    var king = GetTheKing(turn);
                    var rook = GetFigureByCell(new Cell("A1"));

                    if (!king.HasMoved && !rook.HasMoved)
                    {
                        castelingKingCell = new Cell("C1");
                        return true;
                    }
                }
            }
            else
            {
                if (_board["B8"] == null && _board["C8"] == null && _board["D8"] == null)
                {
                    var king = GetTheKing(turn);
                    var rook = GetFigureByCell(new Cell("A8"));

                    if (!king.HasMoved && !rook.HasMoved)
                    {
                        castelingKingCell = new Cell("C8");
                        return true;
                    }
                }
            }

            castelingKingCell = null;
            return false;
        }


        public static bool IsCheckDefenseCell(Figure figureToMove, Cell cellTo)
        {
           
            if (figureToMove is King)
            {
                return true;
            }

            var king = GetTheKing(figureToMove.Color);

            // Check if it is on cell under influence
            if (IsUnderCheckCell(king.CurrentCell, king.Color, out Figure influencingFigure))
            {
                var checkDefenseCells = GetCheckDefenseCells(king, influencingFigure);
                return checkDefenseCells.Contains(cellTo);
            }

            return true;
        }

        public static List<Cell> GetCheckDefenseCells(King king, Figure influencingFigure)
        {
            var checkDefenseCells = new List<Cell>();
            checkDefenseCells.Add(influencingFigure.CurrentCell);
            if (influencingFigure is Pawn || influencingFigure is Knight)
            {
                return checkDefenseCells;
            }

            var kingCell = king.CurrentCell;
            var figureCell = influencingFigure.CurrentCell;

            Cell newCell = new Cell(figureCell.ToString());
            if (kingCell.Letter > figureCell.Letter)
            {
                if (kingCell.Number > figureCell.Number)
                {
                    
                    while (newCell != kingCell)
                    {
                        newCell = new Cell((char)(newCell.Letter + 1), newCell.Number + 1);
                        checkDefenseCells.Add(newCell);
                    }
                }
                else if (kingCell.Number < figureCell.Number)
                {

                    while (newCell != kingCell)
                    {
                        newCell = new Cell((char)(newCell.Letter + 1), newCell.Number - 1);
                        checkDefenseCells.Add(newCell);
                    }
                }
                else
                {
                    while (newCell != kingCell)
                    {
                        newCell = new Cell((char)(newCell.Letter + 1), newCell.Number);
                        checkDefenseCells.Add(newCell);

                    }
                }
            }
            else if (kingCell.Letter < figureCell.Letter)
            {
                if (kingCell.Number > figureCell.Number)
                {
                    while (newCell != kingCell)
                    {
                        newCell = new Cell((char)(newCell.Letter - 1), newCell.Number + 1);
                        checkDefenseCells.Add(newCell);
                    }
                }
                else if (kingCell.Number < figureCell.Number)
                {
                    while (newCell != kingCell)
                    {
                        newCell = new Cell((char)(newCell.Letter - 1), newCell.Number - 1);
                        checkDefenseCells.Add(newCell);
                    }
                }
                else
                {
                    while (newCell != kingCell)
                    {
                        newCell = new Cell((char)(newCell.Letter - 1), newCell.Number);
                        checkDefenseCells.Add(newCell);
                    }
                }
            }
            else
            {
                if (kingCell.Number > figureCell.Number)
                {
                    while (newCell != kingCell)
                    {
                        newCell = new Cell(newCell.Letter, newCell.Number + 1);
                        checkDefenseCells.Add(newCell);
                    }
                }
                else if (kingCell.Number < figureCell.Number)
                {
                    while (newCell != kingCell)
                    {
                        newCell = new Cell(newCell.Letter, newCell.Number - 1);
                        checkDefenseCells.Add(newCell);
                    }
                }
            }

            return checkDefenseCells;
        }

        private static bool IsNotOpeningCheck(Figure figureToMove, Cell cellTo)
        {
            
            var currentCell = figureToMove.CurrentCell;

            var stadingFigure = _board[cellTo.ToString()];
            
            figureToMove.PhantomMove(cellTo);
            UpdateBoard(currentCell, cellTo);
            
            bool IsNotOpeningCheck = !IsCheck(figureToMove.Color);
            figureToMove.PhantomMove(currentCell);
            UpdateBoard(cellTo, currentCell);
            if (stadingFigure != null) 
            {
                _board[cellTo.ToString()] = stadingFigure;
            }
            

            return IsNotOpeningCheck;
        }

        private static bool IsCheck(Color kingColor)
        {
            var king = GetTheKing(kingColor);

            return IsUnderCheckCell(king.CurrentCell, king.Color, out Figure influencingFigure);
        }
    }
}
