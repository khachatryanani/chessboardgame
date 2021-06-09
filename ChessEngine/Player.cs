using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChessEngineLogic
{
    public class Player
    {
        public int PlayerId { get; set; }
        public string PlayerName { get; set; }
        public List<Game> Games { get; set; }
        public int TotalScore { get => Games.Sum(x => x.GameScore);}

        public Player()
        {
            Games = new List<Game>();
        }
    }
}
