using ChessBoard.Figures;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace ChessBoard.BoardAttributes
{
    /// <summary>
    /// Contains all the currenlty available figures of chess game
    /// </summary>
    public class Board : IEnumerable<Figure>
    {
        private readonly Dictionary<string, Figure> _board = new Dictionary<string, Figure>();

        public Figure this[string cell]
        {
            get
            {
                if (_board.ContainsKey(cell))
                {
                    return _board[cell];
                }
                else
                {
                    return null;
                }
            }
            set
            {
                _board[cell] = value;
            }
        }
        public bool Remove(string key)
        {
            return _board.Remove(key);
        }

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
