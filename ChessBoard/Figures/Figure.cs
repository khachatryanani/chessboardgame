using System;
using System.Collections.Generic;
using ChessBoard.BoardAttributes;
using ChessBoard.Extensions;

namespace ChessBoard.Figures
{
    /// <summary>
    /// Base class of Chess Figures, contains the figures' main properties and methods
    /// </summary>
    public abstract class Figure
    {
        // Holds the current location of figure on the chessboard
        public Cell CurrentCell { get; private set; }

        // Color of the figure
        public Color Color { get; set; }

        public string Name { get; set; }


        // Cells on ChessBoard that are falling under the influence of current figure. 
        //Also, this are the cells that the current figure is able to move to.
        public List<Cell> InfluencedCells { get; set; }

        // Parameterized constructor
        protected Figure(Cell cell, Color color)
        {
            CurrentCell = cell;
            Color = color;
        }

        /// <summary>
        /// Check is the speified Cell is one of the influenced cells of the current figure and moves to it
        /// </summary>
        /// <param name="cell">Cell to move to</param>
        public void Move(Cell cell)
        {
            CurrentCell = cell;

            // Based on every new Cell location, determine what are the new cells of influence zone
            InfluencedCells = GetInfluencedCells();
        }

        /// <summary>
        /// For each type of figure will determine the cells falling under the figure's influence.
        /// Also, it will represent the cells the current figure can move to.
        /// </summary>
        /// <param name="currentCell">Current location of the figure</param>
        /// <returns>A list of cells falling under influence of the current figure</returns>
        protected abstract List<Cell> GetInfluencedCells();
    }
}
