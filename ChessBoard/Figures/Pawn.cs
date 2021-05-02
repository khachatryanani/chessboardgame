using System;
using System.Collections.Generic;
using System.Text;
using ChessBoard.BoardAttributes;
using ChessBoard.Extensions;

namespace ChessBoard.Figures
{
    public class Pawn : Figure
    {
        public List<Cell> MovementCells { get; set; }
        public Pawn(Cell cell, Color color) : base(cell, color)
        {
            string colorString = color == Color.Black ? "B" : "W";
            this.Name = colorString + "P";

            InfluencedCells = GetInfluencedCells();
            MovementCells = GetMovementCells();
        }

        public override void Move(Cell cell)
        {
            base.Move(cell);
            MovementCells = GetMovementCells();
        }

        public override void PhantomMove(Cell cell)
        {
            base.PhantomMove(cell);
            MovementCells = GetMovementCells();
        }

        /// <summary>
        /// Get all the cells under influence of Queen on the current Cell
        /// </summary>
        /// <returns>List of Cells under influance of Queen on the current Cell</returns>
        protected override List<Cell> GetInfluencedCells()
        {
            List<Cell> cells = new List<Cell>();
            cells.Add(CurrentCell);

            if (Color == Color.Black)
            {
                if (CurrentCell.Letter - 1 >= 65)
                {
                    cells.Add(new Cell((char)(CurrentCell.Letter - 1), CurrentCell.Number - 1));
                }

                if (CurrentCell.Letter + 1 <= 72)
                {
                    cells.Add(new Cell((char)(CurrentCell.Letter + 1), CurrentCell.Number - 1));
                }
            }
            else
            {
                if (CurrentCell.Letter - 1 >= 65)
                {
                    cells.Add(new Cell((char)(CurrentCell.Letter - 1), CurrentCell.Number + 1));
                }

                if (CurrentCell.Letter + 1 <= 72)
                {
                    cells.Add(new Cell((char)(CurrentCell.Letter + 1), CurrentCell.Number + 1));
                }
            }
            return cells;
        }

        private List<Cell> GetMovementCells()
        {
            List<Cell> cells = new List<Cell>();
            //cells.Add(CurrentCell);

            if (Color == Color.Black)
            {
                cells.Add(new Cell(CurrentCell.Letter, CurrentCell.Number - 1));
                if (!HasMoved)
                {
                    cells.Add(new Cell(CurrentCell.Letter, CurrentCell.Number - 2));
                }
            }
            else
            {
                cells.Add(new Cell(CurrentCell.Letter, CurrentCell.Number + 1));
                if (!HasMoved)
                {
                    cells.Add(new Cell(CurrentCell.Letter, CurrentCell.Number + 2));
                }
            }

            return cells;
        }
    }
}
