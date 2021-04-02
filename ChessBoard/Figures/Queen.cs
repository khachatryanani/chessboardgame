using System.Collections.Generic;
using ChessBoard.BoardAttributes;

namespace ChessBoard.Figures
{
    /// <summary>
    /// Represents the Chess figure of Queen
    /// </summary>
    public class Queen : Figure
    {
        // Parameterized constructor
        public Queen(Cell cell, CellColor color) : base(cell, color)
        {

        }

        /// <summary>
        /// Get all the cells under influence of Queen on the current Cell
        /// </summary>
        /// <returns>List of Cells under influance of Queen on the current Cell</returns>
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

            int digN = CurrentCell.Number + 1;
            int digL = (int)CurrentCell.Letter + 1;
            while (digN <= 8 && digL <= 72)
            {
                cells.Add(new Cell((char)digL, digN));
                digN++;
                digL++;
            }

            digN = CurrentCell.Number + 1;
            digL = (int)CurrentCell.Letter - 1;
            while (digN <= 8 && digL >= 65)
            {
                cells.Add(new Cell((char)digL, digN));
                digN++;
                digL--;
            }

            digN = CurrentCell.Number - 1;
            digL = (int)CurrentCell.Letter + 1;
            while (digN >= 1 && digL <= 72)
            {
                cells.Add(new Cell((char)digL, digN));
                digN--;
                digL++;
            }

            digN = CurrentCell.Number - 1;
            digL = (int)CurrentCell.Letter - 1;
            while (digN >= 1 && digL >= 65)
            {
                cells.Add(new Cell((char)digL, digN));
                digN--;
                digL--;
            }

            return cells;
        }
    }
}
