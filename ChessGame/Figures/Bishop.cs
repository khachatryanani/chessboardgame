using System.Collections.Generic;

namespace ChessGame
{
    public class Bishop: Figure
    {
        // Parameterized constructor
        public Bishop(Cell cell, Color color) : base(cell, color)
        {
            string colorString = color == Color.Black ? "B" : "W";
            this.Name = colorString + "B";

            InfluencedCells = GetInfluencedCells();
        }

        /// <summary>
        /// Get all the cells under influence of Queen on the current Cell
        /// </summary>
        /// <returns>List of Cells under influance of Queen on the current Cell</returns>
        protected override List<Cell> GetInfluencedCells()
        {
            List<Cell> cells = new List<Cell>
            {
                CurrentCell
            };

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
