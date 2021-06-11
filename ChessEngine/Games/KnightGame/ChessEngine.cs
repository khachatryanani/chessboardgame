using ChessGame;
using System;
using System.Collections.Generic;

namespace ChessEngineLogic
{
    /// <summary>
    /// Implementing Knight Minial Moves game algorithm
    /// </summary>
    public partial class ChessEngine
    {
        public List<string> PlayMinKinghtMovesAlgorithm(string cellFrom, string cellTo)
        {
            var moves = GetMinPath(new Cell(cellFrom), new Cell(cellTo));
            return moves;
        }

        private static int[,] CreateTemplate()
        {
            int[,] fixedTemplate = new int[,]
            {
               { 0,3,2,3,2,3,4,5},
               { 3,4,1,2,3,4,3,4},
               { 2,1,4,3,2,3,4,5},
               { 3,2,3,2,3,4,3,4},
               { 2,3,2,3,4,3,4,5},
               { 3,4,3,4,3,4,5,4},
               { 4,3,4,3,4,5,4,5},
               { 5,4,5,4,5,4,5,6},
            };

            return fixedTemplate;
        }

        private int GetMinStepCount(Cell cellFrom, Cell cellTo)
        {
            // Get the relative positing of knight cell to the target cell
            int number = Math.Abs(cellTo.Number - cellFrom.Number);
            int letter = Math.Abs(cellTo.Letter - cellFrom.Letter);

            int[,] template = CreateTemplate();
            return template[number, letter];
        }

        public List<string> GetMinPath(Cell cellFrom, Cell cellTo)
        {
            var movesList = new List<string>();
            
            int moves = GetMinStepCount(cellFrom, cellTo);
            
            for (int m = 0; m <= moves; m++)
            {
                for (int i = 1; i <= 8; i++)
                {
                    int count = movesList.Count;
                    for (int j = 65; j <= 72; j++)
                    {
                        var currentCell = new Cell($"{(char)j}{i}");
                        if (GetMinStepCount(cellFrom, currentCell) == 1 &&
                            GetMinStepCount(new Cell($"{(char)j}{i}"), cellTo) == moves - m)
                        {
                            cellFrom = currentCell;
                            
                            movesList.Add(currentCell.ToString());
                            break;
                        }
                    }

                    if (count != movesList.Count) 
                    {
                        break;
                    }
                }
            }

            return movesList;

        }
    }
}
