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

        // Get the custom gam position from user
        public static void StartTheGame(Color turn, List<KeyValuePair<Color, Type>> figures)
        {
            // Create figures of specified color and on specified cell
            foreach (var item in figures)
            {
                Cell cell;
                Console.WriteLine($"Enter chess board start cell for {item.Key.ToString()} {item.Value.Name}");
                do
                {
                    cell = GetUserStartCell();

                }
                while (!IsAcceptableCell(cell, item.Value, item.Key));

                CreateFigure(cell, item.Value, item.Key);
            }

            // Give the turn to Blacks

            _turn = turn;

        }

        /// <summary>
        /// Checks if it is a Mate situation in the game:
        /// if the King is under influence of opposite color figure and has no available cells to move to
        /// </summary>
        /// <returns>True if it is a Mate, false if it is not a Mate</returns>
        public static bool IsMate()
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

        public static bool IsCheck()
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


        public static bool IsStaleMate()
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
        public static void Play()
        {
            if (_turn == Color.Black)
            {
                // Wait for the user to move a figure
                WaitForUserToMove();
            }
            else
            {
                // Play the winning algorithm
                TryNewAlgorithm();
            }
        }

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
        /// Create figures of the specified types and add them to the chess board
        /// </summary>
        /// <param name="c">Letter of the location coordinate</param>
        /// <param name="i">Interger of the location coordinate</param>
        /// <param name="type">Type of figure to be created</param>
        /// <param name="color">Color of the figure to be created</param>
        public static void CreateFigure(Cell cell, Type type, Color color)
        {
            // Create a figure
            Figure figure = (Figure)Activator.CreateInstance(type, cell, color);

            // Add to ChessBoard
            AddFigure(figure);
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

        /// <summary>
        /// Runs an algorithm of steps to win in this specific game load
        /// </summary>
        //private static void TryWinningAlgorithm()
        //{
        //    King king = ChessBoard.GetTheKing(Color.Black);
        //    Cell kingCellFrom = king.CurrentCell;

        //    if (kingCellFrom.Number == 1)
        //    {
        //        // Case1: Play with left Rook if it is not moved yet
        //        Cell rook2CellFrom = new Cell('H', 8);
        //        if (ChessBoard.GetFigureByCell(rook2CellFrom) is Rook rook2)
        //        {
        //            Cell rook2CellTo = new Cell('H', 2);
        //            if (rook2.Moved(rook2CellTo))
        //            {
        //                _turn = Color.Black;
        //                ChessBoard.UpdateBoard(rook2CellFrom, rook2CellTo);
        //            }

        //            return;
        //        }

        //        // Case2: Play with Queen if it stands on the cell E8
        //        Cell queenCellFrom = new Cell('E', 8);
        //        Queen queen = ChessBoard.GetFigureByCell(queenCellFrom) as Queen;
        //        if (queen != null)
        //        {
        //            Cell queenCellTo;
        //            // Move the queen according to King's position
        //            if (kingCellFrom.Letter == 'D')
        //            {
        //                queenCellTo = new Cell('E', 2);
        //            }
        //            else
        //            {
        //                queenCellTo = new Cell('E', 1);
        //            }

        //            if (queen.Moved(queenCellTo))
        //            {
        //                _turn = Color.Black;
        //                ChessBoard.UpdateBoard(queenCellFrom, queenCellTo);
        //            }
        //            return;
        //        }

        //        // Case3: Play with Queen if it stands on the cell E2
        //        queenCellFrom = new Cell('E', 2);
        //        queen = ChessBoard.GetFigureByCell(queenCellFrom) as Queen;
        //        if (queen != null)
        //        {
        //            Cell queenCellTo = new Cell('E', 1);
        //            if (queen.Moved(queenCellTo))
        //            {
        //                _turn = Color.Black;
        //                ChessBoard.UpdateBoard(queenCellFrom, queenCellTo);
        //            }
        //            return;

        //        }

        //        // Case3: Play with Queen if it stands on the cell A4
        //        queenCellFrom = new Cell('A', 4);
        //        queen = ChessBoard.GetFigureByCell(queenCellFrom) as Queen;
        //        if (queen != null)
        //        {
        //            Cell queenCellTo = new Cell('C', 2);
        //            if (queen.Moved(queenCellTo))
        //            {
        //                _turn = Color.Black;
        //                ChessBoard.UpdateBoard(queenCellFrom, queenCellTo);
        //            }
        //            return;
        //        }

        //    }
        //    else
        //    {
        //        // Case4: Play with right Rook if it is not moved yet
        //        Cell rook1CellFrom = new Cell('A', 8);
        //        if (ChessBoard.GetFigureByCell(rook1CellFrom) is Rook rook1)
        //        {
        //            Cell rook1CellTo = new Cell('A', 3);
        //            if (rook1.Moved(rook1CellTo))
        //            {
        //                _turn = Color.Black;
        //                ChessBoard.UpdateBoard(rook1CellFrom, rook1CellTo);
        //            }

        //            return;
        //        }

        //        if (kingCellFrom.Letter == 'B')
        //        {
        //            // Case6: Move with queen if King is on B
        //            Cell queenCellFrom = new Cell('E', 8);
        //            Queen queen = ChessBoard.GetFigureByCell(queenCellFrom) as Queen;
        //            if (queen != null)
        //            {
        //                Cell queenCellTo = new Cell('A', 4);
        //                if (queen.Moved(queenCellTo))
        //                {
        //                    _turn = Color.Black;
        //                    ChessBoard.UpdateBoard(queenCellFrom, queenCellTo);
        //                }
        //                return;
        //            }

        //        }
        //        else
        //        {
        //            // Case7: Play with left Rook
        //            Cell rook2CellFrom = new Cell('H', 8);
        //            if (ChessBoard.GetFigureByCell(rook2CellFrom) is Rook rook2)
        //            {
        //                Cell rook2CellTo = new Cell('H', 2);
        //                if (rook2.Moved(rook2CellTo))
        //                {
        //                    _turn = Color.Black;
        //                    ChessBoard.UpdateBoard(rook2CellFrom, rook2CellTo);
        //                }

        //                return;
        //            }
        //        }

        //    }
        //}

       // private static void TryWinningFromAnyPointAlgorithm()
       // {
       //     Color colorOfOpposite = _turn == Color.Black ? Color.White : Color.Black;
       //     King oppositeKing = GetTheKing(colorOfOpposite);
       //     int number = oppositeKing.CurrentCell.Number;

       //     if (GetFigure(typeof(Queen), _turn) is Queen queen)
       //     {
       //         Cell queenCellFrom = queen.CurrentCell;
       //         if (queenCellFrom.Number != number + 1 && queenCellFrom.Number != number - 1)
       //         {
       //             Cell queenCellTo = new Cell(queenCellFrom.Letter, number);
       //             if (IsPossibleToMove(queen, queenCellTo) && IsFreeCell(queenCellTo))
       //             {
       //                 queen.Move(queenCellTo);
       //                 UpdateBoard(queenCellFrom, queenCellTo);
       //                 _turn = colorOfOpposite;
       //             }
       //         }
       //         else
       //         {
       //             if (GetFigure(typeof(Rook), _turn) is Rook rook1)
       //             {
       //                 Cell rook1CellFrom = rook1.CurrentCell;
       //                 if (Math.Abs(rook1CellFrom.Letter - oppositeKing.CurrentCell.Letter) <= 2)
       //                 {
       //                     Cell rook1CellTo;
       //                     if (Math.Abs(rook1CellFrom.Letter - queen.CurrentCell.Letter) > 1)
       //                     {
       //                         if (rook1CellFrom.Letter > queen.CurrentCell.Letter)
       //                         {

       //                             rook1CellTo = new Cell((char)(queen.CurrentCell.Letter + 1), rook1CellFrom.Number);
       //                         }
       //                         else
       //                         {
       //                             rook1CellTo = new Cell((char)(queen.CurrentCell.Letter - 1), rook1CellFrom.Number);
       //                         }

       //                         if (IsPossibleToMove(rook1, rook1CellTo) && IsFreeCell(rook1CellTo))
       //                         {
       //                             rook1.Move(rook1CellTo);
       //                             UpdateBoard(rook1CellFrom, rook1CellTo);
       //                             _turn = colorOfOpposite;
       //                         }
       //                         else
       //                         {
       //                             Console.WriteLine("Something went wrong!");
       //                         }

       //                         return;
       //                     }
       //                 }

       //                 if (rook1CellFrom.Number != number + 1 && rook1CellFrom.Number != number - 1)
       //                 {
       //                     Cell rook1CellTo = new Cell(rook1CellFrom.Letter, number);

       //                     if (IsPossibleToMove(rook1, rook1CellTo) && IsFreeCell(rook1CellTo))
       //                     {
       //                         rook1.Move(rook1CellTo);
       //                         UpdateBoard(rook1CellFrom, rook1CellTo);
       //                         _turn = colorOfOpposite;
       //                     }
       //                 }
       //                 else
       //                 {
       //                     //if (GetFigure(typeof(Rook), _turn, 2) is Rook rook2)
       //                     //{
       //                     //    Cell rook2CellFrom = rook2.CurrentCell;
       //                     //    Cell rook2CellTo = new Cell(rook2.CurrentCell.Letter, number);
       //                     //    if (IsPossibleToMove(rook2, rook2CellTo) && IsFreeCell(rook2CellTo))
       //                     //    {
       //                     //        rook2.Move(rook2CellTo);
       //                     //        UpdateBoard(rook2CellFrom, rook2CellTo);

       //                     //        _turn = colorOfOpposite;
       //                     //    }
       //                     //    else
       //                     //    {
       //                     //        Console.WriteLine("Something went wrong!");
       //                     //    }
       //                     //    return;
       //                     //}
       //                     if (GetFigure(typeof(Rook), _turn, 2) is Rook rook2)
       //                     {
       //                         Cell rook2CellFrom = rook2.CurrentCell;
       //                         if (Math.Abs(rook2CellFrom.Letter - oppositeKing.CurrentCell.Letter) <= 2)
       //                         {
       //                             Cell rook2CellTo;
       //                             if (Math.Abs(rook2CellFrom.Letter - queen.CurrentCell.Letter) > 1)
       //                             {
       //                                 if (rook2CellFrom.Letter > queen.CurrentCell.Letter)
       //                                 {

       //                                     rook2CellTo = new Cell((char)(queen.CurrentCell.Letter + 1), rook2CellFrom.Number);
       //                                 }
       //                                 else
       //                                 {
       //                                     rook2CellTo = new Cell((char)(queen.CurrentCell.Letter - 1), rook2CellFrom.Number);
       //                                 }

       //                                 if (IsPossibleToMove(rook1, rook2CellTo) && IsFreeCell(rook2CellTo))
       //                                 {
       //                                     rook1.Move(rook2CellTo);
       //                                     UpdateBoard(rook2CellFrom, rook2CellTo);
       //                                     _turn = colorOfOpposite;
       //                                 }
       //                             }
       //                         }

       //                         if (rook2CellFrom.Number != number + 1 && rook2CellFrom.Number != number - 1)
       //                         {
       //                             Cell rook2CellTo = new Cell(rook2CellFrom.Letter, number);

       //                             if (IsPossibleToMove(rook1, rook2CellTo) && IsFreeCell(rook2CellTo))
       //                             {
       //                                 rook1.Move(rook2CellTo);
       //                                 UpdateBoard(rook2CellFrom, rook2CellTo);
       //                                 _turn = colorOfOpposite;
       //                             }
       //                         }
       //                     }


       //                 }
       //             }
       //         }
       ////     }
       //     King king = GetTheKing(_turn);
       //     Cell kingCellFrom = king.CurrentCell;
       //     Cell kingCellTo;
       //     if (kingCellFrom.Letter >= 69)
       //     {
       //         kingCellTo = new Cell((char)(kingCellFrom.Letter - 1), kingCellFrom.Number);

       //     }
       //     else
       //     {
       //         kingCellTo = new Cell((char)(kingCellFrom.Letter + 1), kingCellFrom.Number);
       //     }

       //     if (IsPossibleToMove(king, kingCellTo) && IsFreeCell(kingCellTo))
       //     {
       //         king.Move(kingCellTo);
       //         UpdateBoard(kingCellFrom, kingCellTo);
       //         _turn = colorOfOpposite;
       //     }
       //     else
       //     {
       //         Console.WriteLine("Something went wrong!");
       //     }
       //     return;
       // }

        private static bool TryMovingQueen()
        {
            Color colorOfOpposite = _turn == Color.Black ? Color.White : Color.Black;
            King oppositeKing = GetTheKing(colorOfOpposite);
            int oppositeKingCellNumber = oppositeKing.CurrentCell.Number;

            if (GetFigure(typeof(Queen), _turn) is Queen queen)
            {
                Cell queenCellFrom = queen.CurrentCell;
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
        private static bool TrySavingRooks()
        {
            Color colorOfOpposite = _turn == Color.Black ? Color.White : Color.Black;
            King oppositeKing = GetTheKing(colorOfOpposite);
            if (GetFigure(typeof(Queen), _turn) is Queen queen)
            {
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

        private static void TryNewAlgorithm()
        {
            if (!TrySavingRooks()) 
            {
                if (TryMovingQueen() ||
                    TryMovingFirstRookHorizontally() ||
                    TryMovingFirstRookVertically() ||
                    TryMovingSecondRookHorizontally() || 
                    TryMovingSecondRookVertically()||
                    TryMovingKing())
                {
                    return;
                }
                else 
                {
                    Console.WriteLine("Something Went Wrong((");
                }
            }
           
        }

        
    }
}
