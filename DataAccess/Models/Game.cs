using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class Game
    {
        public int GameId { get; set; }
        public DateTime StartDate { get; set; }

        // 0 - paused, lost, 1 - mate, 0.5 - stalemate
        public double Result { get; set; }
        public int Status { get; set; }
        public bool? Winner { get; set; }
        public bool Turn { get; set; }

        public Player White { get; set; }
        public Player Black { get; set; }

        public Dictionary<string,string> Board { get; set; }
        public List<Tuple<string,string,string>> MovesLog { get; set; }
    }
}
