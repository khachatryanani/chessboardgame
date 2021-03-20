using ChessBoard.Figures;
using System.Collections;
using System.Collections.Generic;

namespace ChessBoard.BoardAttributes
{
    /// <summary>
    /// Container of all the currenlty available figures of chess game
    /// </summary>
    public class Board : IEnumerable<Figure>
    {
        // Keeps figures in dictionary with their chessboard cell string representation as a key
        private readonly Dictionary<string, Figure> board = new Dictionary<string, Figure>();

        //Indexer of the class
        public Figure this[string cell]
        {
            get
            {
                if (board.ContainsKey(cell))
                {
                    return board[cell];
                }
                else
                {
                    return null;
                }
            }
            set
            {
                board[cell] = value;
            }
        }

        //Removes the value by key
        public bool Remove(string key)
        {
            return board.Remove(key);
        }

        //IEnumarator interface implementation
        public IEnumerator<Figure> GetEnumerator()
        {
            return board.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)board.Values).GetEnumerator();
        }
    }
}
