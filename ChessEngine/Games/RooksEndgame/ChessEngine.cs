using System;
using System.Collections.Generic;
using System.Text;
using ChessBoard.BoardAttributes;
using ChessBoard.Figures;
using ChessBoard.Extensions;
using static ChessBoard.ChessBoardManager;

namespace ChessEngineLogic
{
    public partial class ChessEngine
    {
        public void PlayWinningWithQueenAndTwoRooksAlgorithm()
        {
            Color colorOfOpposite = _turn == Color.Black ? Color.White : Color.Black;
            King king = GetTheKing(colorOfOpposite);
            int number = king.CurrentCell.Number;

            if (GetFigure(typeof(Queen), _turn) is Queen queen)
            {
                Cell queenCellFrom = queen.CurrentCell;
                if (queenCellFrom.Number != number + 1 && queenCellFrom.Number != number - 1)
                {
                    Cell queenCellTo = new Cell(queenCellFrom.Letter, number);
                    if (IsPossibleToMove(queen, queenCellTo) && IsFreeCell(queenCellTo))
                    {
                        queen.Move(queenCellTo);
                        _turn = colorOfOpposite;
                        UpdatePosition(queen,queenCellFrom, queenCellTo);
                    }
                }
                else
                {
                    if (GetFigure(typeof(Rook), _turn) is Rook rook1)
                    {
                        Cell rook1CellFrom = rook1.CurrentCell;
                        if (Math.Abs(rook1CellFrom.Letter - king.CurrentCell.Letter) <= 2)
                        {
                            Cell rook1CellTo;
                            if (Math.Abs(rook1CellFrom.Letter - queen.CurrentCell.Letter) > 1)
                            {
                                if (rook1CellFrom.Letter > queen.CurrentCell.Letter)
                                {

                                    rook1CellTo = new Cell((char)(queen.CurrentCell.Letter + 1), rook1CellFrom.Number);
                                }
                                else
                                {
                                    rook1CellTo = new Cell((char)(queen.CurrentCell.Letter - 1), rook1CellFrom.Number);
                                }

                                if (IsPossibleToMove(rook1, rook1CellTo) && IsFreeCell(rook1CellTo))
                                {
                                    rook1.Move(rook1CellTo);
                                    _turn = colorOfOpposite;
                                    UpdatePosition(rook1,rook1CellFrom, rook1CellTo);
                                }

                                return;
                            }
                        }

                        if (rook1CellFrom.Number != number + 1 && rook1CellFrom.Number != number - 1)
                        {
                            Cell rook1CellTo = new Cell(rook1CellFrom.Letter, number);

                            if (IsPossibleToMove(rook1, rook1CellTo) && IsFreeCell(rook1CellTo))
                            {
                                rook1.Move(rook1CellTo);
                                _turn = colorOfOpposite;
                                UpdatePosition(rook1, rook1CellFrom, rook1CellTo);
                            }
                        }
                        else
                        {
                            if (GetFigure(typeof(Rook), _turn, 2) is Rook rook2)
                            {
                                Cell rook2CellFrom = rook2.CurrentCell;
                                Cell rook2CellTo = new Cell(rook2.CurrentCell.Letter, number);
                                if (IsPossibleToMove(rook2, rook2CellTo) && IsFreeCell(rook2CellTo))
                                {
                                    rook2.Move(rook2CellTo);
                                    _turn = colorOfOpposite;
                                    UpdatePosition(rook2, rook2CellFrom, rook2CellTo);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
