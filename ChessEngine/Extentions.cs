﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ChessEngineLogic
{
    public static class Extentions
    {
        public static bool ContainsGame(this List<GameModel> games, GameModel game) 
        {
            foreach (var g in games)
            {
                if (g.GameId == game.GameId) 
                {
                    return true;
                }
            }

            return false;
        }

        public static void UpdateExistingGame(this List<GameModel> games, GameModel game) 
        {
            for (int i = 0; i < games.Count; i++)
            {
                if (games[i].StartDate == game.StartDate) 
                {
                    games[i] = game;
                }
            }
           
        }

        public static bool ContainsCellFrom(this List<Tuple<string,string,string>> movesLog, string cellFrom) 
        {
            foreach (var move in movesLog)
            {
                if (move.Item2 == cellFrom) 
                {
                    return true;
                }
            }

            return false;
        }
    }
}
