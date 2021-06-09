using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccess
{
    public class Player
    {
        public int PlayerId { get; set; }
        public string Name { get; set; }
        public int NumberOfGames { get; set; }
        public double TotalScore { get; set; }
    }
}
