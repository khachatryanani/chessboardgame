using System;
using System.Collections.Generic;
using System.Text;

namespace ChessBoard.BoardAttributes
{
    /// <summary>
    /// Represents one cell on chessboard with letter+number coordinates
    /// </summary>
    public class Cell
    {
        // Letter of chess board cell
        public char Letter { get; set; }

        // Integer of chess board cell
        public int Number { get; set; }

        public Cell(char c, int i)
        {
            Letter = c;
            Number = i;
        }

        // override the equality operators

        public override string ToString()
        {
            string letter = Letter.ToString();
            string number = Number.ToString();
            return letter + number;
        }

        public override bool Equals(object obj)
        {
            Cell cell = obj as Cell;
            return this.Letter == cell.Letter && this.Number == cell.Number;
        }

#nullable enable
        public static bool operator ==(Cell? cell1, Cell? cell2)
        {
            return cell1?.Letter == cell2?.Letter && cell1?.Number == cell2?.Number;
        }

        public static bool operator !=(Cell? cell1, Cell? cell2)
        {
            return !(cell1?.Letter == cell2?.Letter && cell1?.Number == cell2?.Number);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
