﻿using System;
using ChessBoard.BoardAttributes;
using ChessBoard.Figures;
using static ChessBoard.ChessBoardManager;
using System.Collections.Generic;

namespace ChessEngineLogic
{
    public partial class ChessEngine
    {
        /// <summary>
        /// Turn of the player represented by CellColor enum value
        /// </summary>
        private CellColor _turn;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="playerColor">Color of the first player.</param>
        public ChessEngine(string playerColor) 
        {
            _turn = playerColor == "Black" ? CellColor.Black : CellColor.White;
        }

        /// <summary>
        /// In the endgame, plays as Chess Engine.
        /// </summary>
        /// <param name="choice">Game choice</param>
        public void PlayEngine(int choice)
        {
            switch (choice) 
            {
                case 2:
                    PlayWinningWithQueenAndRookAlgorithm();
                    break;
                case 3:
                    break;
            }
        }

        /// <summary>
        /// According the occured move updates the inner board of Chess Engine and fires the FigreMoved event.
        /// </summary>
        /// <param name="figure">Figure that moved.</param>
        /// <param name="cellFrom">Cell from which the figure moved.</param>
        /// <param name="cellTo">Cell to which the figure moved.</param>
        private void UpdatePosition(Figure figure, Cell cellFrom, Cell cellTo)
        {
            UpdateBoard(cellFrom, cellTo);
            OnFigureMoved(figure.Name, cellFrom.ToString(), cellTo.ToString());
        }

        /// <summary>
        /// Get the list of cell names for the figure that are possible to move to.
        /// </summary>
        /// <param name="cellString">Cell name that figure currenlty stands on.</param>
        /// <returns>List of cells names that are possible to move to.</returns>
        public List<string> GetPossibleMoves(string cellString)
        {
            Cell currentCell = new Cell(cellString);
            Figure figure = GetFigureByCell(currentCell);

            List<string> possibleCells = new List<string>();
            foreach (var cell in figure.InfluencedCells)
            {
                if (IsPossibleToMove(figure, cell) && figure.CurrentCell != cell)
                {
                    if ((figure is King))
                    {
                        if (!IsUnderCheckCell(cell, figure.Color))
                        {
                            possibleCells.Add(cell.ToString());
                        }
                    }
                    else
                    {
                        possibleCells.Add(cell.ToString());
                    }
                }
            }

            return possibleCells;
        }

        /// <summary>
        /// Creates a figure with the given parameters and adds them to the inner board.
        /// </summary>
        /// <param name="cellString">Cell name that the figure stands on.</param>
        /// <param name="typeString">Type name of the figure to be created.</param>
        /// <param name="colorString">Color name of the figure to be created.</param>
        public void CreateFigure(string cellString, string typeString, string colorString)
        {
            Type type = GetTypeFromString(typeString);
            CellColor color = GetCellColorFromString(colorString);
            Cell cell = new Cell(cellString);

            // Create a figure
            Figure figure = (Figure)Activator.CreateInstance(type, cell, color);

            // Add to ChessBoard
            AddFigure(figure);
        }

        /// <summary>
        /// Get the Type based on the type name
        /// </summary>
        /// <param name="type">string representation of type name</param>
        /// <returns>Type according the string name of the type./returns>
        private Type GetTypeFromString(string type)
        {
            return type switch
            {
                "Q" => typeof(Queen),
                "R" => typeof(Rook),
                _ => typeof(King),
            };
        }

        /// <summary>
        /// Get the CellColor enum value based on the color name
        /// </summary>
        /// <param name="color">String representation of color</param>
        /// <returns>CellColor value relevant to given color name.</returns>
        private CellColor GetCellColorFromString(string color)
        {
            return color == "B" ? CellColor.Black : CellColor.White;
        }

        /// <summary>
        /// Registered the user move and updates the inner board.
        /// </summary>
        /// <param name="cellFrom">Cell from which user moved.</param>
        /// <param name="cellTo">Cell to which user moved.</param>
        public void PlayUser(string cellFrom, string cellTo) 
        {
            Figure figure = GetFigureByCell(new Cell(cellFrom));
            figure.Move(new Cell(cellTo));
            
            UpdateBoard(new Cell(cellFrom), new Cell(cellTo));

            this._turn = this._turn == CellColor.Black ? CellColor.White : CellColor.Black;
        }

        /// Checks if it is a Mate situation in the game:
        /// if the King is under influence of opposite color figure and has no available cells to move to
        /// </summary>
        /// <returns>True if it is a Mate, False if it is not a Mate</returns>
        private bool IsMate()
        {
            // Get the King Figure
            King king = GetTheKing(_turn);

            if (IsUnderCheckCell(king.CurrentCell, king.Color))
            {
                foreach (var item in king.InfluencedCells)
                {
                    if (!IsInfluencedCell(item, king.Color) && IsFreeCell(item))
                    {
                        return false;
                    }
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// Verifies if the king of the current player is/will be under Chess Check
        /// </summary>
        /// <returns>True if the king is under Chess Check, False if it does not</returns>
        private bool IsCheck()
        {

            // Get the King Figure
            King king = GetTheKing(_turn);

            // Check if it is on cell under influence
            if (IsUnderCheckCell(king.CurrentCell, king.Color))
            {

                return true;
            }

            return false;
        }

        /// <summary>
        /// Check if it is a stalemate situation in the game
        /// </summary>
        /// <returns>True if it is a stalemate, False if it is not</returns>
        private bool IsStaleMate()
        {
            King king = GetTheKing(_turn);

            if (!IsUnderCheckCell(king.CurrentCell, king.Color))
            {
                foreach (var item in king.InfluencedCells)
                {
                    if (!IsUnderCheckCell(item, king.Color) && item != king.CurrentCell)
                    {
                        return false;
                    }
                }

                return true;
            }

            return false;
        }
    }
}