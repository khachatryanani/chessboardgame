using System;
using ChessBoard.BoardAttributes;
using ChessBoard.Figures;
using ChessBoard.Extensions;

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
        public static bool IsUnderCheckCell(Cell cellToMove, CellColor colorOfPlayer)
        {
            foreach (var item in _board)
            {
                if (item.InfluencedCells.ContainsCell(cellToMove) && item.Color != colorOfPlayer)
                {
                    King king = GetTheKing(colorOfPlayer);
                    Cell kingCell = king.CurrentCell;
                    Cell placeHolder = new Cell();
                    king.Move(placeHolder);
                    if (IsPossibleToMove(item, cellToMove))
                    {
                        king.Move(kingCell);
                        return true;
                    }
                    king.Move(kingCell);
                }
            }
            return false;
        }

        /// <summary>
        /// Checks if the current cell falls under an influence of any of opposite figure
        /// </summary>
        /// <param name="cellToCheck">Cell to check if there is an influence on</param>
        /// <param name="colorOfPlayer">Color of the current player</param>
        /// <returns>True if the cell is in the influeneced cells of the opposite player, False if it does not</returns>
        public static bool IsInfluencedCell(Cell cellToCheck, CellColor colorOfPlayer)
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

        ///// <summary>
        ///// Checks if the given cell for the given figure is free to stand in
        ///// </summary>
        ///// <param name="cell">Cell to check</param>
        ///// <param name="figureType">Figure to check the cell for</param>
        ///// <param name="color">Color of figure to check the cell for</param>
        ///// <returns>True if cell is acceptable for the given figure, False if not</returns>
        //public static bool IsAcceptableCell(Cell cell, Type figureType, CellColor color, out string error)
        //{
        //    if (IsFreeCell(cell))
        //    {
        //        if (figureType == typeof(King))
        //        {
        //            King king = new King(cell, color);

        //            if (!IsUnderCheckCell(cell, color))
        //            {
        //                var rook = GetFigure(typeof(Rook), CellColor.White) as Rook;
        //                if (Math.Abs(rook.CurrentCell.Letter - cell.Letter) == 1 && Math.Abs(rook.CurrentCell.Number - cell.Number) == 1) 
        //                {
        //                    error = "King will be near rook! Don't do this.";
        //                    return false;
        //                }
        //                foreach (var item in king.InfluencedCells)
        //                {
        //                    if (!IsUnderCheckCell(item, color) && item != king.CurrentCell)
        //                    {
        //                        error = string.Empty;
        //                        return true;
        //                    }
        //                }
        //                error = "This will cause a stalemate!";
        //                return false;
        //            }
        //            error = "King will be under check!";
        //            return false;
        //        }
        //        error = string.Empty;
        //        return true;
        //    }
        //    error = "There is already a figure in this cell!";
        //    return false;
        //}

        /// <summary>
        /// Checks if the given figure can move to the given cell: if it is free and if the path to reach it is also free
        /// </summary>
        /// <param name="figure">Figure to move</param>
        /// <param name="cellToMove">Cell to move the figure to</param>
        /// <returns>True is it is possible to move the given figure to the given cell, False if not</returns>
        public static bool IsPossibleToMove(Figure figure, Cell cellToMove)
        {
            if (!figure.InfluencedCells.ContainsCell(cellToMove))
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
        public static King GetTheKing(CellColor colorOfKing)
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
        public static Figure GetFigure(Type typeOfFigure, CellColor colorofFigure, int order = 1)
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
    }
}
