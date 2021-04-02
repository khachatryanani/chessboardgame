using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ChessBoard;
using ChessBoard.BoardAttributes;
using ChessBoard.Figures;
using static ChessBoard.ChessBoardManager;

namespace ChessBoardDesktop
{
    /// <summary>
    /// Manages the Chess board game by giving turns to User and Computer brain
    /// </summary>
    public class GameManager
    {
        // Define the turn to play by a color of currenlty playing figures
        public CellColor Turn { get; set; }

        public Dictionary<string, Grid> BoardOfDesktop { get; private set; }

        public Figure LastMoved { get; private set; }

        private char _algorithmChoice;

        public GameManager()
        {
            this.BoardOfDesktop = new Dictionary<string, Grid>();
            _algorithmChoice = '0';
        }

        /// <summary>
        /// Creates a Figure of given Type and Color and adds it to the Chess Board
        /// </summary>
        /// <param name="cell">Cell on which the created figure is standing</param>
        /// <param name="type">Type of the Figure to be created</param>
        /// <param name="color">Color of Figure to be created</param>
        public void CreateFigure(string cellString, string typeString, string colorString)
        {
            Type type = GetTypeFromString(typeString);
            CellColor color = GetCellColorFromString(colorString);
            Cell cell = new Cell(cellString);

            // Create a figure
            Figure figure = (Figure)Activator.CreateInstance(type, cell, color);

            // Add to ChessBoard
            AddFigure(figure);
        }
        private Type GetTypeFromString(string type) 
        {
            switch (type) 
            {
                case "Q":
                    return typeof(Queen);
                case "R":
                    return typeof(Rook);
                default:
                    return typeof(King);
                
            }
        }

        private CellColor GetCellColorFromString(string color) 
        {
            return color == "B" ? CellColor.Black : CellColor.White;
        }

        /// <summary>
        /// Get the possible moves of the figure standing on the given cell and returns the list of possible cell names
        /// </summary>
        /// <param name="cellString">String representation of Chess Board Cell on which the figure is standing</param>
        /// <returns>List of string representation of possible cells to move</returns>
        public List<string> GetPossibleMoves(string cellString)
        {
            Cell currentCell = new Cell(cellString);
            Figure figure = GetFigureByCell(currentCell);

            List<string> possibleCells = new List<string>();
            foreach (var cell in figure.InfluencedCells)
            {
                if (IsPossibleToMove(figure, cell) && figure.CurrentCell != cell)
                {
                    if ((figure is King))
                    {
                        if (!IsUnderCheckCell(cell, figure.Color))
                        {
                            possibleCells.Add(cell.ToString());
                        }
                    }
                    else
                    {
                        possibleCells.Add(cell.ToString());
                    }
                }
            }

            return possibleCells;
        }

        /// <summary>
        /// Does the move of the user by updating the Chess Board according the Move.
        /// </summary>
        /// <param name="cellFrom">Chess Board Cell from which the User moves.</param>
        /// <param name="cellTo">Chess Board Cell to which the User moves.</param>
        public void UserMove(string cellFrom, string cellTo)
        {
            // King of the user
            King king = GetTheKing(Turn);

            king.Move(new Cell(cellTo));
            UpdateBoard(new Cell(cellFrom), king.CurrentCell);

            this.Turn = CellColor.White;
            this.LastMoved = king;
        }

        /// <summary>
        /// Runs the winning algorithm and returns the current game situation: 0 = OK, 1 = Check, 2 = Mate, 3 = StaleMate
        /// </summary>
        /// <returns></returns>
        public int BrainMove()
        {
            if (!IsMate() && !IsStaleMate())
            {
                RunWinningAlgorithm();
            }
            if (IsMate())
            {
                return 2;
            }

            if (IsStaleMate())
            {
                return 3;
            }

            if (IsCheck())
            {
                return 1;
            }

            return 0;
        }

        /// <summary>
        /// After Computer Move updates the Chess Board and Desktop Board images according the Move.
        /// </summary>
        /// <param name="cellFrom">Chess Board Cell from which the figure moved</param>
        /// <param name="cellTo">Chess Board Cell to which the figure moves</param>
        private void UpdatePosition(Cell cellFrom, Cell cellTo)
        {
            UpdateBoard(cellFrom, cellTo);

            double top = BoardOfDesktop[cellTo.ToString()].Margin.Top;
            double left = BoardOfDesktop[cellTo.ToString()].Margin.Left;

            Figure figure = GetFigureByCell(cellTo);
            this.LastMoved = figure;

            Image image = BoardOfDesktop[cellFrom.ToString()].Tag as Image;
            BoardOfDesktop[cellFrom.ToString()].Tag = null;
            BoardOfDesktop[cellTo.ToString()].Tag = image;
           
            image.Margin = new Thickness(left, top, 0, 0);
            image.Tag = cellTo.ToString();
        }

        /// <summary>
        /// Runs the Computer's algorithm to win the end game with Queen and Rook
        /// </summary>
        private void RunWinningAlgorithm()
        {
            if (_algorithmChoice == '0')
            {
                _algorithmChoice = PressingRelativelyToWhiteKing();
            }

            if (!TryWinningWithQueenMove() && !TryWinningWithRookMove() && !TryWinningWithKingMove() && !TrySavingRook())
            {
                switch (_algorithmChoice)
                {
                    case 'D':
                        PressingDown();
                        break;
                    case 'U':
                        PressingUp();
                        break;
                    case 'L':
                        PressingLeft();
                        break;
                    case 'R':
                        PressingRight();
                        break;
                    default:
                        break;
                }
            }

        }

        /// <summary>
        /// Check the positing of white and black kings to define the better algorithm for pressing black king: opposite side of white king
        /// </summary>
        /// <returns>'D' if PressingDown is chosen, 'U' if PressingUp is chosen, 'L' if PressingLeft is chosen, 'R' if PressingRight is chosen</returns>
        private char PressingRelativelyToWhiteKing()
        {
            CellColor oppositeColor = Turn == CellColor.White ? CellColor.Black : CellColor.White;

            King oppositeKing = GetTheKing(oppositeColor);
            Cell oppositeKingCell = oppositeKing.CurrentCell;

            King king = GetTheKing(Turn);
            Cell kingCell = king.CurrentCell;


            if (oppositeKingCell.Letter > kingCell.Letter + 1)
            {
                return 'L';
            }
            else if (oppositeKingCell.Letter < kingCell.Letter - 1)
            {
                return 'R';
            }
            else if (oppositeKingCell.Number > kingCell.Number + 1 &&
                    oppositeKingCell.Letter >= kingCell.Letter - 1 &&
                    oppositeKingCell.Letter <= kingCell.Letter + 1)
            {
                return 'D';
            }
            else
            {
                return 'U';
            }

        }

        /// <summary>
        /// Algoithm moves queen and rook in turn to keep pressing king to the border until it the Mate.
        /// </summary>
        /// <returns>True if moved, False if no case mathced</returns>
        private bool PressingDown()
        {
            CellColor oppositeColor = Turn == CellColor.White ? CellColor.Black : CellColor.White;
            King oppositeKing = GetTheKing(oppositeColor);

            King king = GetTheKing(Turn);

            Queen queen = GetFigure(typeof(Queen), Turn) as Queen;
            Cell queenCellFrom = queen.CurrentCell;

            Rook rook = GetFigure(typeof(Rook), Turn) as Rook;
            Cell rookCellFrom = rook.CurrentCell;

            Cell rookCellTo;
            //if queen is its correct start place
            if (queenCellFrom.Number == oppositeKing.CurrentCell.Number - 1)
            {
                rookCellTo = new Cell(rookCellFrom.Letter, oppositeKing.CurrentCell.Number);
                //if rook is safe to move down
                if (Math.Abs(rookCellFrom.Letter - oppositeKing.CurrentCell.Letter) >= 2
                    || IsPossibleToMove(queen, rookCellTo) || IsPossibleToMove(king, rookCellTo))
                {
                    if (IsPossibleToMove(rook, rookCellTo) && IsFreeCell(rookCellTo))
                    {
                        rook.Move(rookCellTo);
                        UpdatePosition(rookCellFrom, rookCellTo);
                        Turn = oppositeColor;
                        return true;
                    }
                }
                // moves away of king
                else
                {
                    if (oppositeKing.CurrentCell.Letter >= 69)
                    {

                        rookCellTo = new Cell(queenCellFrom.Letter == 65 ? (char)(queenCellFrom.Letter + 1) : 'A', rookCellFrom.Number);

                    }
                    else
                    {
                        rookCellTo = new Cell(queenCellFrom.Letter == 72 ? (char)(queenCellFrom.Letter - 1) : 'H', rookCellFrom.Number);
                    }

                    if (!(IsPossibleToMove(rook, rookCellTo) && IsFreeCell(rookCellTo)))
                    {
                        rookCellTo = new Cell(rookCellFrom.Letter, rookCellFrom.Number + 1);
                    }

                    rook.Move(rookCellTo);
                    UpdatePosition(rookCellFrom, rookCellTo);
                    Turn = oppositeColor;
                    return true;
                }

            }
            else if (rookCellFrom.Number == oppositeKing.CurrentCell.Number - 1)
            {
                // Place the queen on th esame line as king
                foreach (var queenCellTo in queen.InfluencedCells)
                {
                    if (queenCellTo.Number == oppositeKing.CurrentCell.Number &&
                        (Math.Abs(queenCellTo.Letter - oppositeKing.CurrentCell.Letter) >= 2 ||
                        IsPossibleToMove(king, queenCellTo) || IsPossibleToMove(rook, queenCellTo)))
                    {
                        if (IsPossibleToMove(queen, queenCellTo) && IsFreeCell(queenCellTo))
                        {
                            queen.Move(queenCellTo);
                            UpdatePosition(queenCellFrom, queenCellTo);
                            Turn = oppositeColor;
                            return true;
                        }
                    }
                }
            }
            else
            {
                // Place the queen above the king
                foreach (var queenCellTo in queen.InfluencedCells)
                {
                    if (queenCellTo.Number == oppositeKing.CurrentCell.Number - 1 &&
                        (Math.Abs(queenCellTo.Letter - oppositeKing.CurrentCell.Letter) >= 2 ||
                        IsPossibleToMove(king, queenCellTo) || IsPossibleToMove(rook, queenCellTo)))
                    {
                        if (IsPossibleToMove(queen, queenCellTo) && IsFreeCell(queenCellTo))
                        {
                            queen.Move(queenCellTo);

                            UpdatePosition(queenCellFrom, queenCellTo);
                            Turn = oppositeColor;
                            if (IsStaleMate())
                            {
                                queen.Move(queenCellFrom);
                                UpdatePosition(queenCellTo, queenCellFrom);
                                Turn = queen.Color;
                                continue;
                            }
                            return true;
                        }
                    }
                }

            }

            return TryMovingKing() || TryMovingQueen();
        }

        /// <summary>
        /// Algoithm moves queen and rook in turn to keep pressing king to the border until it the Mate.
        /// </summary>
        /// <returns>True if moved, False if no case mathced</returns>
        private bool PressingUp()
        {
            CellColor oppositeColor = Turn == CellColor.White ? CellColor.Black : CellColor.White;
            King oppositeKing = GetTheKing(oppositeColor);

            King king = GetTheKing(Turn);

            Queen queen = GetFigure(typeof(Queen), Turn) as Queen;
            Cell queenCellFrom = queen.CurrentCell;

            Rook rook = GetFigure(typeof(Rook), Turn) as Rook;
            Cell rookCellFrom = rook.CurrentCell;

            Cell rookCellTo;
            //if queen is its correct start place
            if (queenCellFrom.Number == oppositeKing.CurrentCell.Number + 1)
            {
                rookCellTo = new Cell(rookCellFrom.Letter, oppositeKing.CurrentCell.Number);
                //if rook is safe to move up
                if (Math.Abs(rookCellFrom.Letter - oppositeKing.CurrentCell.Letter) >= 2
                    || IsPossibleToMove(queen, rookCellTo) || IsPossibleToMove(king, rookCellTo))
                {
                    if (IsPossibleToMove(rook, rookCellTo) && IsFreeCell(rookCellTo))
                    {
                        rook.Move(rookCellTo);
                        UpdatePosition(rookCellFrom, rookCellTo);
                        Turn = oppositeColor;
                        return true;
                    }
                }
                // moves around queen
                else
                {
                    if (oppositeKing.CurrentCell.Letter >= 69)
                    {

                        rookCellTo = new Cell(queenCellFrom.Letter == 65 ? (char)(queenCellFrom.Letter + 1) : 'A', rookCellFrom.Number);

                    }
                    else
                    {
                        rookCellTo = new Cell(queenCellFrom.Letter == 72 ? (char)(queenCellFrom.Letter - 1) : 'H', rookCellFrom.Number);
                    }


                    if (!(IsPossibleToMove(rook, rookCellTo) && IsFreeCell(rookCellTo)))
                    {
                        rookCellTo = new Cell(rookCellFrom.Letter, rookCellFrom.Number - 1);
                    }

                    rook.Move(rookCellTo);
                    UpdatePosition(rookCellFrom, rookCellTo);
                    Turn = oppositeColor;
                    return true;
                }
            }
            else if (rookCellFrom.Number == oppositeKing.CurrentCell.Number + 1)
            {
                // Place the king on the same line as the king
                foreach (var queenCellTo in queen.InfluencedCells)
                {
                    if (queenCellTo.Number == oppositeKing.CurrentCell.Number &&
                        (Math.Abs(queenCellTo.Letter - oppositeKing.CurrentCell.Letter) >= 2 ||
                        IsPossibleToMove(king, queenCellTo) || IsPossibleToMove(rook, queenCellTo)))
                    {
                        if (IsPossibleToMove(queen, queenCellTo) && IsFreeCell(queenCellTo))
                        {
                            queen.Move(queenCellTo);
                            UpdatePosition(queenCellFrom, queenCellTo);
                            Turn = oppositeColor;
                            return true;
                        }
                    }
                }
            }
            else
            {
                // Place the queen above the king
                foreach (var queenCellTo in queen.InfluencedCells)
                {
                    if (queenCellTo.Number == oppositeKing.CurrentCell.Number + 1 &&
                        (Math.Abs(queenCellTo.Letter - oppositeKing.CurrentCell.Letter) >= 2 ||
                        IsPossibleToMove(king, queenCellTo) || IsPossibleToMove(rook, queenCellTo)))
                    {
                        if (IsPossibleToMove(queen, queenCellTo) && IsFreeCell(queenCellTo))
                        {
                            queen.Move(queenCellTo);

                            UpdatePosition(queenCellFrom, queenCellTo);
                            Turn = oppositeColor;
                            if (IsStaleMate())
                            {
                                queen.Move(queenCellFrom);
                                UpdatePosition(queenCellTo, queenCellFrom);
                                Turn = queen.Color;
                                continue;
                            }
                            return true;
                        }
                    }
                }

            }

            return TryMovingKing() || TryMovingQueen();
        }

        /// <summary>
        /// Algoithm moves queen and rook in turn to keep pressing king to the border until it the Mate.
        /// </summary>
        /// <returns>True if moved, False if no case mathced</returns>
        private bool PressingLeft()
        {
            CellColor oppositeColor = Turn == CellColor.White ? CellColor.Black : CellColor.White;
            King oppositeKing = GetTheKing(oppositeColor);

            King king = GetTheKing(Turn);

            Queen queen = GetFigure(typeof(Queen), Turn) as Queen;
            Cell queenCellFrom = queen.CurrentCell;

            Rook rook = GetFigure(typeof(Rook), Turn) as Rook;
            Cell rookCellFrom = rook.CurrentCell;

            Cell rookCellTo;
            //if queen is its correct start place
            if (queenCellFrom.Letter == oppositeKing.CurrentCell.Letter - 1)
            {
                //if rook is safe to move left
                rookCellTo = new Cell(oppositeKing.CurrentCell.Letter, rookCellFrom.Number);
                //if rook is safe to move up
                if (Math.Abs(rookCellFrom.Number - oppositeKing.CurrentCell.Number) >= 2
                    || IsPossibleToMove(queen, rookCellTo) || IsPossibleToMove(king, rookCellTo))
                {
                    if (IsPossibleToMove(rook, rookCellTo) && IsFreeCell(rookCellTo))
                    {
                        rook.Move(rookCellTo);
                        UpdatePosition(rookCellFrom, rookCellTo);
                        Turn = oppositeColor;
                        return true;
                    }
                }
                // moves around queen
                else
                {
                    if (oppositeKing.CurrentCell.Number >= 5)
                    {
                        rookCellTo = new Cell(rookCellFrom.Letter, queenCellFrom.Number == 1 ? queenCellFrom.Number + 1 : 1);
                    }
                    else
                    {
                        rookCellTo = new Cell(rookCellFrom.Letter, queenCellFrom.Number == 8 ? queenCellFrom.Number - 1 : 8);
                    }

                    if (!(IsPossibleToMove(rook, rookCellTo) && IsFreeCell(rookCellTo)))
                    {
                        rookCellTo = new Cell((char)(rookCellFrom.Letter + 1), rookCellFrom.Number);
                    }

                    rook.Move(rookCellTo);
                    UpdatePosition(rookCellFrom, rookCellTo);
                    Turn = oppositeColor;
                    return true;
                }
            }
            else if (rookCellFrom.Letter == oppositeKing.CurrentCell.Letter - 1)
            {
                // Place the queen on the same line as king
                foreach (var queenCellTo in queen.InfluencedCells)
                {
                    if (queenCellTo.Letter == oppositeKing.CurrentCell.Letter &&
                        (Math.Abs(queenCellTo.Number - oppositeKing.CurrentCell.Number) >= 2 ||
                        IsPossibleToMove(king, queenCellTo) || IsPossibleToMove(rook, queenCellTo)))
                    {
                        if (IsPossibleToMove(queen, queenCellTo) && IsFreeCell(queenCellTo))
                        {
                            queen.Move(queenCellTo);
                            UpdatePosition(queenCellFrom, queenCellTo);
                            Turn = oppositeColor;
                            return true;
                        }
                    }
                }
            }
            else
            {
                // Place the queen above the king
                foreach (var queenCellTo in queen.InfluencedCells)
                {
                    if (queenCellTo.Letter == oppositeKing.CurrentCell.Letter - 1 &&
                        (Math.Abs(queenCellTo.Number - oppositeKing.CurrentCell.Number) >= 2 ||
                        IsPossibleToMove(king, queenCellTo) || IsPossibleToMove(rook, queenCellTo)))
                    {
                        if (IsPossibleToMove(queen, queenCellTo) && IsFreeCell(queenCellTo))
                        {
                            queen.Move(queenCellTo);

                            UpdatePosition(queenCellFrom, queenCellTo);
                            Turn = oppositeColor;
                            if (IsStaleMate())
                            {
                                queen.Move(queenCellFrom);
                                UpdatePosition(queenCellTo, queenCellFrom);
                                Turn = queen.Color;
                                continue;
                            }
                            return true;
                        }
                    }
                }

            }

            return TryMovingKing() || TryMovingQueen();
        }

        /// <summary>
        /// Algoithm moves queen and rook in turn to keep pressing king to the border until it the Mate.
        /// </summary>
        /// <returns>True if moved, False if no case mathced</returns>
        private bool PressingRight()
        {
            CellColor oppositeColor = Turn == CellColor.White ? CellColor.Black : CellColor.White;
            King oppositeKing = GetTheKing(oppositeColor);

            King king = GetTheKing(Turn);

            Queen queen = GetFigure(typeof(Queen), Turn) as Queen;
            Cell queenCellFrom = queen.CurrentCell;

            Rook rook = GetFigure(typeof(Rook), Turn) as Rook;
            Cell rookCellFrom = rook.CurrentCell;

            Cell rookCellTo;
            //if queen is its correct start place
            if (queenCellFrom.Letter == oppositeKing.CurrentCell.Letter + 1)
            {
                //if rook is safe to move right
                rookCellTo = new Cell(oppositeKing.CurrentCell.Letter, rookCellFrom.Number);
                if (Math.Abs(rookCellFrom.Number - oppositeKing.CurrentCell.Number) >= 2
                    || IsPossibleToMove(queen, rookCellTo) || IsPossibleToMove(king, rookCellTo))
                {
                    if (IsPossibleToMove(rook, rookCellTo) && IsFreeCell(rookCellTo))
                    {
                        rook.Move(rookCellTo);
                        UpdatePosition(rookCellFrom, rookCellTo);
                        Turn = oppositeColor;
                        return true;
                    }
                }
                // moves around queen
                else
                {
                    if (oppositeKing.CurrentCell.Number >= 5)
                    {
                        rookCellTo = new Cell(rookCellFrom.Letter, queenCellFrom.Number == 1 ? queenCellFrom.Number + 1 : 1);
                    }
                    else
                    {
                        rookCellTo = new Cell(rookCellFrom.Letter, queenCellFrom.Number == 8 ? queenCellFrom.Number - 1 : 8);
                    }

                    if (!(IsPossibleToMove(rook, rookCellTo) && IsFreeCell(rookCellTo)))
                    {
                        rookCellTo = new Cell((char)(rookCellFrom.Letter - 1), rookCellFrom.Number);
                    }

                    rook.Move(rookCellTo);
                    UpdatePosition(rookCellFrom, rookCellTo);
                    Turn = oppositeColor;
                    return true;

                }
            }
            else if (rookCellFrom.Letter == oppositeKing.CurrentCell.Letter + 1)
            {
                // Place the queen on the same line as king
                foreach (var queenCellTo in queen.InfluencedCells)
                {
                    if (queenCellTo.Letter == oppositeKing.CurrentCell.Letter &&
                        (Math.Abs(queenCellTo.Number - oppositeKing.CurrentCell.Number) >= 2 ||
                        IsPossibleToMove(king, queenCellTo) || IsPossibleToMove(rook, queenCellTo)))
                    {
                        if (IsPossibleToMove(queen, queenCellTo) && IsFreeCell(queenCellTo))
                        {
                            queen.Move(queenCellTo);
                            UpdatePosition(queenCellFrom, queenCellTo);
                            Turn = oppositeColor;
                            return true;
                        }
                    }
                }
            }
            else
            {
                // Place the queen above the king
                foreach (var queenCellTo in queen.InfluencedCells)
                {
                    if (queenCellTo.Letter == oppositeKing.CurrentCell.Letter + 1 &&
                        (Math.Abs(queenCellTo.Number - oppositeKing.CurrentCell.Number) >= 2 ||
                        IsPossibleToMove(king, queenCellTo) || IsPossibleToMove(rook, queenCellTo)))
                    {
                        if (IsPossibleToMove(queen, queenCellTo) && IsFreeCell(queenCellTo))
                        {
                            queen.Move(queenCellTo);

                            UpdatePosition(queenCellFrom, queenCellTo);
                            Turn = oppositeColor;
                            if (IsStaleMate())
                            {
                                queen.Move(queenCellFrom);
                                UpdatePosition(queenCellTo, queenCellFrom);
                                Turn = queen.Color;
                                continue;
                            }
                            return true;
                        }
                    }
                }

            }

            return TryMovingKing() || TryMovingQueen();
        }

        /// <summary>
        /// Move the queen to all its possible cells to check if there will be a Mate situation on one of them
        /// </summary>
        /// <returns>True if moveing queen can result in Mate, False if no Mate situation is possible for this positing</returns>
        private bool TryWinningWithQueenMove()
        {
            CellColor playerColor = Turn;
            CellColor oppositeColor = Turn == CellColor.White ? CellColor.Black : CellColor.White;

            King oppositeKing = GetTheKing(oppositeColor);
            Queen queen = GetFigure(typeof(Queen), Turn) as Queen;
            Rook rook = GetFigure(typeof(Rook), Turn) as Rook;
            King king = GetTheKing(Turn);

            Cell cellFrom = queen.CurrentCell;
            List<Cell> currentInfuencedCells = queen.InfluencedCells;

            foreach (var cellTo in currentInfuencedCells)
            {
                if (IsPossibleToMove(queen, cellTo) && IsFreeCell(cellTo) &&
                    ((Math.Abs(cellTo.Number - oppositeKing.CurrentCell.Number) >= 2 && Math.Abs(cellTo.Letter - oppositeKing.CurrentCell.Letter) >= 2)
                        || IsPossibleToMove(rook, cellTo) || IsPossibleToMove(king, cellTo)))
                {
                    queen.Move(cellTo);
                    Turn = oppositeColor;
                    if (IsMate())
                    {
                        UpdatePosition(cellFrom, cellTo);

                        return true;
                    }
                    else
                    {
                        queen.Move(cellFrom);
                        Turn = playerColor;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Move the rook to all its possible cells to check if there will be a Mate situation on one of them
        /// </summary>
        /// <returns>True if moveing rook can result in Mate, False if no Mate situation is possible for this positing</returns>
        private bool TryWinningWithRookMove()
        {
            CellColor oppositeColor = Turn == CellColor.White ? CellColor.Black : CellColor.White;

            King oppositeKing = GetTheKing(oppositeColor);
            Queen queen = GetFigure(typeof(Queen), Turn) as Queen;
            Rook rook = GetFigure(typeof(Rook), Turn) as Rook;
            King king = GetTheKing(Turn);
            Cell cellFrom = rook.CurrentCell;

            foreach (var cellTo in rook.InfluencedCells)
            {
                if (IsPossibleToMove(rook, cellTo) && IsFreeCell(cellTo) &&
                   ((Math.Abs(cellTo.Number - oppositeKing.CurrentCell.Number) >= 2 && Math.Abs(cellTo.Letter - oppositeKing.CurrentCell.Letter) >= 2)
                       || IsPossibleToMove(queen, cellTo) || IsPossibleToMove(king, cellTo)))
                {
                    rook.Move(cellTo);
                    Turn = oppositeColor;
                    if (IsMate())
                    {
                        UpdatePosition(cellFrom, cellTo);
                        return true;
                    }
                    else
                    {
                        rook.Move(cellFrom);
                        Turn = Turn == CellColor.White ? CellColor.Black : CellColor.White;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Move the king to all its possible cells to check if there will be a Mate situation on one of them
        /// </summary>
        /// <returns>True if moveing rook can result in Mate, False if no Mate situation is possible for this positing</returns>
        private bool TryWinningWithKingMove()
        {
            CellColor oppositeColor = Turn == CellColor.White ? CellColor.Black : CellColor.White;

            King oppositeKing = GetTheKing(oppositeColor);
            Queen queen = GetFigure(typeof(Queen), Turn) as Queen;
            Rook rook = GetFigure(typeof(Rook), Turn) as Rook;
            King king = GetTheKing(Turn);
            Cell cellFrom = king.CurrentCell;

            foreach (var cellTo in king.InfluencedCells)
            {
                if (IsPossibleToMove(king, cellTo) && IsFreeCell(cellTo) &&
                   ((Math.Abs(cellTo.Number - oppositeKing.CurrentCell.Number) >= 2 && Math.Abs(cellTo.Letter - oppositeKing.CurrentCell.Letter) >= 2)
                       || IsPossibleToMove(queen, cellTo) || IsPossibleToMove(rook, cellTo)))
                {
                    king.Move(cellTo);

                    Turn = oppositeColor;
                    if (IsMate())
                    {
                        UpdatePosition(cellFrom, cellTo);
                        return true;
                    }
                    else
                    {
                        king.Move(cellFrom);
                        Turn = Turn == CellColor.White ? CellColor.Black : CellColor.White;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Checks if there is a need to move thr Rook to save it from opposite King
        /// </summary>
        /// <returns>True if moved, False if no need to move</returns>
        private bool TrySavingRook()
        {
            CellColor oppositeColor = Turn == CellColor.White ? CellColor.Black : CellColor.White;
            King oppositeKing = GetTheKing(oppositeColor);

            Queen queen = GetFigure(typeof(Queen), Turn) as Queen;
            Rook rook = GetFigure(typeof(Rook), Turn) as Rook;
            King king = GetTheKing(Turn);
            Cell rookCellFrom = rook.CurrentCell;

            for (int i = oppositeKing.CurrentCell.Letter - 1; i <= oppositeKing.CurrentCell.Letter + 1; i++)
            {
                for (int j = oppositeKing.CurrentCell.Number - 1; j <= oppositeKing.CurrentCell.Number + 1; j++)
                {
                    if (rookCellFrom.Letter == i && rookCellFrom.Number == j &&
                        !IsPossibleToMove(queen, rookCellFrom) &&
                        !IsPossibleToMove(king, rookCellFrom))
                    {
                        Cell rookCellTo;
                        switch (_algorithmChoice)
                        {
                            case 'U':
                            case 'D':
                                if (oppositeKing.CurrentCell.Letter >= 69)
                                {
                                    char letterTo = queen.CurrentCell.Letter == 'A' ? 'B' : 'A';
                                    rookCellTo = new Cell(letterTo, rookCellFrom.Number);
                                    if (!(IsPossibleToMove(rook, rookCellTo) && IsFreeCell(rookCellTo)))
                                    {
                                        rookCellTo = new Cell((char)(king.CurrentCell.Letter + 1), rookCellFrom.Number);
                                    }

                                }
                                else
                                {
                                    char letterTo = queen.CurrentCell.Letter == 'H' ? 'G' : 'H';
                                    rookCellTo = new Cell(letterTo, rookCellFrom.Number);
                                    if (!(IsPossibleToMove(rook, rookCellTo) && IsFreeCell(rookCellTo)))
                                    {
                                        rookCellTo = new Cell((char)(king.CurrentCell.Letter - 1), rookCellFrom.Number);

                                    }
                                }
                                break;
                            default:

                                if (oppositeKing.CurrentCell.Number >= 5)
                                {
                                    int numberTo = queen.CurrentCell.Number == 1 ? 2 : 1;
                                    rookCellTo = new Cell(rookCellFrom.Letter, numberTo);
                                    if (!(IsPossibleToMove(rook, rookCellTo) && IsFreeCell(rookCellTo)))
                                    {
                                        rookCellTo = new Cell(rookCellFrom.Letter, king.CurrentCell.Number + 1);

                                    }
                                }
                                else
                                {
                                    int numberTo = queen.CurrentCell.Number == 8 ? 7 : 8;
                                    rookCellTo = new Cell(rookCellFrom.Letter, numberTo);
                                    if (!(IsPossibleToMove(rook, rookCellTo) && IsFreeCell(rookCellTo)))
                                    {
                                        rookCellTo = new Cell(rookCellFrom.Letter, king.CurrentCell.Number - 1);

                                    }
                                }
                                break;
                        }

                        rook.Move(rookCellTo);
                        UpdatePosition(rookCellFrom, rookCellTo);
                        Turn = oppositeColor;
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Moves with the King vertically or horizontally depending on the chosen algorithm
        /// </summary>
        /// <returns></returns>
        private bool TryMovingKing()
        {
            CellColor oppositeColor = Turn == CellColor.White ? CellColor.Black : CellColor.White;

            King king = GetTheKing(Turn);
            Cell kingCellFrom = king.CurrentCell;
            Rook rook = GetFigure(typeof(Rook), Turn) as Rook;


            switch (_algorithmChoice)
            {
                case 'U':
                case 'D':
                    Cell kingCellTo;
                    if (kingCellFrom.Letter == rook.CurrentCell.Letter)
                    {
                        if (kingCellFrom.Letter >= 69)
                        {
                            kingCellTo = new Cell((char)(kingCellFrom.Letter - 1), kingCellFrom.Number);

                        }
                        else
                        {
                            kingCellTo = new Cell((char)(kingCellFrom.Letter + 1), kingCellFrom.Number);
                        }
                        if (IsPossibleToMove(king, kingCellTo) && IsFreeCell(kingCellTo))
                        {
                            king.Move(kingCellTo);
                            UpdatePosition(kingCellFrom, kingCellTo);
                            Turn = oppositeColor;
                            return true;
                        }

                    }
                    break;
                case 'L':
                case 'R':
                    if (kingCellFrom.Number == rook.CurrentCell.Number)
                    {
                        if (kingCellFrom.Number >= 5)
                        {
                            kingCellTo = new Cell(kingCellFrom.Letter, kingCellFrom.Number - 1);

                        }
                        else
                        {
                            kingCellTo = new Cell(kingCellFrom.Letter, kingCellFrom.Number + 1);
                        }

                        if (IsPossibleToMove(king, kingCellTo) && IsFreeCell(kingCellTo))
                        {
                            king.Move(kingCellTo);
                            UpdatePosition(kingCellFrom, kingCellTo);
                            Turn = oppositeColor;
                            return true;
                        }
                    }
                    break;
            }

            return false;
        }

        /// <summary>
        /// Moves with the Queen vertically or horizontally depending on the chosen algorithm
        /// </summary>
        /// <returns></returns>
        private bool TryMovingQueen()
        {
            CellColor oppositeColor = Turn == CellColor.White ? CellColor.Black : CellColor.White;

            King oppositeKing = GetTheKing(oppositeColor);
            Cell kingCellFrom = oppositeKing.CurrentCell;

            Queen queen = GetFigure(typeof(Queen), Turn) as Queen;
            Cell queenCellFrom = queen.CurrentCell;
            switch (_algorithmChoice)
            {
                case 'U':
                case 'D':
                    foreach (var queenCellTo in queen.InfluencedCells)
                    {
                        if (queenCellTo.Number == queenCellFrom.Number &&
                            Math.Abs(queenCellTo.Letter - kingCellFrom.Letter) >= 2 &&
                            queenCellTo != queenCellFrom)
                        {
                            if (IsPossibleToMove(queen, queenCellTo) && IsFreeCell(queenCellTo))
                            {
                                queen.Move(queenCellTo);
                                UpdatePosition(queenCellFrom, queenCellTo);
                                Turn = oppositeColor;
                                return true;
                            }
                        }
                    }
                    break;
                case 'L':
                case 'R':
                    foreach (var queenCellTo in queen.InfluencedCells)
                    {
                        if (queenCellTo.Letter == queenCellFrom.Letter &&
                            Math.Abs(queenCellTo.Number - kingCellFrom.Number) >= 2 &&
                            queenCellTo != queenCellFrom)
                        {
                            if (IsPossibleToMove(queen, queenCellTo) && IsFreeCell(queenCellTo))
                            {
                                queen.Move(queenCellTo);
                                UpdatePosition(queenCellFrom, queenCellTo);
                                Turn = oppositeColor;
                                return true;
                            }
                        }
                    }
                    break;
            }

            if (queenCellFrom.Letter == 'A' || queenCellFrom.Letter == 'H')
            {
                if (queenCellFrom.Number == 1 || queenCellFrom.Number == 8)
                {
                    foreach (var queenCellTo in queen.InfluencedCells)
                    {
                        if (Math.Abs(queenCellTo.Number - queenCellFrom.Number) == 1 &&
                            Math.Abs(queenCellTo.Letter - queenCellFrom.Letter) == 1)
                        {
                            if (IsPossibleToMove(queen, queenCellTo) && IsFreeCell(queenCellTo))
                            {
                                queen.Move(queenCellTo);
                                UpdatePosition(queenCellFrom, queenCellTo);
                                Turn = oppositeColor;
                                return true;
                            }
                        }
                    }
                }
            }

            return false;
        }

        /// Checks if it is a Mate situation in the game:
        /// if the King is under influence of opposite color figure and has no available cells to move to
        /// </summary>
        /// <returns>True if it is a Mate, False if it is not a Mate</returns>
        private bool IsMate()
        {
            // Get the King Figure
            King king = GetTheKing(Turn);

            if (IsUnderCheckCell(king.CurrentCell, king.Color))
            {
                foreach (var item in king.InfluencedCells)
                {
                    if (!IsInfluencedCell(item, king.Color) && IsFreeCell(item))
                    {
                        return false;
                    }
                }


                // Print winner side name

                return true;
            }
            return false;
        }

        /// <summary>
        /// Verifies if the king of the current player is/will be under Chess Check
        /// </summary>
        /// <returns>True if the king is under Chess Check, False if it does not</returns>
        private bool IsCheck()
        {

            // Get the King Figure
            King king = GetTheKing(Turn);

            // Check if it is on cell under influence
            if (IsUnderCheckCell(king.CurrentCell, king.Color))
            {

                return true;
            }

            return false;
        }

        /// <summary>
        /// Check if it is a stalemate situation in the game
        /// </summary>
        /// <returns>True if it is a stalemate, False if it is not</returns>
        private bool IsStaleMate()
        {
            King king = GetTheKing(Turn);

            if (!IsUnderCheckCell(king.CurrentCell, king.Color))
            {
                foreach (var item in king.InfluencedCells)
                {
                    if (!IsUnderCheckCell(item, king.Color) && item != king.CurrentCell)
                    {
                        return false;
                    }
                }


                return true;
            }
            return false;

        }
    }
}
