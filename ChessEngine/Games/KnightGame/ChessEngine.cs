using System;
using System.Collections.Generic;
using ChessBoard.BoardAttributes;

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


            var cells = GetKnightMoveCells(number, letter);
            var coefs = GetCoefficiants(cellFrom, cellTo);
            List<string> path = new List<string>();
            foreach (var item in cells)
            {
                Cell pathCell = new Cell((char)(cellTo.Letter + (coefs.Item2 * item.Item2)), cellTo.Number + (coefs.Item1 * item.Item1));
                if (pathCell.Number > 8)
                {

                    pathCell.Number -= 2 * (Math.Abs(cellFrom.Number - pathCell.Number));
                }
                else if (pathCell.Number < 1)
                {
                    pathCell.Number += 2 * (Math.Abs(cellFrom.Number - pathCell.Number));
                }

                if (pathCell.Letter > 72)
                {
                    pathCell.Letter -= (char)(2 * (Math.Abs(cellFrom.Letter - pathCell.Letter)));
                }
                else if (pathCell.Letter < 65)
                {
                    pathCell.Letter += (char)(2 * (Math.Abs(cellFrom.Letter - pathCell.Letter)));
                }

                //if (cellFrom.Letter < cellTo.Letter || cellFrom.Number > cellTo.Number) 
                //{
                //    pathCell.Number -= 2 * (Math.Abs(cellFrom.Number - pathCell.Number));
                //}

                //if (cellFrom.Letter < cellTo.Letter)
                //{
                //    pathCell.Letter += (char)(2 * (Math.Abs(cellFrom.Letter - pathCell.Letter)));
                //}
                path.Add(pathCell.ToString());
                cellFrom = pathCell;
            }

            return path;
        }

        private List<(int, int)> GetKnightMoveCells(int number, int letter)
        {
            int[,] template = CreateTemplate();
            int moves = template[number, letter];
            List<(int, int)> cells = new List<(int, int)>();

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

        private (int, int) GetCoefficiants(Cell cellFrom, Cell cellTo)
        {
            
            //if (cellTo.Letter <= cellFrom.Letter && cellTo.Letter < 69)
            //{
            //    if (cellTo.Number <= cellFrom.Number && cellTo.Number < 5)
            //    {
            //        return (1, 1);
            //    }
            //    else
            //    {
            //        return (-1, 1);
            //    }
            //}
            //else
            //{
            //    if (cellTo.Number <= cellFrom.Number && cellTo.Number > 4)
            //    {
            //        return (1, -1);
            //    }
            //    else
            //    {
            //        return (-1, -1);
            //    }
            //}

            //if (cellTo.Letter <= cellFrom.Letter && cellTo.Number <= cellFrom.Number)
            //{
            //    return (1, 1);
            //}

            //if (cellTo.Letter > cellFrom.Letter && cellTo.Number <= cellFrom.Number)
            //{
            //    return (1, -1);
            //}

            //if (cellTo.Letter <= cellFrom.Letter && cellTo.Number > cellFrom.Number)
            //{
            //    return (-1, 1);
            //}

            //if (cellTo.Letter > cellFrom.Letter && cellTo.Number > cellFrom.Number)
            //{
            //    return (-1, -1);
            //}
            //return (0, 0);

            //if (cellTo.Letter <= 69)
            //{
            //    if (cellTo.Number >= 5 ||)
            //    {
            //        return (-1, 1);
            //    }
            //    else
            //    {
            //        return (1, 1);
            //    }
            //}
            //else 
            //{
            //    if (cellTo.Number >= 5)
            //    {
            //        return (-1, -1);
            //    }
            //    else
            //    {
            //        return (1, -1);
            //    }
            //}

            if (cellTo.Letter <= cellFrom.Letter)
            {
                if (cellTo.Number <= cellFrom.Number)
                {
                    return (1, 1);
                }
                else
                {
                    return (-1, 1);
                }
            }
            else 
            {
                if (cellTo.Number <= cellFrom.Number)
                {
                    return (1, -1);
                }
                else
                {
                    return (-1, -1);
                }
            }

        }
    }
}
