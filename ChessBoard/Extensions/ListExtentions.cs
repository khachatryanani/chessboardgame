using System.Collections.Generic;
using ChessBoard.BoardAttributes;

namespace ChessBoard.Extensions
{
    public static class ListExtensions
    {
        /// <summary>
        /// Extension for List of Cells to check if the list contains the given Cell
        /// </summary>
        /// <param name="obj">List of Cellc</param>
        /// <param name="c">Cell to look for</param>
        /// <returns>True, if list contains the current cell, False if it does not</returns>
        public static bool ContainsCell(this List<Cell> obj, Cell c)
        {
            foreach (var item in obj)
            {
                if (item.Number == c.Number && item.Letter == c.Letter)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
