using System;
using System.Collections.Generic;
using System.Text;

namespace ChessEngineLogic
{
    public class Game
    {
        public int GameId { get; set; }
        public int OpponentId { get; set; }
        public DateTime PlayedOn { get; set; }

        // 0, 0.5, 1
        public int GameScore { get; set; }

        public Dictionary<string, string> Board { get; set; }
    }
}
