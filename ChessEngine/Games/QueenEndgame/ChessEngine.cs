﻿using ChessGame;
using System;
using System.Collections.Generic;

namespace ChessEngineLogic
{
    public partial class ChessEngine
    {
        /// <summary>
        /// Algorithm choice for Queen End Game
        /// </summary>
        private char _algorithmChoice = '0';
       
        /// <summary>
        /// Runs the Computer's algorithm to win the end game with Queen and Rook
        /// </summary>
        public void PlayWinningWithQueenAndRookAlgorithm()
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
            var turn = _boardManager.Turn ? Color.White : Color.Black;
            var oppositeColor = _boardManager.Turn ? Color.Black : Color.White;

            var oppositeKing = _boardManager.GetTheKing(oppositeColor);
            var oppositeKingCell = oppositeKing.CurrentCell;

            var king = _boardManager.GetTheKing(turn);
            var kingCell = king.CurrentCell;


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
        private void PressingDown()
        {
            var turn = _boardManager.Turn ? Color.White : Color.Black;

            Color oppositeColor = turn == Color.White ? Color.Black : Color.White;
            King oppositeKing = _boardManager.GetTheKing(oppositeColor);

            King king = _boardManager.GetTheKing(turn);

            Queen queen = _boardManager.GetFigure(typeof(Queen), turn) as Queen;
            Cell queenCellFrom = queen.CurrentCell;

            Rook rook = _boardManager.GetFigure(typeof(Rook), turn) as Rook;
            Cell rookCellFrom = rook.CurrentCell;

            Cell rookCellTo;
            //if queen is its correct start place
            if (queenCellFrom.Number == oppositeKing.CurrentCell.Number - 1)
            {
                rookCellTo = new Cell(rookCellFrom.Letter, oppositeKing.CurrentCell.Number);
                //if rook is safe to move down
                if (Math.Abs(rookCellFrom.Letter - oppositeKing.CurrentCell.Letter) >= 2
                    || _boardManager.IsPossibleToMove(queen, rookCellTo) || _boardManager.IsPossibleToMove(king, rookCellTo))
                {
                    if (_boardManager.IsPossibleToMove(rook, rookCellTo) && _boardManager.IsFreeCell(rookCellTo))
                    {
                        rook.Move(rookCellTo);
                        _boardManager.ChangeTurn();
                        _boardManager.UpdatePosition(rook, rookCellFrom, rookCellTo);
                        return;
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

                    if (!(_boardManager.IsPossibleToMove(rook, rookCellTo) && _boardManager.IsFreeCell(rookCellTo)))
                    {
                        rookCellTo = new Cell(rookCellFrom.Letter, rookCellFrom.Number + 1);
                    }

                    rook.Move(rookCellTo);
                    _boardManager.ChangeTurn();
                    _boardManager.UpdatePosition(rook, rookCellFrom, rookCellTo);
                    return;
                }

            }
            else if (rookCellFrom.Number == oppositeKing.CurrentCell.Number - 1)
            {
                // Place the queen on the same line as king
                foreach (var queenCellTo in queen.InfluencedCells)
                {
                    if (queenCellTo.Number == oppositeKing.CurrentCell.Number &&
                        (Math.Abs(queenCellTo.Letter - oppositeKing.CurrentCell.Letter) >= 2 ||
                        _boardManager.IsPossibleToMove(king, queenCellTo) || _boardManager.IsPossibleToMove(rook, queenCellTo)) &&
                        _boardManager.IsPossibleToMove(queen, queenCellTo) && _boardManager.IsFreeCell(queenCellTo))
                    {
                        queen.Move(queenCellTo);
                        _boardManager.ChangeTurn();
                        _boardManager.UpdatePosition(queen, queenCellFrom, queenCellTo);
                        return;
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
                        _boardManager.IsPossibleToMove(king, queenCellTo) || _boardManager.IsPossibleToMove(rook, queenCellTo)) &&
                        _boardManager.IsPossibleToMove(queen, queenCellTo) && _boardManager.IsFreeCell(queenCellTo))
                    {
                        queen.Move(queenCellTo);

                        _boardManager.ChangeTurn();
                        _boardManager.UpdatePosition(queen, queenCellFrom, queenCellTo);
                        if (_boardManager.IsStaleMate())
                        {
                            queen.Move(queenCellFrom);
                            _boardManager.Turn = queen.Color == Color.White;
                            _boardManager.UpdatePosition(queen, queenCellTo, queenCellFrom);
                            continue;
                        }
                        return;
                    }
                }
            }

            if (!TryMovingKing())
            {
                TryMovingQueen();
            }
        }

        /// <summary>
        /// Algoithm moves queen and rook in turn to keep pressing king to the border until it the Mate.
        /// </summary>
        /// <returns>True if moved, False if no case mathced</returns>
        private void PressingUp()
        {
            var turn = _boardManager.Turn ? Color.White : Color.Black;

            Color oppositeColor = turn == Color.White ? Color.Black : Color.White;
            King oppositeKing = _boardManager.GetTheKing(oppositeColor);

            King king = _boardManager.GetTheKing(turn);

            Queen queen = _boardManager.GetFigure(typeof(Queen), turn) as Queen;
            Cell queenCellFrom = queen.CurrentCell;

            Rook rook = _boardManager.GetFigure(typeof(Rook), turn) as Rook;
            Cell rookCellFrom = rook.CurrentCell;

            Cell rookCellTo;
            //if queen is its correct start place
            if (queenCellFrom.Number == oppositeKing.CurrentCell.Number + 1)
            {
                rookCellTo = new Cell(rookCellFrom.Letter, oppositeKing.CurrentCell.Number);
                //if rook is safe to move up
                if (Math.Abs(rookCellFrom.Letter - oppositeKing.CurrentCell.Letter) >= 2
                    || _boardManager.IsPossibleToMove(queen, rookCellTo) || _boardManager.IsPossibleToMove(king, rookCellTo))
                {
                    if (_boardManager.IsPossibleToMove(rook, rookCellTo) && _boardManager.IsFreeCell(rookCellTo))
                    {
                        rook.Move(rookCellTo);
                        _boardManager.ChangeTurn();
                        _boardManager.UpdatePosition(rook, rookCellFrom, rookCellTo);
                        return;
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


                    if (!(_boardManager.IsPossibleToMove(rook, rookCellTo) && _boardManager.IsFreeCell(rookCellTo)))
                    {
                        rookCellTo = new Cell(rookCellFrom.Letter, rookCellFrom.Number - 1);
                    }

                    rook.Move(rookCellTo);
                    _boardManager.ChangeTurn();
                    _boardManager.UpdatePosition(rook, rookCellFrom, rookCellTo);
                    return;
                }
            }
            else if (rookCellFrom.Number == oppositeKing.CurrentCell.Number + 1)
            {
                // Place the king on the same line as the king
                foreach (var queenCellTo in queen.InfluencedCells)
                {
                    if (queenCellTo.Number == oppositeKing.CurrentCell.Number &&
                        (Math.Abs(queenCellTo.Letter - oppositeKing.CurrentCell.Letter) >= 2 ||
                        _boardManager.IsPossibleToMove(king, queenCellTo) || _boardManager.IsPossibleToMove(rook, queenCellTo)) &&
                        _boardManager.IsPossibleToMove(queen, queenCellTo) && _boardManager.IsFreeCell(queenCellTo))
                    {
                        queen.Move(queenCellTo);
                        _boardManager.ChangeTurn();
                        _boardManager.UpdatePosition(queen, queenCellFrom, queenCellTo);
                        return;
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
                        _boardManager.IsPossibleToMove(king, queenCellTo) || _boardManager.IsPossibleToMove(rook, queenCellTo)) &&
                        _boardManager.IsPossibleToMove(queen, queenCellTo) && _boardManager.IsFreeCell(queenCellTo))
                    {
                        queen.Move(queenCellTo);
                        _boardManager.ChangeTurn();
                        _boardManager.UpdatePosition(queen, queenCellFrom, queenCellTo);
                        if (_boardManager.IsStaleMate())
                        {
                            queen.Move(queenCellFrom);
                            _boardManager .Turn = queen.Color == Color.White;                            
                            _boardManager.UpdatePosition(queen, queenCellTo, queenCellFrom);
                            continue;
                        }

                        return;
                    }
                }
            }

            if (!TryMovingKing())
            {
                TryMovingQueen();
            }
        }

        /// <summary>
        /// Algoithm moves queen and rook in turn to keep pressing king to the border until it the Mate.
        /// </summary>
        /// <returns>True if moved, False if no case mathced</returns>
        private void PressingLeft()
        {
            var turn = _boardManager.Turn ? Color.White : Color.Black;

            Color oppositeColor = turn == Color.White ? Color.Black : Color.White;
            King oppositeKing = _boardManager.GetTheKing(oppositeColor);

            King king = _boardManager.GetTheKing(turn);

            Queen queen = _boardManager.GetFigure(typeof(Queen), turn) as Queen;
            Cell queenCellFrom = queen.CurrentCell;

            Rook rook = _boardManager.GetFigure(typeof(Rook), turn) as Rook;
            Cell rookCellFrom = rook.CurrentCell;

            Cell rookCellTo;
            //if queen is its correct start place
            if (queenCellFrom.Letter == oppositeKing.CurrentCell.Letter - 1)
            {
                //if rook is safe to move left
                rookCellTo = new Cell(oppositeKing.CurrentCell.Letter, rookCellFrom.Number);
                //if rook is safe to move up
                if (Math.Abs(rookCellFrom.Number - oppositeKing.CurrentCell.Number) >= 2
                    || _boardManager.IsPossibleToMove(queen, rookCellTo) || _boardManager.IsPossibleToMove(king, rookCellTo))
                {
                    if (_boardManager.IsPossibleToMove(rook, rookCellTo) && _boardManager.IsFreeCell(rookCellTo))
                    {
                        rook.Move(rookCellTo);
                        _boardManager.ChangeTurn();
                        _boardManager.UpdatePosition(rook, rookCellFrom, rookCellTo);
                        return;
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

                    if (!(_boardManager.IsPossibleToMove(rook, rookCellTo) && _boardManager.IsFreeCell(rookCellTo)))
                    {
                        rookCellTo = new Cell((char)(rookCellFrom.Letter + 1), rookCellFrom.Number);
                    }

                    rook.Move(rookCellTo);
                    _boardManager.ChangeTurn();
                    _boardManager.UpdatePosition(rook, rookCellFrom, rookCellTo);                   
                    return;
                }
            }
            else if (rookCellFrom.Letter == oppositeKing.CurrentCell.Letter - 1)
            {
                // Place the queen on the same line as king
                foreach (var queenCellTo in queen.InfluencedCells)
                {
                    if (queenCellTo.Letter == oppositeKing.CurrentCell.Letter &&
                        (Math.Abs(queenCellTo.Number - oppositeKing.CurrentCell.Number) >= 2 ||
                        _boardManager.IsPossibleToMove(king, queenCellTo) || _boardManager.IsPossibleToMove(rook, queenCellTo)) &&
                        _boardManager.IsPossibleToMove(queen, queenCellTo) && _boardManager.IsFreeCell(queenCellTo))
                    {
                        queen.Move(queenCellTo);
                        _boardManager.ChangeTurn();
                        _boardManager.UpdatePosition(queen, queenCellFrom, queenCellTo);
                        return;
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
                        _boardManager.IsPossibleToMove(king, queenCellTo) || _boardManager.IsPossibleToMove(rook, queenCellTo)) &&
                        _boardManager.IsPossibleToMove(queen, queenCellTo) && _boardManager.IsFreeCell(queenCellTo))
                    {
                        queen.Move(queenCellTo);
                        _boardManager.ChangeTurn();
                        _boardManager.UpdatePosition(queen, queenCellFrom, queenCellTo);
                        if (_boardManager.IsStaleMate())
                        {
                            queen.Move(queenCellFrom);
                            _boardManager.Turn = queen.Color == Color.White;
                            _boardManager.UpdatePosition(queen, queenCellTo, queenCellFrom);
                            continue;
                        }
                        return;
                    }
                }
            }

            if (!TryMovingKing())
            {
                TryMovingQueen();
            }
        }

        /// <summary>
        /// Algoithm moves queen and rook in turn to keep pressing king to the border until it the Mate.
        /// </summary>
        /// <returns>True if moved, False if no case mathced</returns>
        private void PressingRight()
        {
            var turn = _boardManager.Turn ? Color.White : Color.Black;

            Color oppositeColor = turn == Color.White ? Color.Black : Color.White;
            King oppositeKing = _boardManager.GetTheKing(oppositeColor);

            King king = _boardManager.GetTheKing(turn);

            Queen queen = _boardManager.GetFigure(typeof(Queen), turn) as Queen;
            Cell queenCellFrom = queen.CurrentCell;

            Rook rook = _boardManager.GetFigure(typeof(Rook), turn) as Rook;
            Cell rookCellFrom = rook.CurrentCell;

            Cell rookCellTo;
            //if queen is its correct start place
            if (queenCellFrom.Letter == oppositeKing.CurrentCell.Letter + 1)
            {
                //if rook is safe to move right
                rookCellTo = new Cell(oppositeKing.CurrentCell.Letter, rookCellFrom.Number);
                if (Math.Abs(rookCellFrom.Number - oppositeKing.CurrentCell.Number) >= 2
                    || _boardManager.IsPossibleToMove(queen, rookCellTo) || _boardManager.IsPossibleToMove(king, rookCellTo))
                {
                    if (_boardManager.IsPossibleToMove(rook, rookCellTo) && _boardManager.IsFreeCell(rookCellTo))
                    {
                        rook.Move(rookCellTo);
                        _boardManager.ChangeTurn();
                        _boardManager.UpdatePosition(rook, rookCellFrom, rookCellTo);
                        return;
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

                    if (!(_boardManager.IsPossibleToMove(rook, rookCellTo) && _boardManager.IsFreeCell(rookCellTo)))
                    {
                        rookCellTo = new Cell((char)(rookCellFrom.Letter - 1), rookCellFrom.Number);
                    }

                    rook.Move(rookCellTo);
                    _boardManager.ChangeTurn();
                    _boardManager.UpdatePosition(rook, rookCellFrom, rookCellTo);
                    return;

                }
            }
            else if (rookCellFrom.Letter == oppositeKing.CurrentCell.Letter + 1)
            {
                // Place the queen on the same line as king
                foreach (var queenCellTo in queen.InfluencedCells)
                {
                    if (queenCellTo.Letter == oppositeKing.CurrentCell.Letter &&
                        (Math.Abs(queenCellTo.Number - oppositeKing.CurrentCell.Number) >= 2 ||
                        _boardManager.IsPossibleToMove(king, queenCellTo) || _boardManager.IsPossibleToMove(rook, queenCellTo)) &&
                        _boardManager.IsPossibleToMove(queen, queenCellTo) && _boardManager.IsFreeCell(queenCellTo))
                    {
                        queen.Move(queenCellTo);
                        _boardManager.ChangeTurn();
                        _boardManager.UpdatePosition(queen, queenCellFrom, queenCellTo);
                        return;
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
                        _boardManager.IsPossibleToMove(king, queenCellTo) || _boardManager.IsPossibleToMove(rook, queenCellTo)) &&
                        _boardManager.IsPossibleToMove(queen, queenCellTo) && _boardManager.IsFreeCell(queenCellTo))
                    {
                        queen.Move(queenCellTo);
                        _boardManager.ChangeTurn();
                        _boardManager.UpdatePosition(queen, queenCellFrom, queenCellTo);
                        if (_boardManager.IsStaleMate())
                        {
                            queen.Move(queenCellFrom);
                            _boardManager.Turn = queen.Color == Color.White;
                            _boardManager.UpdatePosition(queen, queenCellTo, queenCellFrom);
                            
                            continue;
                        }
                        return;
                    }
                }
            }

            if (!TryMovingKing())
            {
                TryMovingQueen();
            }
        }

        /// <summary>
        /// Move the queen to all its possible cells to check if there will be a Mate situation on one of them
        /// </summary>
        /// <returns>True if moveing queen can result in Mate, False if no Mate situation is possible for this positing</returns>
        private bool TryWinningWithQueenMove()
        {
            var turn = _boardManager.Turn ? Color.White : Color.Black;

            Color playerColor = _boardManager.Turn ? Color.White : Color.Black ;
            Color oppositeColor = playerColor == Color.White ? Color.Black : Color.White;

            King oppositeKing = _boardManager.GetTheKing(oppositeColor);
            Queen queen = _boardManager.GetFigure(typeof(Queen), turn) as Queen;
            Rook rook = _boardManager.GetFigure(typeof(Rook), turn) as Rook;
            King king = _boardManager.GetTheKing(turn);

            Cell cellFrom = queen.CurrentCell;
            List<Cell> currentInfuencedCells = queen.InfluencedCells;

            foreach (var cellTo in currentInfuencedCells)
            {
                bool isSafeDisstance = Math.Abs(cellTo.Number - oppositeKing.CurrentCell.Number) >= 2 || 
                                       Math.Abs(cellTo.Letter - oppositeKing.CurrentCell.Letter) >= 2;
                if ((_boardManager.IsPossibleToMove(queen, cellTo) && _boardManager.IsFreeCell(cellTo) && isSafeDisstance)
                    || _boardManager.IsPossibleToMove(rook, cellTo) || _boardManager.IsPossibleToMove(king, cellTo))
                {
                    queen.Move(cellTo);
                    _boardManager.ChangeTurn();
                    if (_boardManager.IsMate())
                    {
                        _boardManager.UpdatePosition(queen, cellFrom, cellTo);
                        return true;
                    }
                    else
                    {
                        queen.Move(cellFrom);
                        _boardManager.Turn = playerColor == Color.White;
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
            var turn = _boardManager.Turn ? Color.White : Color.Black;

            Color oppositeColor = turn == Color.White ? Color.Black : Color.White;

            King oppositeKing = _boardManager.GetTheKing(oppositeColor);
            Queen queen = _boardManager.GetFigure(typeof(Queen), turn) as Queen;
            Rook rook = _boardManager.GetFigure(typeof(Rook), turn) as Rook;
            King king = _boardManager.GetTheKing(turn);
            Cell cellFrom = rook.CurrentCell;

            foreach (var cellTo in rook.InfluencedCells)
            {
                if (_boardManager.IsPossibleToMove(rook, cellTo) && _boardManager.IsFreeCell(cellTo) &&
                   ((Math.Abs(cellTo.Number - oppositeKing.CurrentCell.Number) >= 2 && Math.Abs(cellTo.Letter - oppositeKing.CurrentCell.Letter) >= 2)
                       || _boardManager.IsPossibleToMove(queen, cellTo) || _boardManager.IsPossibleToMove(king, cellTo)))
                {
                    rook.Move(cellTo);
                    _boardManager.ChangeTurn();
                    if (_boardManager.IsMate())
                    {
                        _boardManager.UpdatePosition(rook, cellFrom, cellTo);
                        return true;
                    }
                    else
                    {
                        rook.Move(cellFrom);
                        _boardManager.Turn = turn == Color.White;
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
            var turn = _boardManager.Turn ? Color.White : Color.Black;

            Color oppositeColor = turn == Color.White ? Color.Black : Color.White;

            King oppositeKing = _boardManager.GetTheKing(oppositeColor);
            Queen queen = _boardManager.GetFigure(typeof(Queen), turn) as Queen;
            Rook rook = _boardManager.GetFigure(typeof(Rook), turn) as Rook;
            King king = _boardManager.GetTheKing(turn);
            Cell cellFrom = king.CurrentCell;

            foreach (var cellTo in king.InfluencedCells)
            {
                if (_boardManager.IsPossibleToMove(king, cellTo) && _boardManager.IsFreeCell(cellTo) &&
                   ((Math.Abs(cellTo.Number - oppositeKing.CurrentCell.Number) >= 2 && Math.Abs(cellTo.Letter - oppositeKing.CurrentCell.Letter) >= 2)
                       || _boardManager.IsPossibleToMove(queen, cellTo) || _boardManager.IsPossibleToMove(rook, cellTo)))
                {
                    king.Move(cellTo);
                    _boardManager.ChangeTurn();
                    if (_boardManager.IsMate())
                    {
                        _boardManager.UpdatePosition(king, cellFrom, cellTo);
                        return true;
                    }
                    else
                    {
                        king.Move(cellFrom);
                        _boardManager.Turn = turn == Color.White;
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
            var turn = _boardManager.Turn ? Color.White : Color.Black;

            Color oppositeColor = turn == Color.White ? Color.Black : Color.White;
            King oppositeKing = _boardManager.GetTheKing(oppositeColor);

            Queen queen = _boardManager.GetFigure(typeof(Queen), turn) as Queen;
            Rook rook = _boardManager.GetFigure(typeof(Rook), turn) as Rook;
            King king = _boardManager.GetTheKing(turn);
            Cell rookCellFrom = rook.CurrentCell;

            for (int i = oppositeKing.CurrentCell.Letter - 1; i <= oppositeKing.CurrentCell.Letter + 1; i++)
            {
                for (int j = oppositeKing.CurrentCell.Number - 1; j <= oppositeKing.CurrentCell.Number + 1; j++)
                {
                    if (rookCellFrom.Letter == i && rookCellFrom.Number == j &&
                        !_boardManager.IsPossibleToMove(queen, rookCellFrom) &&
                        !_boardManager.IsPossibleToMove(king, rookCellFrom))
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
                                    if (!(_boardManager.IsPossibleToMove(rook, rookCellTo) && _boardManager.IsFreeCell(rookCellTo)))
                                    {
                                        rookCellTo = new Cell((char)(king.CurrentCell.Letter + 1), rookCellFrom.Number);
                                    }

                                }
                                else
                                {
                                    char letterTo = queen.CurrentCell.Letter == 'H' ? 'G' : 'H';
                                    rookCellTo = new Cell(letterTo, rookCellFrom.Number);
                                    if (!(_boardManager.IsPossibleToMove(rook, rookCellTo) && _boardManager.IsFreeCell(rookCellTo)))
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
                                    if (!(_boardManager.IsPossibleToMove(rook, rookCellTo) && _boardManager.IsFreeCell(rookCellTo)))
                                    {
                                        rookCellTo = new Cell(rookCellFrom.Letter, king.CurrentCell.Number + 1);
                                    }
                                }
                                else
                                {
                                    int numberTo = queen.CurrentCell.Number == 8 ? 7 : 8;
                                    rookCellTo = new Cell(rookCellFrom.Letter, numberTo);
                                    if (!(_boardManager.IsPossibleToMove(rook, rookCellTo) && _boardManager.IsFreeCell(rookCellTo)))
                                    {
                                        rookCellTo = new Cell(rookCellFrom.Letter, king.CurrentCell.Number - 1);
                                    }
                                }
                                break;
                        }

                        rook.Move(rookCellTo);
                        _boardManager.ChangeTurn();
                        _boardManager.UpdatePosition(rook, rookCellFrom, rookCellTo);
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
            var turn = _boardManager.Turn ? Color.White : Color.Black;

            Color oppositeColor = turn == Color.White ? Color.Black : Color.White;

            King king = _boardManager.GetTheKing(turn);
            Cell kingCellFrom = king.CurrentCell;
            Rook rook = _boardManager.GetFigure(typeof(Rook), turn) as Rook;


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
                        if (_boardManager.IsPossibleToMove(king, kingCellTo) && _boardManager.IsFreeCell(kingCellTo))
                        {
                            king.Move(kingCellTo);
                            _boardManager.ChangeTurn();
                            _boardManager.UpdatePosition(king, kingCellFrom, kingCellTo);
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

                        if (_boardManager.IsPossibleToMove(king, kingCellTo) && _boardManager.IsFreeCell(kingCellTo))
                        {
                            king.Move(kingCellTo);
                            _boardManager.ChangeTurn();
                            _boardManager.UpdatePosition(king, kingCellFrom, kingCellTo);
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
            var turn = _boardManager.Turn ? Color.White : Color.Black;

            Color oppositeColor = turn == Color.White ? Color.Black : Color.White;

            King oppositeKing = _boardManager.GetTheKing(oppositeColor);
            Cell kingCellFrom = oppositeKing.CurrentCell;

            Queen queen = _boardManager.GetFigure(typeof(Queen), turn) as Queen;
            Cell queenCellFrom = queen.CurrentCell;
            switch (_algorithmChoice)
            {
                case 'U':
                case 'D':
                    foreach (var queenCellTo in queen.InfluencedCells)
                    {
                        if (queenCellTo.Number == queenCellFrom.Number &&
                            Math.Abs(queenCellTo.Letter - kingCellFrom.Letter) >= 2 &&
                            queenCellTo != queenCellFrom && _boardManager.IsPossibleToMove(queen, queenCellTo) && _boardManager.IsFreeCell(queenCellTo))
                        {
                            queen.Move(queenCellTo);
                            _boardManager.ChangeTurn();
                            _boardManager.UpdatePosition(queen, queenCellFrom, queenCellTo);
                            return true;
                        }
                    }
                    break;
                case 'L':
                case 'R':
                    foreach (var queenCellTo in queen.InfluencedCells)
                    {
                        if (queenCellTo.Letter == queenCellFrom.Letter &&
                            Math.Abs(queenCellTo.Number - kingCellFrom.Number) >= 2 &&
                            queenCellTo != queenCellFrom && _boardManager.IsPossibleToMove(queen, queenCellTo) && _boardManager.IsFreeCell(queenCellTo))
                        {
                            queen.Move(queenCellTo);
                            _boardManager.ChangeTurn();
                            _boardManager.UpdatePosition(queen, queenCellFrom, queenCellTo);
                            return true;
                        }
                    }
                    break;
            }

            if (queenCellFrom.Letter == 'A' || queenCellFrom.Letter == 'H')
            {
                switch (queenCellFrom.Number)
                {
                    case 1:
                    case 8:
                        {
                            foreach (var queenCellTo in queen.InfluencedCells)
                            {
                                if (Math.Abs(queenCellTo.Number - queenCellFrom.Number) == 1 &&
                                    Math.Abs(queenCellTo.Letter - queenCellFrom.Letter) == 1 &&
                                    _boardManager.IsPossibleToMove(queen, queenCellTo) && _boardManager.IsFreeCell(queenCellTo))
                                {
                                    queen.Move(queenCellTo);
                                    _boardManager.ChangeTurn();
                                    _boardManager.UpdatePosition(queen, queenCellFrom, queenCellTo);
                                    return true;
                                }
                            }

                            break;
                        }
                }
            }

            return false;
        }
    }
}
