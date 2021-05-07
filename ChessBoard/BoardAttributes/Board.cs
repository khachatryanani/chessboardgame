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
        private Dictionary<string, Figure> _board = new Dictionary<string, Figure>();

        //Indexer of the class
        public Figure this[string cell]
        {
            get => _board.ContainsKey(cell) ? _board[cell] : null;
            set => _board[cell] = value;
        }

        //Removes the value by key
        public bool Remove(string key)
        {
            return _board.Remove(key);
        }

        public void Clear() 
        {
            _board = new Dictionary<string, Figure>();
        }

        //IEnumarator interface implementation
        public IEnumerator<Figure> GetEnumerator()
        {
            return _board.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_board.Values).GetEnumerator();
        }
    }
}
