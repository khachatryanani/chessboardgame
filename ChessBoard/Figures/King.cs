using System;
using System.Collections.Generic;
using System.Text;
using ChessBoard.BoardAttributes;

namespace ChessBoard.Figures
{
    /// <summary>
    /// Represents the Chess figure of King
    /// </summary>
    public class King : Figure
    {
        public King(Cell cell, Color color) : base(cell, color)
        {

        }

        /// <summary>
        /// Get all the cells under influence of King on the current Cell
        /// </summary>
        /// <returns>List of Cells under influance of King on the current Cell</returns>
        protected override List<Cell> GetInfluencedCells()
        {
            List<Cell> cells = new List<Cell>();
            int i = CurrentCell.Number - 1;

            for (; i <= CurrentCell.Number + 1; i++)
            {
                int j = (int)CurrentCell.Letter + 1;
                for (; j >= (int)CurrentCell.Letter - 1; j--)
                {
                    if ((i < 1 || i > 8) || (j < 65 || j > 72))
                    {
                        continue;
                    }
                    cells.Add(new Cell((char)j, i));
                }
            }
            return cells;
        }


    }
}
