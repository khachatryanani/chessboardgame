using System;
using System.Collections.Generic;

namespace ChessGame
{
    public class Knight : Figure
    {
        public Knight(Cell cell, Color color) : base(cell, color)
        {
            string colorString = color == Color.Black ? "B" : "W";
            this.Name = colorString + "N";

            InfluencedCells = GetInfluencedCells();
        }

        protected override List<Cell> GetInfluencedCells() 
        {
            int letter = this.CurrentCell.Letter;
            int number = this.CurrentCell.Number;
            List<Cell> cells = new List<Cell>();
            for (int i = letter - 2; i <= letter + 2; i++)
            {
                if (i == letter) 
                {
                    continue;
                }
                if (i >= 65 && i <= 72)
                {
                    if ((number + (3 - Math.Abs(letter - i)) >= 1 && number + (3 - Math.Abs(letter - i)) <= 8)) 
                    {
                        cells.Add(new Cell((char)i, number + (3 - Math.Abs(letter - i))));
                    }

                    if ((number - (3 - Math.Abs(letter - i)) >= 1 && number - (3 - Math.Abs(letter - i)) <= 8))
                    {
                        cells.Add(new Cell((char)i, number - (3 - Math.Abs(letter - i))));
                    }
                }
            }

            return cells;
        }
    }
}
