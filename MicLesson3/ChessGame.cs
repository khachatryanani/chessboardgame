using System;
using System.Collections.Generic;
using ChessBoard.BoardAttributes;
using ChessBoard.Figures;

namespace ChessGameManager
{
    class ChessGame
    {
        static void Main()
        {
            var figures = new List<KeyValuePair<Color, Type>>()
            {
                new KeyValuePair<Color, Type>(Color.White, typeof(Queen)),
                new KeyValuePair<Color, Type>(Color.White, typeof(Rook)),
                new KeyValuePair<Color, Type>(Color.White, typeof(Rook)),
                new KeyValuePair<Color, Type>(Color.White, typeof(King)),
                new KeyValuePair<Color, Type>(Color.Black, typeof(King))
            };

            ChessManager.Run(Color.Black, figures);
        }
    }
}
