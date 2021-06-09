using System;
using System.Collections.Generic;
using System.Text;

namespace ChessEngineLogic
{
    public class PlayerModel
    {
        public int PlayerId { get; set; }
        public string Name { get; set; }
        public int NumberOfGames { get; set; }
        public double TotalScore { get; set; }
    }
}
