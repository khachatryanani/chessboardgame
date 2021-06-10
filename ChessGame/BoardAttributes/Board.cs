using System.Collections;
using System.Collections.Generic;

namespace ChessGame
{
    /// <summary>
    /// Container of all the currenlty available figures of chess game
    /// </summary>
    public class Board : IEnumerable<Figure>
    {
        // Keeps figures in dictionary with their chessboard cell string representation as a key
        private Dictionary<string, Figure> _board = new Dictionary<string, Figure>();

        public int Count { get => _board.Count; }
        //Indexer of the class
        public Figure this[string cell]
        {
            get => _board.ContainsKey(cell)? _board[cell] : null;
            set => _board[cell] = value;
        }

        //Removes the value by key
        public bool Remove(string key)
        {
            return _board.Remove(key);
        }

        public bool Add(string key, Figure value) 
        {
            if (_board.ContainsKey(key)) 
            {
                return false;
            }

            _board.Add(key, value);
            return true;
        }

        public bool Add(Cell cell, Figure value)
        {
            string key = cell.ToString();
            //if (_board.ContainsKey(key))
            //{
            //    return false;
            //}

            //_board.Add(key, value);
            _board[key] = value;
            return true;
        }

        public bool ContainsKey(Cell cell) 
        {
            string key = cell.ToString();
            return _board.ContainsKey(key);
        }

        public bool ContainsKey(string key)
        {
            return _board[key] != null;
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
