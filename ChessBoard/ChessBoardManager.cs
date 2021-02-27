﻿using System;
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
        public static bool IsUnderCheckCell(Cell cellToMove, Color colorOfPlayer)
        {
            foreach (var item in _board)
            {
                if (item.InfluencedCells.ContainsCell(cellToMove) && item.Color != colorOfPlayer)
                {
                    if (IsPossibleToMove(item, cellToMove))
                    {
                        return true;
                    }
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
        /// Checks if the given cell for the given figure is free to stand in
        /// </summary>
        /// <param name="cell">Cell to check</param>
        /// <param name="figureType">Figure to check the cell for</param>
        /// <param name="color">Color of figure to check the cell for</param>
        /// <returns>True if cell is acceptable for the given figure, False if not</returns>
        public static bool IsAcceptableCell(Cell cell, Type figureType, Color color)
        {
            if (IsFreeCell(cell))
            {
                if (figureType == typeof(King))
                {
                    if (!IsUnderCheckCell(cell, color))
                    {
                        King king = new King(cell, color);
                        foreach (var item in king.InfluencedCells)
                        {
                            if (!IsUnderCheckCell(item, color) && item != king.CurrentCell)
                            {
                                return true;
                            }
                        }
                        Console.WriteLine("This will cause a stalemate!");
                        return false;
                    }
                    Console.WriteLine("King will be under check!");
                    return false;
                }
                return true;
            }
            Console.WriteLine("There is already a figure in this cell!");
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
                    // Checks if the path from cell to move from to cell to move to is completle free
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
                //Case1: up-right
                if (numberMoveFrom > numberMoveTo && letterMoveFrom > letterMoveTo)
                {
                    Cell cell;
                    // Checks if the path from cell to move from to cell to move to is completle free
                    for (int i = numberMoveFrom - 1, j = letterMoveFrom - 1; i > numberMoveTo && j > letterMoveTo; i--, j--)
                    {
                        cell = new Cell((char)j, i);
                        if (!IsFreeCell(cell))
                        {
                            return false;
                        }
                    }
                    return true;
                }
                //Case2: up-left
                else if (numberMoveFrom > numberMoveTo && letterMoveFrom < letterMoveTo)
                {
                    Cell cell;
                    // Checks if the path from cell to move from to cell to move to is completle free
                    for (int i = numberMoveFrom - 1, j = letterMoveFrom + 1; i > numberMoveTo && j < letterMoveTo; i--, j++)
                    {
                        cell = new Cell((char)j, i);
                        if (!IsFreeCell(cell))
                        {
                            return false;
                        }
                    }
                    return true;
                }
                //Case3: down-right
                else if (numberMoveFrom < numberMoveTo && letterMoveFrom > letterMoveTo)
                {
                    Cell cell;
                    // Checks if the path from cell to move from to cell to move to is completle free
                    for (int i = numberMoveFrom + 1, j = letterMoveFrom - 1; i < numberMoveTo && j > letterMoveTo; i++, j--)
                    {
                        cell = new Cell((char)j, i);
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
                    Cell cell;
                    // Checks if the path from cell to move from to cell to move to is completle free
                    for (int i = numberMoveFrom + 1, j = letterMoveFrom + 1; i < numberMoveTo && j < letterMoveTo; i++, j++)
                    {
                        cell = new Cell((char)j, i);
                        if (!IsFreeCell(cell))
                        {
                            return false;
                        }
                    }
                    return true;
                }
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

        /// <summary>
        /// Based on the array of Figures and their Figure type displays the board in Console.
        /// </summary>
        public static void DrawBoard()
        {
            Console.ForegroundColor = ConsoleColor.Green;

            // Print the heading of the board
            Console.WriteLine("    H   G   F   E   D   C   B   A   ");
            Console.WriteLine("  +---+---+---+---+---+---+---+---+");

            for (int i = 0; i < 8; i++)
            {
                Console.Write($"{i + 1} | ");

                for (int j = 0; j < 8; j++)
                {
                    string cell = ConvertIndexesToCell(i, j);
                    if (_board[cell] == null)
                    {
                        Console.Write("  | ");
                    }
                    else
                    {
                        // Get the first letter of figure type as an icon to display in Console.
                        string icon = _board[cell].GetType().Name.Substring(0, 1);
                        Color color = _board[cell].Color;
                        //Change the Console Forground Color based on the Figure color (Red for Blacks and White for Whites)
                        if (color == Color.White)
                        {
                            Console.ForegroundColor = ConsoleColor.White;
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                        }

                        Console.Write(icon);
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write(" | ");
                    }
                }

                Console.Write("\n");
                Console.WriteLine("  +---+---+---+---+---+---+---+---+");
            }
        }

        /// <summary>
        /// Converts user input string to Cell object and returns it. Returns Null if user input was invalid
        /// </summary>
        /// <param name="userResponse">User input string</param>
        /// <returns>Valid Cell object</returns>
        public static Cell ConvertToValidCell(string userResponse)
        {
            if (userResponse.Length != 2)
            {
                Console.WriteLine("No matching cell for chessboard!");
                return null;
            }

            char Letter = userResponse.ToUpper()[0];
            int Number = (int)userResponse[1] - 48;
            if ((Letter < 65 || Letter > 72) || (Number < 1 || Number > 8))
            {
                Console.WriteLine("No matching cell for chessboard!");
                return null;
            }

            return new Cell(Letter, Number);
        }

        /// <summary>
        /// Converts to indexes to location string of the chess board
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <returns>Chessboard cell string representation</returns>
        private static string ConvertIndexesToCell(int i, int j)
        {
            string number = (i + 1).ToString();
            char letter = (char)(72 - j);

            return letter + number;
        }
    }

}
