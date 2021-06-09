using System;
using System.Collections.Generic;
using System.Text;

namespace KingdomChessGame_Desktop
{
    public class PlayerViewModel
    {
        public int PlayerId { get; set; }
        public string Name { get; set; }
        public int NumberOfGames { get; set; }
        public double TotalScore { get; set; }

        public override string ToString()
        {
            return this.Name;
        }
    }
}
