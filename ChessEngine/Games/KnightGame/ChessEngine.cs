using System;
using System.Collections.Generic;
using ChessBoard.BoardAttributes;
using ChessBoard.Figures;
using ChessBoard.Extensions;
using static ChessBoard.ChessBoardManager;


namespace ChessEngineLogic
{
    /// <summary>
    /// Implementing Knight Minial Moves game algorithm
    /// </summary>
    public partial class ChessEngine
    {
        public List<string> PlayMinKinghtMovesAlgorithm(string cellFrom, string cellTo)
        {
            var moves = GetPath(new Cell(cellFrom), new Cell(cellTo));
            return moves;
        }

        private List<string> GetPath(Cell cellFrom, Cell cellTo)
        {

            // Get the relative positing of knight cell to the target cell
            int number = Math.Abs(cellTo.Number - cellFrom.Number);
            int letter = Math.Abs(cellTo.Letter - cellFrom.Letter);

            bool isReverted = false;
            Cell target = cellTo;
            Cell start = cellFrom;

            if (cellTo.Letter >= cellFrom.Letter && cellTo.Number <= cellFrom.Number) 
            {
                isReverted = true;
                target = cellFrom;
                start = cellTo;
            }

            Knight knight = new Knight(start, Color.Black);
            var cells = GetKnightMoveRelativeCells(number, letter);
            
            List<string> path = new List<string>();

            foreach (var cell in cells)
            {
                var point = new Cell((char)(target.Letter + cell.Item1), target.Number + cell.Item2);
                if (IsValidCell(point) && knight.InfluencedCells.ContainsCell(point)) 
                {
                    knight.Move(point);
                    path.Add(point.ToString());
                    continue;
                }

                point = new Cell((char)(target.Letter - cell.Item1), target.Number + cell.Item2);
                if (IsValidCell(point) && knight.InfluencedCells.ContainsCell(point))
                {
                    knight.Move(point);
                    path.Add(point.ToString());
                    continue;
                }

                point = new Cell((char)(target.Letter + cell.Item1), target.Number - cell.Item2);
                if (IsValidCell(point) && knight.InfluencedCells.ContainsCell(point))
                {
                    knight.Move(point);
                    path.Add(point.ToString());
                    continue;
                }

                point = new Cell((char)(target.Letter - cell.Item1), target.Number - cell.Item2);
                if (IsValidCell(point) && knight.InfluencedCells.ContainsCell(point))
                {
                    knight.Move(point);
                    path.Add(point.ToString());
                    continue;
                }

                point = new Cell((char)(target.Letter + cell.Item2), target.Number + cell.Item1);
                if (IsValidCell(point) && knight.InfluencedCells.ContainsCell(point))
                {
                    knight.Move(point);
                    path.Add(point.ToString());
                    continue;
                }

                point = new Cell((char)(target.Letter - cell.Item2), target.Number + cell.Item1);
                if (IsValidCell(point) && knight.InfluencedCells.ContainsCell(point))
                {
                    knight.Move(point);
                    path.Add(point.ToString());
                    continue;
                }

                point = new Cell((char)(target.Letter + cell.Item2), target.Number - cell.Item1);
                if (IsValidCell(point) && knight.InfluencedCells.ContainsCell(point))
                {
                    knight.Move(point);
                    path.Add(point.ToString());
                    continue;
                }

                point = new Cell((char)(target.Letter - cell.Item2), target.Number - cell.Item1);
                if (IsValidCell(point) && knight.InfluencedCells.ContainsCell(point))
                {
                    knight.Move(point);
                    path.Add(point.ToString());
                }
            }

            if (isReverted) 
            {
                path.Reverse();
            }
            return path;
        }

        
        private List<(int, int)> GetKnightMoveRelativeCells(int number, int letter)
        {
            int[,] template = CreateTemplate();
            int moves = template[number, letter];
            List<(int, int)> cells = new List<(int, int)>();
            cells.Add((number, letter));
            for (int i = moves - 1; i >= 0; i--)
            {
                if (number - 2 >= 0 && letter - 1 >= 0 && template[number - 2, letter - 1] == i)
                {
                    number -= 2;
                    letter -= 1;
                    cells.Add((number, letter));
                }
                else if (number - 2 >= 0 && letter + 1 <= 7 && template[number - 2, letter + 1] == i)
                {
                    number -= 2;
                    letter += 1;
                    cells.Add((number, letter));
                }
                else if (number - 1 >= 0 && letter - 2 >= 0 && template[number - 1, letter - 2] == i)
                {
                    number -= 1;
                    letter -= 2;
                    cells.Add((number, letter));
                }
                else if (number - 1 >= 0 && letter + 2 <= 7 && template[number - 1, letter + 2] == i)
                {
                    number -= 1;
                    letter += 2;
                    cells.Add((number, letter));
                }
                else if (number + 1 <= 7 && letter - 2 >= 0 && template[number + 1, letter - 2] == i)
                {
                    number += 1;
                    letter -= 2;
                    cells.Add((number, letter));
                }
                else if (number + 1 <= 7 && letter + 2 <= 7 && template[number + 1, letter + 2] == i)
                {
                    number += 1;
                    letter += 2;
                    cells.Add((number, letter));
                }
                else if (number + 2 <= 7 && letter - 1 >= 0 && template[number + 2, letter - 1] == i)
                {
                    number += 2;
                    letter -= 1;
                    cells.Add((number, letter));
                }
                else if (number + 2 <= 7 && letter + 1 <= 7 && template[number + 2, letter + 1] == i)
                {
                    number += 2;
                    letter += 1;
                    cells.Add((number, letter));
                }
            }

            return cells;
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

        private bool IsValidCell(Cell cell) 
        {
            return cell.Letter <= 72 && cell.Letter >= 65 && cell.Number <= 8 && cell.Number >= 1;
        }
    }
}
