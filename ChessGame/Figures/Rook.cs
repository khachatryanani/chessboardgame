﻿using System.Collections.Generic;

namespace ChessGame
{
    /// <summary>
    /// Represents the Chess figure of Rook
    /// </summary>
    public class Rook : Figure
    {
        // Parameterized constructor
        public Rook(Cell cell, Color color) : base(cell, color)
        {
            string colorString = color == Color.Black ? "B" : "W";
            this.Name = colorString + "R";

            InfluencedCells = GetInfluencedCells();
        }

        /// <summary>
        /// Get all the cells under influence of Rook on the current Cell
        /// </summary>
        /// <returns>List of Cells under influance of Rook on the current Cell</returns>
        protected override List<Cell> GetInfluencedCells()
        {
            List<Cell> cells = new List<Cell>();
            for (int i = 1; i <= 8; i++)
            {
                cells.Add(new Cell(CurrentCell.Letter, i));
            }

            for (int j = 65; j <= 72; j++)
            {
                if (j == (int)CurrentCell.Letter)
                {
                    continue;
                }

                cells.Add(new Cell((char)j, CurrentCell.Number));
            }

            return cells;
        }
    }
}
