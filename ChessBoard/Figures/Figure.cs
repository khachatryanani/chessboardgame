using System;
using System.Collections.Generic;
using System.Text;
using ChessBoard.BoardAttributes;
using ChessBoard.Extensions;

namespace ChessBoard.Figures
{
    /// <summary>
    /// Base class of Chess Figures, contains the main figure properties, defines some virtual methodes to be overriden in derived classes
    /// </summary>
    public class Figure
    {
        public Cell CurrentCell { get; private set; }

        public Color Color { get; set; }

        // Cells on ChessBoard that are falling under the influence of current figure. 
        //Also, this are the cells that the current figure can move to.
        public List<Cell> InfluencedCells { get; set; }

        public Figure(Cell cell, Color color)
        {
            CurrentCell = cell;
            Color = color;
            InfluencedCells = GetInfluencedCells();
        }

        /// <summary>
        /// Check is the speified Cell is one of the influenced cells of the current figure and moves to it
        /// </summary>
        /// <param name="cell">Cell to move to</param>
        /// <returns>True if the figure moved to, false if no movement occured</returns>
        public void Move(Cell cell)
        {
            if (!InfluencedCells.ContainsCell(cell))
            {
                Console.WriteLine($"Cell is out of reach for your figure.");

            }

            CurrentCell = cell;

            // Based on every new Cell location, determine what are the new cells of influence zone
            InfluencedCells = GetInfluencedCells();
            //return true;
        }

        /// <summary>
        /// For each type of figure will determine the cells falling under the figure's influence.
        /// Also, it will represent the cells the current figure can move to.
        /// </summary>
        /// <param name="currentCell">Current location of the figure</param>
        /// <returns>A list of cells falling under influence of the current figure</returns>
        protected virtual List<Cell> GetInfluencedCells()
        {
            throw new NotImplementedException();
        }
    }
}
