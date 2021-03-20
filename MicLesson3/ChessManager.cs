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

        private static char AlgorithmChoice;

        // Run the chess game with Queen, Rook
        public static void RunWinningAlgorithm(Color turn, List<KeyValuePair<Color, Type>> figures)
        {
            StartTheGame(turn, figures);
            while (!IsMate() && !IsStaleMate())
            {
                Play();
            }
        }

        // Count the minimal steps of Knight
        public static void RunKnightAlgorithm(List<KeyValuePair<Color, Type>> figures)
        {
            StartKinghtAlgorithmGame(figures);
            DrawOneFigureBoard();
            Cell cellTo = GetUserMoveTo();

            GetKnightMinimalMovesCount1(cellTo);
            GetKnightMinimalMovesCount2(cellTo);
        }


        public static void StartKinghtAlgorithmGame(List<KeyValuePair<Color, Type>> figures)
        {
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
        }

        /// <summary>
        /// Get the custom game position from user
        /// </summary>
        /// <param name="turn">Defines whos turn it is to play according to given color</param>
        /// <param name="figures">List of figures to be created and added to board</param>
        private static void StartTheGame(Color turn, List<KeyValuePair<Color, Type>> figures)
        {
            DrawBoard();
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
                Console.Clear();
                DrawBoard();
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
            King king = GetTheKing(_turn);

            if (IsUnderCheckCell(king.CurrentCell, king.Color))
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

        #region User's turn to play
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
        #endregion

        #region Algorithm to get the minimum path from the current position of Knight to the given cell

        /// <summary>
        /// Prints the minimal number of moves needed from the exisitng Knight figure on the board to the given cell (using queue)
        /// </summary>
        /// <param name="CellTo">Cell the Knigth figure must reach to</param>
        private static void GetKnightMinimalMovesCount1(Cell CellTo)
        {
            // Get the Knight object from the Board
            Knight knight = GetFigure(typeof(Knight), Color.White) as Knight;

            // Collection to hold all the possible moves of knight, cells will be enqueued consecutively, dequeued if cell is not the target cell 
            Queue<Cell> allPossibleCells = new Queue<Cell>();

            // Counter for number of dequeued cells: it allows to count the number of moves knight need to reach to that cell
            int dequeuedIndex = 0;
            // start with enqueuing the current position
            allPossibleCells.Enqueue(knight.CurrentCell);

            // Enqueue and Dequeue cells until we get to the target cell
            while (allPossibleCells.Peek() != CellTo)
            {
                dequeuedIndex++;

                var nextMoveCells = GetKnightMoves(allPossibleCells.Dequeue());
                foreach (var cell in nextMoveCells)
                {
                    allPossibleCells.Enqueue(cell);
                }
            }

            // Convert the number of dequeued cells to the number of moves
            int powerOfEight = -1;
            int counter = 0;

            while (counter <= dequeuedIndex)
            {
                powerOfEight++;
                counter = counter + (int)Math.Pow(8, powerOfEight);
            }

            Console.WriteLine($"You need {powerOfEight} move(s). (Algorithm 1: Queue)");
        }

        /// <summary>
        /// For the given cell it returns a list of cells that the Knight figure would have reached
        /// </summary>
        /// <param name="cellFrom">Cell from where the Knight figure would start its moves</param>
        /// <returns>List of cells that the Knigth figure would have reached from the given cell</returns>
        private static List<Cell> GetKnightMoves(Cell cellFrom)
        {
            int letter = cellFrom.Letter;
            int number = cellFrom.Number;
            List<Cell> cells = new List<Cell>();

            // If the cell is out of board, we should add a dummy cell in its place to keep the number of queued cell equal to 8
            Cell dummyCell = new Cell('0', 0);
            for (int i = letter - 2; i <= letter + 2; i++)
            {
                if (i == letter)
                {
                    continue;
                }
                // if it is in limits of board
                if (i >= 65 && i <= 72)
                {
                    if ((number + (3 - Math.Abs(letter - i)) >= 1 && number + (3 - Math.Abs(letter - i)) <= 8))
                    {
                        cells.Add(new Cell((char)i, number + (3 - Math.Abs(letter - i))));
                    }
                    else
                    {
                        cells.Add(dummyCell);
                    }
                    if ((number - (3 - Math.Abs(letter - i)) >= 1 && number - (3 - Math.Abs(letter - i)) <= 8))
                    {
                        cells.Add(new Cell((char)i, number - (3 - Math.Abs(letter - i))));
                    }
                    else
                    {
                        cells.Add(dummyCell);
                    }
                }
                // if it is out of board add two dummy cells
                else
                {
                    cells.Add(dummyCell);
                    cells.Add(dummyCell);
                }
            }

            return cells;
        }


        /// <summary>
        /// Prints the minimal number of moves needed from the exisitng Knight figure on the board to the given cell (using template)
        /// </summary>
        /// <param name="CellTo">Cell the Knigth figure must reach to</param>
        private static void GetKnightMinimalMovesCount2(Cell cellTo)
        {
            // Get the cell from which the knight starts its moves
            Knight knight = GetFigure(typeof(Knight), Color.White) as Knight;
            Cell cellFrom = knight.CurrentCell;

            // Get the relative positing of knight cell to the target cell
            int number = 1 + Math.Abs(cellTo.Number - cellFrom.Number);
            int letter = 65 + Math.Abs(cellTo.Letter - cellFrom.Letter);

            // Call the GetMovesCount with the relative cell location
            int moves = GetMovesCount(new Cell((char)letter, number));

            Console.WriteLine($"You need {moves} move(s). (Algorithm 2: Template)");
        }

        /// <summary>
        /// Returns the number of moves needed for getting from relative cell <param name="cellFrom"> to fictive target 'A1'
        /// </summary>
        /// <param name="cellFrom">Cell from which the moves to A1 are calculated and returned</param>
        /// <returns>Number of moves needed to reach A1</returns>
        private static int GetMovesCount(Cell cellFrom)
        {
            // Get the Default tempalte (with fictive target cell 'A1')
            int[,] tempalte = CreateTemplate();

            // Convert Cell location to i and j indexes of 8x8 bi-dimentional array
            (int, int) indexes = ConvertCellToIndexes(cellFrom);

            // Return the respective element which represents the number of moves
            return tempalte[indexes.Item1, indexes.Item2];
        }


        /// <summary>
        /// Generates and returns the default template of moves on 8x8 board, having A1 at the upper left corner.
        /// </summary>
        /// <returns>Bi-Dimentional array of integers that represents the number of moves from specific cell to A1.</returns>
        private static int[,] CreateTemplate()
        {
            int[,] fixedTemplate = new int[,]
            {
               { 0,3,2,3,2,3,4,5},
               { 3,2,1,2,3,4,3,4},
               { 2,1,4,3,2,3,4,5},
               { 3,2,3,2,3,4,3,4},
               { 2,3,2,3,4,3,4,5},
               { 3,4,3,4,3,4,5,4},
               { 4,3,4,3,4,5,4,5},
               { 5,4,5,4,5,4,5,6},
            };

            return fixedTemplate;
        }

        /// <summary>
        /// Converts board cell location to i and j indexes for 8x8 bi-dimentional array
        /// </summary>
        /// <param name="cell">Cell to convert to indexes</param>
        /// <returns>Tuple of i and j indexes</returns>
        private static (int, int) ConvertCellToIndexes(Cell cell)
        {
            int i = cell.Number - 1;
            int j = cell.Letter - 65;

            return (i, j);
        }
        #endregion

        #region Algorithm for end game with Queen and Rook
        /// <summary>
        /// At the very begining chooses the alrogithm for pressing black king
        /// Checks if there can be an end game with one Queen move or one Rook move, if not, continues with algorithm steps
        /// </summary>
        private static void TryWinningAlgorithm()
        {
            if (!TryWinningWithQueenMove() && !TryWinningWithRookMove() && !TrySavingRook())
            {
                switch (AlgorithmChoice)
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
                        AlgorithmChoice = PressingRelativelyToWhiteKing();
                        break;
                }
            }

        }


        /// <summary>
        /// Check the positing of white and black kings to define the better algorithm for pressing black king: opposite side of white king
        /// </summary>
        /// <returns>'D' if PressingDown is chosen, 'U' if PressingUp is chosen, 'L' if PressingLeft is chosen, 'R' if PressingRight is chosen</returns>
        private static char PressingRelativelyToWhiteKing()
        {
            Color oppositeColor = _turn == Color.White ? Color.Black : Color.White;

            King oppositeKing = GetTheKing(oppositeColor);
            Cell oppositeKingCell = oppositeKing.CurrentCell;

            King king = GetTheKing(_turn);
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
        private static bool PressingDown()
        {
            Color oppositeColor = _turn == Color.White ? Color.Black : Color.White;
            King oppositeKing = GetTheKing(oppositeColor);

            King king = GetTheKing(_turn);

            Queen queen = GetFigure(typeof(Queen), _turn) as Queen;
            Cell queenCellFrom = queen.CurrentCell;

            Rook rook = GetFigure(typeof(Rook), _turn) as Rook;
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
                        UpdateBoard(rookCellFrom, rookCellTo);
                        _turn = oppositeColor;
                        return true;
                    }
                }
                // moves away of king
                else
                {
                    if (oppositeKing.CurrentCell.Letter >=69)
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
                    UpdateBoard(rookCellFrom, rookCellTo);
                    _turn = oppositeColor;
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
                            UpdateBoard(queenCellFrom, queenCellTo);
                            _turn = oppositeColor;
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

                            UpdateBoard(queenCellFrom, queenCellTo);
                            _turn = oppositeColor;
                            if (IsStaleMate()) 
                            {
                                queen.Move(queenCellFrom);
                                UpdateBoard(queenCellTo,queenCellFrom);
                                _turn = queen.Color;
                                return TryMovingKing() || TryMovingQueen();
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
        private static bool PressingUp()
        {
            Color oppositeColor = _turn == Color.White ? Color.Black : Color.White;
            King oppositeKing = GetTheKing(oppositeColor);

            King king = GetTheKing(_turn);

            Queen queen = GetFigure(typeof(Queen), _turn) as Queen;
            Cell queenCellFrom = queen.CurrentCell;

            Rook rook = GetFigure(typeof(Rook), _turn) as Rook;
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
                        UpdateBoard(rookCellFrom, rookCellTo);
                        _turn = oppositeColor;
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
                    UpdateBoard(rookCellFrom, rookCellTo);
                    _turn = oppositeColor;
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
                            UpdateBoard(queenCellFrom, queenCellTo);
                            _turn = oppositeColor;
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

                            UpdateBoard(queenCellFrom, queenCellTo);
                            _turn = oppositeColor;
                            if (IsStaleMate())
                            {
                                queen.Move(queenCellFrom);
                                UpdateBoard(queenCellTo, queenCellFrom);
                                _turn = queen.Color;
                                return TryMovingKing() || TryMovingQueen();
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
        private static bool PressingLeft()
        {
            Color oppositeColor = _turn == Color.White ? Color.Black : Color.White;
            King oppositeKing = GetTheKing(oppositeColor);

            King king = GetTheKing(_turn);

            Queen queen = GetFigure(typeof(Queen), _turn) as Queen;
            Cell queenCellFrom = queen.CurrentCell;

            Rook rook = GetFigure(typeof(Rook), _turn) as Rook;
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
                        UpdateBoard(rookCellFrom, rookCellTo);
                        _turn = oppositeColor;
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
                    UpdateBoard(rookCellFrom, rookCellTo);
                    _turn = oppositeColor;
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
                            UpdateBoard(queenCellFrom, queenCellTo);
                            _turn = oppositeColor;
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

                            UpdateBoard(queenCellFrom, queenCellTo);
                            _turn = oppositeColor;
                            if (IsStaleMate())
                            {
                                queen.Move(queenCellFrom);
                                UpdateBoard(queenCellTo, queenCellFrom);
                                _turn = queen.Color;
                                return TryMovingKing() || TryMovingQueen();
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
        private static bool PressingRight()
        {
            Color oppositeColor = _turn == Color.White ? Color.Black : Color.White;
            King oppositeKing = GetTheKing(oppositeColor);

            King king = GetTheKing(_turn);

            Queen queen = GetFigure(typeof(Queen), _turn) as Queen;
            Cell queenCellFrom = queen.CurrentCell;

            Rook rook = GetFigure(typeof(Rook), _turn) as Rook;
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
                        UpdateBoard(rookCellFrom, rookCellTo);
                        _turn = oppositeColor;
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
                    UpdateBoard(rookCellFrom, rookCellTo);
                    _turn = oppositeColor;
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
                            UpdateBoard(queenCellFrom, queenCellTo);
                            _turn = oppositeColor;
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

                            UpdateBoard(queenCellFrom, queenCellTo);
                            _turn = oppositeColor;
                            if (IsStaleMate())
                            {
                                queen.Move(queenCellFrom);
                                UpdateBoard(queenCellTo, queenCellFrom);
                                _turn = queen.Color;
                                return TryMovingKing() || TryMovingQueen();
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
        private static bool TryWinningWithQueenMove()
        {
            Color playerColor = _turn;
            Color oppositeColor = _turn == Color.White ? Color.Black : Color.White;

            King oppositeKing = GetTheKing(oppositeColor);
            Queen queen = GetFigure(typeof(Queen), _turn) as Queen;
            Rook rook = GetFigure(typeof(Rook), _turn) as Rook;
            King king = GetTheKing(_turn);

            Cell cellFrom = queen.CurrentCell;
            List<Cell> currentInfuencedCells = queen.InfluencedCells;

            foreach (var cellTo in currentInfuencedCells)
            {
                if (IsPossibleToMove(queen, cellTo) && IsFreeCell(cellTo) &&
                    ((Math.Abs(cellTo.Number - oppositeKing.CurrentCell.Number) >= 2 && Math.Abs(cellTo.Letter - oppositeKing.CurrentCell.Letter) >= 2)
                        || IsPossibleToMove(rook,cellTo) || IsPossibleToMove(king,cellTo)))
                {
                    queen.Move(cellTo);
                    _turn = oppositeColor;
                    if (IsMate())
                    {
                        UpdateBoard(cellFrom, cellTo);

                        return true;
                    }
                    else
                    {
                        queen.Move(cellFrom);
                        _turn = playerColor;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Move the rook to all its possible cells to check if there will be a Mate situation on one of them
        /// </summary>
        /// <returns>True if moveing rook can result in Mate, False if no Mate situation is possible for this positing</returns>
        private static bool TryWinningWithRookMove()
        {
            Color oppositeColor = _turn == Color.White ? Color.Black : Color.White;

            King oppositeKing = GetTheKing(oppositeColor);
            Queen queen = GetFigure(typeof(Queen), _turn) as Queen;
            Rook rook = GetFigure(typeof(Rook), _turn) as Rook;
            King king = GetTheKing(_turn);
            Cell cellFrom = rook.CurrentCell;

            foreach (var cellTo in rook.InfluencedCells)
            {
                if (IsPossibleToMove(queen, cellTo) && IsFreeCell(cellTo) &&
                   ((Math.Abs(cellTo.Number - oppositeKing.CurrentCell.Number) >= 2 && Math.Abs(cellTo.Letter - oppositeKing.CurrentCell.Letter) >= 2)
                       || IsPossibleToMove(queen,cellTo) || IsPossibleToMove(king,cellTo)))
                {
                    rook.Move(cellTo);
                    if (IsMate())
                    {
                        UpdateBoard(cellFrom, cellTo);
                        _turn = oppositeColor;
                        return true;
                    }
                    else
                    {
                        rook.Move(cellFrom);
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Checks if there is a need to move thr Rook to save it from opposite King
        /// </summary>
        /// <returns>True if moved, False if no need to move</returns>
        private static bool TrySavingRook()
        {
            Color oppositeColor = _turn == Color.White ? Color.Black : Color.White;
            King oppositeKing = GetTheKing(oppositeColor);

            Queen queen = GetFigure(typeof(Queen), _turn) as Queen;
            Rook rook = GetFigure(typeof(Rook), _turn) as Rook;
            King king = GetTheKing(_turn);
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
                        switch (AlgorithmChoice)
                        {
                            case 'U':
                            case 'D':
                                if (oppositeKing.CurrentCell.Letter >= 69)
                                {
                                    rookCellTo = new Cell('A', rookCellFrom.Number);
                                    if (!(IsPossibleToMove(rook, rookCellTo) && IsFreeCell(rookCellTo)))
                                    {
                                        rookCellTo = new Cell((char)(king.CurrentCell.Letter + 1), rookCellFrom.Number);
                                    }

                                }
                                else
                                {
                                    rookCellTo = new Cell('H', rookCellFrom.Number);
                                    if (!(IsPossibleToMove(rook, rookCellTo) && IsFreeCell(rookCellTo)))
                                    {
                                        rookCellTo = new Cell((char)(king.CurrentCell.Letter - 1), rookCellFrom.Number);

                                    }
                                }
                                break;
                            default:

                                if (oppositeKing.CurrentCell.Number >= 5)
                                {
                                    rookCellTo = new Cell(rookCellFrom.Letter, 1);
                                    if (!(IsPossibleToMove(rook, rookCellTo) && IsFreeCell(rookCellTo)))
                                    {
                                        rookCellTo = new Cell(rookCellFrom.Letter, king.CurrentCell.Number + 1);

                                    }
                                }
                                else
                                {
                                    rookCellTo = new Cell(rookCellFrom.Letter, 8);
                                    if (!(IsPossibleToMove(rook, rookCellTo) && IsFreeCell(rookCellTo)))
                                    {
                                        rookCellTo = new Cell(rookCellFrom.Letter, king.CurrentCell.Number - 1);

                                    }
                                }
                                break;
                        }

                        rook.Move(rookCellTo);
                        UpdateBoard(rookCellFrom, rookCellTo);
                        _turn = oppositeColor;
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
        private static bool TryMovingKing()
        {
            Color oppositeColor = _turn == Color.White ? Color.Black : Color.White;

            King king = GetTheKing(_turn);
            Cell kingCellFrom = king.CurrentCell;
            Rook rook = GetFigure(typeof(Rook),_turn) as Rook;


            switch (AlgorithmChoice)
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
                            UpdateBoard(kingCellFrom, kingCellTo);
                            _turn = oppositeColor;
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
                            UpdateBoard(kingCellFrom, kingCellTo);
                            _turn = oppositeColor;
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
        private static bool TryMovingQueen() 
        {
            Color oppositeColor = _turn == Color.White ? Color.Black : Color.White;

            King oppositeKing = GetTheKing(oppositeColor);
            Cell kingCellFrom = oppositeKing.CurrentCell;

            Queen queen = GetFigure(typeof(Queen), _turn) as Queen;
            Cell queenCellFrom = queen.CurrentCell;
            switch (AlgorithmChoice) 
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
                                UpdateBoard(queenCellFrom, queenCellTo);
                                _turn = oppositeColor;
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
                                UpdateBoard(queenCellFrom, queenCellTo);
                                _turn = oppositeColor;
                                return true;
                            }
                        }
                    }
                    break;
            }

            return false;
        }

        private static bool OpenRookPath() 
        {
            Rook rook = GetFigure(typeof(Rook), _turn) as Rook;
            Queen queen = GetFigure(typeof(Queen), _turn) as Queen;
            King king = GetTheKing(_turn);

            switch (AlgorithmChoice) 
            {
                case 'U':
                    if (rook.CurrentCell.Letter == king.CurrentCell.Letter && rook.CurrentCell.Number == king.CurrentCell.Number + 1)
                    {
                        return TryMovingKing();
                    }
                    else
                    {
                        return TryMovingQueen();
                    }
                case 'D':
                    if (rook.CurrentCell.Letter == king.CurrentCell.Letter && rook.CurrentCell.Number == king.CurrentCell.Number - 1)
                    {
                        return TryMovingKing();
                    }
                    else 
                    {
                        return TryMovingQueen();
                    }
                case 'L':
                    if (rook.CurrentCell.Number == king.CurrentCell.Number && rook.CurrentCell.Letter == king.CurrentCell.Letter - 1)
                    {
                        return TryMovingKing();
                    }
                    else
                    {
                        return TryMovingQueen();
                    }
                case 'R':
                    if (rook.CurrentCell.Number == king.CurrentCell.Number && rook.CurrentCell.Letter == king.CurrentCell.Letter + 1)
                    {
                        return TryMovingKing();
                    }
                    else
                    {
                        return TryMovingQueen();
                    }
            }
            return false;
        }
        #endregion
    }

}