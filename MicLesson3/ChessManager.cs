using System;
using System.Collections.Generic;
using ChessBoard;
using ChessBoard.BoardAttributes;
using ChessBoard.Figures;
using ChessBoard.Extensions;
using static ChessBoard.ChessBoardManager;

namespace ChessGameManager
{
    /// <summary>
    /// Manages the game of chess and plays with the user
    /// </summary>
    public static class ChessManager
    {
        // Define the turn to play by a color of currenlty playing figures
        private static Color _turn = Color.Black;

        // Run the chess game
        public static void Run(Color turn, List<KeyValuePair<Color, Type>> figures)
        {
            StartTheGame(turn, figures);
            while (!IsMate() && !IsStaleMate())
            {
                Play();
            }
        }

        /// <summary>
        /// Get the custom game position from user
        /// </summary>
        /// <param name="turn">Defines whos turn it is to play according to given color</param>
        /// <param name="figures">List of figures to be created and added to board</param>
        private static void StartTheGame(Color turn, List<KeyValuePair<Color, Type>> figures)
        {
            // Create figures of specified color and on specified cell
            foreach (var item in figures)
            {
                Cell cell;
                // Ask user for cell info on each figure, check if it's an acceptable
                Console.WriteLine($"Enter chess board start cell for {item.Key.ToString()} {item.Value.Name}");
                do
                {
                    cell = GetUserStartCell();
                }
                while (!IsAcceptableCell(cell, item.Value, item.Key));

                CreateFigure(cell, item.Value, item.Key);
            }

            // Give the turn
            _turn = turn;

        }

        /// <summary>
        /// Checks if it is a Mate situation in the game:
        /// if the King is under influence of opposite color figure and has no available cells to move to
        /// </summary>
        /// <returns>True if it is a Mate, False if it is not a Mate</returns>
        private static bool IsMate()
        {
            // Get the King Figure
            King king = ChessBoard.ChessBoardManager.GetTheKing(_turn);

            if (ChessBoard.ChessBoardManager.IsUnderCheckCell(king.CurrentCell, king.Color))
            {
                foreach (var item in king.InfluencedCells)
                {
                    if (!IsInfluencedCell(item, king.Color) && IsFreeCell(item))
                    {
                        return false;
                    }
                }
                Console.Clear();
                DrawBoard();

                // Print winner side name
                string winner = _turn == Color.Black ? "Whites" : "Blacks";
                Console.WriteLine($"Mate! {winner} win!");

                return true;
            }
            return false;
        }

        /// <summary>
        /// Verifies if the king of the current player is/will be under Chess Check
        /// </summary>
        /// <returns>True if the king is under Chess Check, False if it does not</returns>
        private static bool IsCheck()
        {

            // Get the King Figure
            King king = GetTheKing(_turn);

            // Check if it is on cell under influence
            if (IsUnderCheckCell(king.CurrentCell, king.Color))
            {
                Console.WriteLine($"{king.Color} king is under check!");
                return true;
            }

            return false;
        }

        /// <summary>
        /// Check if it is a stalemate situation in the game
        /// </summary>
        /// <returns>True if it is a stalemate, False if it is not</returns>
        private static bool IsStaleMate()
        {
            King king = GetTheKing(_turn);

            if (!IsUnderCheckCell(king.CurrentCell, king.Color))
            {
                foreach (var item in king.InfluencedCells)
                {
                    if (!IsUnderCheckCell(item, king.Color) && item != king.CurrentCell)
                    {
                        return false;
                    }
                }

                Console.Clear();
                DrawBoard();

                Console.WriteLine("It is a stalemate...");
                return true;
            }
            return false;

        }

        /// <summary>
        /// Plays the game by giving turns to user and chessmanager's algorithm
        /// </summary>
        private static void Play()
        {
            if (_turn == Color.Black)
            {
                // Wait for the user to move a figure
                WaitForUserToMove();
            }
            else
            {
                // Play the winning algorithm
                TryWinningAlgorithm();
            }
        }

        /// <summary>
        /// Create figures of the specified types and add them to the chess board
        /// </summary>
        /// <param name="c">Letter of the location coordinate</param>
        /// <param name="i">Interger of the location coordinate</param>
        /// <param name="type">Type of figure to be created</param>
        /// <param name="color">Color of the figure to be created</param>
        private static void CreateFigure(Cell cell, Type type, Color color)
        {
            // Create a figure
            Figure figure = (Figure)Activator.CreateInstance(type, cell, color);

            // Add to ChessBoard
            AddFigure(figure);
        }

        /// <summary>
        /// Initializes the starting cell of figure according to user's custom position
        /// </summary>
        /// <returns>Custom position cell from user</returns>
        private static Cell GetUserStartCell()
        {
            Cell cellFrom;

            do
            {
                Console.Write("Enter only valid cell coordinates: ");

                // Convert user input to Cell object
                string userCellFrom = Console.ReadLine();
                cellFrom = ConvertToValidCell(userCellFrom);
            }
            while (cellFrom == null);

            return cellFrom;

        }

        /// <summary>
        /// Get the user input of which cell to move the figure from
        /// </summary>
        /// <returns>Cell of the choosen figure</returns>
        private static Cell GetUserMoveFrom()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Cell cellFrom;

            do
            {
                Console.Write("Enter the coordinates of the cell to move FROM: ");

                // Convert user input to Cell object
                string userCellFrom = Console.ReadLine();
                cellFrom = ConvertToValidCell(userCellFrom);
            }
            while (cellFrom == null);

            return cellFrom;
        }

        /// <summary>
        /// Get the user input of which cell to move the figure to
        /// </summary>
        /// <returns>Cell of the choosen location</returns>
        private static Cell GetUserMoveTo()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Cell cellTo;

            do
            {
                Console.Write("Enter the coordinates of the cell to move TO: ");

                // Convert user input to Cell object
                string userCellFrom = Console.ReadLine();
                cellTo = ConvertToValidCell(userCellFrom);
                if (!IsFreeCell(cellTo))
                {
                    Console.WriteLine("There is already a figure here.");
                }
            }
            while (!(cellTo != null && IsFreeCell(cellTo)));

            return cellTo;
        }

        /// <summary>
        /// Asks a user to determine the move, moves the figure and updates the board.
        /// </summary>
        private static void WaitForUserToMove()
        {
            Console.Clear();

            // Display the current board for the user
            DrawBoard();

            // King of the user
            King king;

            // The cell to be moved
            Cell userCellFrom;

            //if king is under check display message and set the moveFrom cell to king's cell
            if (IsCheck())
            {
                king = GetTheKing(_turn);
                userCellFrom = king.CurrentCell;
            }
            // Get the user's cell to move from
            else
            {
                do
                {
                    userCellFrom = GetUserMoveFrom();
                    // for this particular game the only figure user hase will be the King
                    king = GetFigureByCell(userCellFrom) as King;

                    if (king == null)
                    {
                        Console.WriteLine("You do not have a figure on that position.");
                    }

                } while (king == null);
            }

            // Get the user's cell to move to
            Cell userCellTo;
            do
            {
                userCellTo = GetUserMoveTo();
                if (IsUnderCheckCell(userCellTo, _turn))
                {
                    Console.WriteLine($"{_turn.ToString()} king will be under check!");
                }
            }
            while (IsUnderCheckCell(userCellTo, Color.Black));

            king.Move(userCellTo);
            _turn = Color.White;
            UpdateBoard(userCellFrom, userCellTo);
        }


        /// Difining seperate steps for the winning algorithm and playing them in turn
        /// Queen and two Rooks try to hold horizontal lines and Check the opposite King until it is a Mate!


        /// <summary>
        /// Tries moving Queen for Check towards the opposite King
        /// </summary>
        /// <returns>True if the movement occured, False if no move occured</returns>
        private static bool TryMovingQueen()
        {
            Color colorOfOpposite = _turn == Color.Black ? Color.White : Color.Black;
            King oppositeKing = GetTheKing(colorOfOpposite);
            int oppositeKingCellNumber = oppositeKing.CurrentCell.Number;

            if (GetFigure(typeof(Queen), _turn) is Queen queen)
            {
                Cell queenCellFrom = queen.CurrentCell;
                // Move the queen only if it is not holding a horizontal line near the king
                if (queenCellFrom.Number != oppositeKingCellNumber + 1 && queenCellFrom.Number != oppositeKingCellNumber - 1)
                {
                    Cell queenCellTo;
                    foreach (var cell in queen.InfluencedCells)
                    {
                        if (cell.Number == oppositeKingCellNumber && Math.Abs(cell.Letter - oppositeKing.CurrentCell.Letter) >= 2)
                        {
                            queenCellTo = cell;
                            if (IsPossibleToMove(queen, queenCellTo) && IsFreeCell(queenCellTo))
                            {
                                queen.Move(queenCellTo);
                                UpdateBoard(queenCellFrom, queenCellTo);
                                _turn = colorOfOpposite;
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Tries moving the first (order 1) rook towards the horizontal line of the opposite king
        /// </summary>
        /// <returns>True if the movement occured, False if no move occured</returns>
        private static bool TryMovingFirstRookVertically()
        {
            Color colorOfOpposite = _turn == Color.Black ? Color.White : Color.Black;
            King oppositeKing = GetTheKing(colorOfOpposite);
            int oppositeKingCellNumber = oppositeKing.CurrentCell.Number;

            if (GetFigure(typeof(Queen), _turn) is Queen queen)
            {
                if (GetFigure(typeof(Rook), _turn) is Rook rook)
                {
                    Cell rookCellFrom = rook.CurrentCell;
                    // Moves only if the rook is not currenlty holding a horizontal line near the king
                    if (rookCellFrom.Number != oppositeKingCellNumber + 1 && rookCellFrom.Number != oppositeKingCellNumber - 1)
                    {
                        Cell rook1CellTo = new Cell(rookCellFrom.Letter, oppositeKingCellNumber);

                        if (IsPossibleToMove(rook, rook1CellTo) && IsFreeCell(rook1CellTo))
                        {
                            rook.Move(rook1CellTo);
                            UpdateBoard(rookCellFrom, rook1CellTo);
                            _turn = colorOfOpposite;
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        /// <summary>
        // Checks if the opposite king might get near the Rook (order 1) and moves the Rook to be near the queen for safety
        /// <returns>True if the movement occured, False if no move occured</returns>
        private static bool TryMovingFirstRookHorizontally()
        {
            Color colorOfOpposite = _turn == Color.Black ? Color.White : Color.Black;
            King oppositeKing = GetTheKing(colorOfOpposite);
            int oppositeKingCellNumber = oppositeKing.CurrentCell.Number;

            if (GetFigure(typeof(Queen), _turn) is Queen queen)
            {
                if (GetFigure(typeof(Rook), _turn) is Rook rook)
                {
                    Cell rookCellFrom = rook.CurrentCell;
                    Cell rookCellTo;

                    // Moves if the king is near (2 cell away) to the rook and the queen is not near
                    if ((Math.Abs(rookCellFrom.Letter - oppositeKing.CurrentCell.Letter) <= 2) &&
                        !(rookCellFrom.Letter == queen.CurrentCell.Letter - 1 ||
                        rookCellFrom.Letter == queen.CurrentCell.Letter + 1 ||
                        rookCellFrom.Letter == queen.CurrentCell.Letter))
                    {
                        if (rookCellFrom.Letter > queen.CurrentCell.Letter)
                        { 
                            rookCellTo = new Cell((char)(queen.CurrentCell.Letter + 1), rookCellFrom.Number);
                        }
                        else
                        {
                            rookCellTo = new Cell((char)(queen.CurrentCell.Letter - 1), rookCellFrom.Number);
                        }

                        if (IsPossibleToMove(rook, rookCellTo) && IsFreeCell(rookCellTo))
                        {
                            rook.Move(rookCellTo);
                            UpdateBoard(rookCellFrom, rookCellTo);
                            _turn = colorOfOpposite;
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Tries moving the second (order 2) rook towards the horizontal line of the opposite king
        /// </summary>
        /// <returns>True if the movement occured, False if no move occured</returns>
        private static bool TryMovingSecondRookVertically()
        {
            Color colorOfOpposite = _turn == Color.Black ? Color.White : Color.Black;
            King oppositeKing = GetTheKing(colorOfOpposite);
            int oppositeKingCellNumber = oppositeKing.CurrentCell.Number;

            if (GetFigure(typeof(Queen), _turn) is Queen queen)
            {
                if (GetFigure(typeof(Rook), _turn, 2) is Rook rook)
                {
                    Cell rookCellFrom = rook.CurrentCell;
                    // Moves only if the rook is not currenlty holding a horizontal line near the king
                    if (rookCellFrom.Number != oppositeKingCellNumber + 1 && rookCellFrom.Number != oppositeKingCellNumber - 1)
                    {
                        Cell rook1CellTo = new Cell(rookCellFrom.Letter, oppositeKingCellNumber);

                        if (IsPossibleToMove(rook, rook1CellTo) && IsFreeCell(rook1CellTo))
                        {
                            rook.Move(rook1CellTo);
                            UpdateBoard(rookCellFrom, rook1CellTo);
                            _turn = colorOfOpposite;
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        /// <summary>
        // Checks if the opposite king might get near the Rook (order 2) and moves the Rook to be near the queen for safety
        /// <returns>True if the movement occured, False if no move occured</returns>
        private static bool TryMovingSecondRookHorizontally()
        {
            Color colorOfOpposite = _turn == Color.Black ? Color.White : Color.Black;
            King oppositeKing = GetTheKing(colorOfOpposite);
            int oppositeKingCellNumber = oppositeKing.CurrentCell.Number;

            if (GetFigure(typeof(Queen), _turn) is Queen queen)
            {
                if (GetFigure(typeof(Rook), _turn, 2) is Rook rook)
                {
                    Cell rookCellFrom = rook.CurrentCell;
                    Cell rookCellTo;

                    // Moves if the king is near (2 cell away) to the rook and the queen is not near
                    if ((Math.Abs(rookCellFrom.Letter - oppositeKing.CurrentCell.Letter) <= 2) &&
                        !(rookCellFrom.Letter == queen.CurrentCell.Letter - 1 ||
                        rookCellFrom.Letter == queen.CurrentCell.Letter + 1 ||
                        rookCellFrom.Letter == queen.CurrentCell.Letter))
                    {
                        if (rookCellFrom.Letter > queen.CurrentCell.Letter)
                        {

                            rookCellTo = new Cell((char)(queen.CurrentCell.Letter + 1), rookCellFrom.Number);
                        }
                        else
                        {
                            rookCellTo = new Cell((char)(queen.CurrentCell.Letter - 1), rookCellFrom.Number);
                        }

                        if (IsPossibleToMove(rook, rookCellTo) && IsFreeCell(rookCellTo))
                        {
                            rook.Move(rookCellTo);
                            UpdateBoard(rookCellFrom, rookCellTo);
                            _turn = colorOfOpposite;
                            return true;
                        }
                    }

                }
            }
            return false;
        }

        /// <summary>
        /// Check if the King may defeat the Rooks and move them towards Queen (vertically)
        /// </summary>
        /// <returns>True if the movement occured, False if no move occured</returns>
        private static bool TrySavingRooks()
        {
            Color colorOfOpposite = _turn == Color.Black ? Color.White : Color.Black;
            King oppositeKing = GetTheKing(colorOfOpposite);
            if (GetFigure(typeof(Queen), _turn) is Queen queen)
            {
                // Check the first Rook
                if (GetFigure(typeof(Rook), _turn) is Rook rook1)
                {
                    Cell rook1CellFrom = rook1.CurrentCell;
                    Cell rook1CellTo;
                    if (Math.Abs(rook1CellFrom.Letter - oppositeKing.CurrentCell.Letter) == 1 &&
                        Math.Abs(rook1CellFrom.Number - oppositeKing.CurrentCell.Number) == 1)
                    {
                        if (!TryMovingFirstRookHorizontally())
                        {
                            rook1CellTo = new Cell(rook1CellFrom.Letter, queen.CurrentCell.Number);
                            if (IsPossibleToMove(rook1, rook1CellTo) && IsFreeCell(rook1CellTo))
                            {
                                rook1.Move(rook1CellTo);
                                UpdateBoard(rook1CellFrom, rook1CellTo);
                                _turn = colorOfOpposite;
                                return true;
                            }
                        }
                    }
                }

                // Check the second Rook
                if (GetFigure(typeof(Rook), _turn, 2) is Rook rook2)
                {
                    Cell rook2CellFrom = rook2.CurrentCell;
                    Cell rook2CellTo;
                    if (Math.Abs(rook2CellFrom.Letter - oppositeKing.CurrentCell.Letter) == 1 &&
                        Math.Abs(rook2CellFrom.Number - oppositeKing.CurrentCell.Number) == 1)
                    {
                        if (!TryMovingFirstRookHorizontally())
                        {
                            rook2CellTo = new Cell(rook2CellFrom.Letter, queen.CurrentCell.Number);
                            if (IsPossibleToMove(rook2, rook2CellTo) && IsFreeCell(rook2CellTo))
                            {
                                rook2.Move(rook2CellTo);
                                UpdateBoard(rook2CellFrom, rook2CellTo);
                                _turn = colorOfOpposite;
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Try moving King because it might be blocking other's moves
        /// </summary>
        /// <returns>True if the movement occured, False if no move occured</returns>
        private static bool TryMovingKing() 
        {
            Color colorOfOpposite = _turn == Color.Black ? Color.White : Color.Black;

            King king = GetTheKing(_turn);
            Cell kingCellFrom = king.CurrentCell;
            Cell kingCellTo;
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
                UpdateBoard(kingCellFrom, kingCellTo);
                _turn = colorOfOpposite;
                return true;
            }
            
            return false ;
        }

        private static void TryWinningAlgorithm()
        {
            // If the Rooks are under danger, firstly move them
            if (!TrySavingRooks()) 
            {
                // If rooks are safe start by moving the queen
                if (TryMovingQueen() ||
                    // If queen is already near the king's line, check if there is a need to move the rook horizontally
                    TryMovingFirstRookHorizontally() ||
                    // if not, move it vertically towards the king's line
                    TryMovingFirstRookVertically() ||
                    // Check the same for the second rook if the queen and the first rook are already moved to their good positions
                    TryMovingSecondRookHorizontally() || 
                    TryMovingSecondRookVertically()||
                    // Try moving the King if no move from above occured
                    TryMovingKing())
                {
                    return;
                }
                else 
                {
                    // TO BE REMOVED FROM THE FINAL CODE
                    Console.WriteLine("Something Went Wrong((");
                }
            }
        }        
    }
}
