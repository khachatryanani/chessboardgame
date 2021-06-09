using ChessEngineLogic;
using System;
using System.Collections.Generic;
using System.Text;

namespace KingdomChessGame_Desktop
{
    public class GameViewModel
    {
        public int GameId { get; set; }
        public string StartDate { get; set; }

        // 0 - paused, lost, 1 - mate, 0.5 - stalemate
        public string Result { get; set; }
        public string Winner { get; set; }
        public string Turn { get; set; }
        public PlayerViewModel White { get; set; }
        public PlayerViewModel Black { get; set; }
        public List<Tuple<string, string, string>> MovesLog { get; set; }
        public Dictionary<string, string> Board { get; set; } 
    }
}
