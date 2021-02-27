﻿using ChessBoard.Figures;
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
        private readonly Dictionary<string, Figure> _board = new Dictionary<string, Figure>();

        //Indexer of he class
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

        //Removes the value by key
        public bool Remove(string key)
        {
            return _board.Remove(key);
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
