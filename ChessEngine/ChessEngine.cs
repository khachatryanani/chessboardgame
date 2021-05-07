using ChessBoard.BoardAttributes;
using ChessBoard.Figures;
using static ChessBoard.ChessBoardManager;
using System;
using System.Collections.Generic;

namespace ChessEngineLogic
{
    public partial class ChessEngine
    {
        /// <summary>
        /// Turn of the player represented by CellColor enum value
        /// </summary>
        private Color _turn;

        /// <summary>
        /// Event occures when a figure is moved in the inner board.
        /// </summary>
        public event EventHandler<GameEventArgs> FigureMoved;

        public event EventHandler<GameEventArgs> GameStatusChanged;

        protected virtual void OnGameStatusChanged()
        {
            int status = GetGameStatus();

            GameStatusChanged?.Invoke(this, new GameEventArgs
            {
                GameStatus = status,
                CurrentPlayer = _turn == Color.Black ? "Black" : "White"
            });
        }

        protected virtual void OnFigureMoved(Figure movedFigure, Cell cellFrom)
        {
            int status = GetGameStatus();
            string currentPlayer = movedFigure.Color == Color.Black ? "White" : "Black";
            string winner = string.Empty;

            if (status == 2)
            {
                winner = _turn == Color.Black ? "Whites" : "Blacks";
            }

            FigureMoved?.Invoke(this, new GameEventArgs
            {
                MovedFigure = movedFigure.Name,
                CellFrom = cellFrom.ToString(),
                CellTo = movedFigure.CurrentCell.ToString(),
                GameStatus = status,
                CurrentPlayer = currentPlayer,
                WinnerPlayer = winner
            });
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="playerColor">Color of the first player.</param>
        public ChessEngine(string playerColor)
        {
            _turn = playerColor == "Black" ? Color.Black : Color.White;
            ResetBoard();
        }

        public void SetTurn(bool whites) 
        {
            if (whites)
            {
                _turn = Color.White;
            }
            else 
            {
                _turn = Color.Black;
            }
        }
        /// <summary>
        /// In the endgame, plays as Chess Engine.
        /// </summary>
        /// <param name="choice">Game choice</param>
        public void PlayEngine(int choice)
        {
            switch (choice)
            {
                case 2:
                    PlayWinningWithQueenAndRookAlgorithm();
                    break;
                case 3:
                    break;
            }
        }

        /// <summary>
        /// According the occured move updates the inner board of Chess Engine and fires the FigreMoved event.
        /// </summary>
        /// <param name="figure">Figure that moved.</param>
        /// <param name="cellFrom">Cell from which the figure moved.</param>
        /// <param name="cellTo">Cell to which the figure moved.</param>
        private void UpdatePosition(Figure figure, Cell cellFrom, Cell cellTo)
        {
            UpdateBoard(cellFrom, cellTo);
            OnFigureMoved(figure, cellFrom);
        }

        /// <summary>
        /// Returns the current game status: 0 = usual, 1 = check, 2 = mate, 3 = stalemate
        /// </summary>
        /// <returns>Game status in integer representation</returns>
        private int GetGameStatus()
        {
            if (IsMate())
            {
                return 2;
            }
            else if (IsStaleMate())
            {
                return 3;
            }
            else if (IsCheck())
            {
                return 1;
            }
            else if (IsPawnUpgrade()) 
            {
                _turn = _turn == Color.White ? Color.Black : Color.White;
                return 4;
            }

            return 0;
        }



        /// <summary>
        /// Get the list of cell names for the figure that are possible to move to.
        /// </summary>
        /// <param name="cellString">Cell name that figure currenlty stands on.</param>
        /// <returns>List of cells names that are possible to move to.</returns>
        public List<string> GetPossibleMoves(string cellString)
        {
            List<string> possibleMoves = new List<string>();
            Cell currentCell = new Cell(cellString);
            Figure figure = GetFigureByCell(currentCell);

            if (figure is King)
            {
                if (IsShortCastelingPossible(figure.Color, out Cell cellToMoveS))
                {
                    possibleMoves.Add(cellToMoveS.ToString());
                }

                if (IsLongCastelingPossible(figure.Color, out Cell cellToMoveL))
                {
                    possibleMoves.Add(cellToMoveL.ToString());
                }
            }

            if (figure is Pawn)
            {
                foreach (var pawnCell in (figure as Pawn).MovementCells)
                {
                    if (figure.CurrentCell != pawnCell && IsFreeCell(pawnCell))
                    {
                        possibleMoves.Add(pawnCell.ToString());
                    }
                }
            }

            foreach (var cell in figure.InfluencedCells)
            {
                if (IsPossibleMoveForCurrentPosition(figure, cell) && figure.CurrentCell != cell)
                {
                    possibleMoves.Add(cell.ToString());
                }
            }

            return possibleMoves;
        }

        /// <summary>
        /// Creates a figure with the given parameters and adds them to the inner board.
        /// </summary>
        /// <param name="cellString">Cell name that the figure stands on.</param>
        /// <param name="typeString">Type name of the figure to be created.</param>
        /// <param name="colorString">Color name of the figure to be created.</param>
        public void CreateFigure(string cellString, string typeString, string colorString)
        {
            Type type = GetTypeFromString(typeString);
            Color color = GetCellColorFromString(colorString);
            Cell cell = new Cell(cellString);

            // Create a figure
            Figure figure = (Figure)Activator.CreateInstance(type, cell, color);

            // Add to ChessBoard
            AddFigure(figure);
        }

        /// <summary>
        /// Get the Type based on the type name
        /// </summary>
        /// <param name="type">string representation of type name</param>
        /// <returns>Type according the string name of the type./returns>
        private Type GetTypeFromString(string type)
        {
            return type switch
            {
                "Q" => typeof(Queen),
                "R" => typeof(Rook),
                "K" => typeof(King),
                "N" => typeof(Knight),
                "B" => typeof(Bishop),
                _ => typeof(Pawn),
            };
        }

        /// <summary>
        /// Get the CellColor enum value based on the color name
        /// </summary>
        /// <param name="color">String representation of color</param>
        /// <returns>CellColor value relevant to given color name.</returns>
        private Color GetCellColorFromString(string color)
        {
            return color == "B" ? Color.Black : Color.White;
        }

        /// <summary>
        /// Registered the user move and updates the inner board.
        /// </summary>
        /// <param name="cellFrom">Cell from which user moved.</param>
        /// <param name="cellTo">Cell to which user moved.</param>
        public void PlayUser(string cellFrom, string cellTo)
        {
            Figure figure = GetFigureByCell(new Cell(cellFrom));
            figure.Move(new Cell(cellTo));

            UpdateBoard(new Cell(cellFrom), new Cell(cellTo));
            if (figure is King)
            {
                DoCasteling(figure);
            }
            this._turn = this._turn == Color.Black ? Color.White : Color.Black;
            OnGameStatusChanged();
        }

        public void DoCasteling(Figure king) 
        {
            if (king.Color == Color.Black)
            {
                if (king.CurrentCell.ToString() == "G8")
                {
                    var rook = GetFigureByCell(new Cell("H8"));
                    var cellFrom = rook.CurrentCell;
                    var cellTo = new Cell("F8");
                    rook.Move(cellTo);
                    UpdateBoard(cellFrom, cellTo);
                    OnFigureMoved(rook, cellFrom);
                }

                if (king.CurrentCell.ToString() == "C8")
                {
                    var rook = GetFigureByCell(new Cell("A8"));
                    var cellFrom = rook.CurrentCell;

                    var cellTo = new Cell("D8");
                    rook.Move(cellTo);
                    UpdateBoard(cellFrom, cellTo);
                    OnFigureMoved(rook, cellFrom);
                }
            }
            else 
            {
                if (king.CurrentCell.ToString() == "G1")
                {
                    var rook = GetFigureByCell(new Cell("H1"));
                    var cellFrom = rook.CurrentCell;

                    var cellTo = new Cell("F1");
                    rook.Move(cellTo);
                    UpdateBoard(cellFrom, cellTo);
                    OnFigureMoved(rook, cellFrom);
                }

                if (king.CurrentCell.ToString() == "C1")
                {
                    var rook = GetFigureByCell(new Cell("A1"));
                    var cellFrom = rook.CurrentCell;

                    var cellTo = new Cell("D1");
                    rook.Move(cellTo);
                    UpdateBoard(cellFrom, cellTo);
                    OnFigureMoved(rook, cellFrom);
                }
            }
        }

        // Move to ChessBoard as a seperate class that checks
        /// <summary>
        /// Verifies if the king of the current player is/will be under Chess Check
        /// </summary>
        /// <returns>True if the king is under Chess Check, False if it does not</returns>
        private bool IsCheck()
        {

            // Get the King Figure
            King king = GetTheKing(_turn);

            // Check if it is on cell under influence
            return IsUnderCheckCell(king.CurrentCell, king.Color, out _);
        }

        /// Checks if it is a Mate situation in the game:
        /// if the King is under influence of opposite color figure and has no available cells to move to
        /// </summary>
        /// <returns>True if it is a Mate, False if it is not a Mate</returns>
        private bool IsMate()
        {
            King king = GetTheKing(_turn);
            if (IsCheck())
            {
                foreach (var item in king.InfluencedCells)
                {
                    if (!IsInfluencedCell(item, king.Color) && IsFreeCell(item))
                    {
                        return false;
                    }
                }

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
            King king = GetTheKing(_turn);

            if (!IsCheck())
            {
                foreach (var item in king.InfluencedCells)
                {
                    if (!IsUnderCheckCell(item, king.Color, out _) && item != king.CurrentCell)
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
