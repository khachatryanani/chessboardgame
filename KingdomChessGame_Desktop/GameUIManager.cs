using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ChessEngineLogic;
using System.Threading.Tasks;

namespace KingdomChessGame_Desktop
{
    /// <summary>
    /// Manages the UI changes during the game, interacts with Chess Engine
    /// </summary>
    public class GameUIManager
    {
        /// <summary>
        /// Chess Engine that is responsible for game logic
        /// </summary>
        private ChessEngine _engine;

        /// <summary>
        /// Predefined colors for board cells on UI
        /// </summary>
        private readonly Brush _selectedBlackColor = (Brush)(new BrushConverter().ConvertFrom("#F85F9E"));
        private readonly Brush _selectedWhiteColor = (Brush)(new BrushConverter().ConvertFrom("#FDB6B6"));
        private readonly Brush _blackColor = (Brush)(new BrushConverter().ConvertFrom("#67D07D"));

        public string MessageText { get; set; }

        /// <summary>
        /// Holds the string representations of Figure names that are to be created for the current game.
        /// </summary>
        public List<string> FiguresForGame { get; set; }

        /// <summary>
        /// Holds the list of images in Main Grid created for the current game.
        /// </summary>
        public List<Image> FigureImages { get; set; }

        /// <summary>
        /// Holds the list of grids formming the chess board on UI.
        /// </summary>
        public List<Grid> GridBoard { get; set; }

        /// <summary>
        /// Indicates the game chosen by the user via UI.
        /// </summary>
        public int GameChoice { get; set; }

        /// <summary>
        /// Contructor initializes fields.
        /// </summary>
        public GameUIManager()
        {
            SetNewGame();
        }

        /// <summary>
        /// Initializes the objecs required for the game.
        /// </summary>
        private void SetNewGame() 
        {
            FiguresForGame = new List<string>();
            FigureImages = new List<Image>();
            GridBoard = new List<Grid>();

            // To be changed into a dynamic choice
            _engine = new ChessEngine("Black");
            _engine.FigureMoved += FigureMove;

        }

        /// <summary>
        /// Resets the objecs required for the game.
        /// </summary>
        public void ResetGame() 
        {
            SetNewGame();
            
        }

        /// <summary>
        /// For the given Figure checks if the grid to move on is a valid cell in terms of chess game.
        /// </summary>
        /// <param name="cellFrom">Grid Name that the moving figure stansd on.</param>
        /// <param name="cellTo">Grid Name that the figure wants to move on.</param>
        /// <returns></returns>
        public bool IsValidCellToMove(string cellFrom, string cellTo)
        {
            var possibleMoves = _engine.GetPossibleMoves(cellFrom);
            return possibleMoves.Contains(cellTo);
        }

        /// <summary>
        /// Puts the moving image on the center of the given grid.
        /// </summary>
        /// <param name="imageToMove">Image object that is being moved and placed.</param>
        /// <param name="gridToMoveTo">Grid that Image objects moves to.</param>
        public void InsertImage(Image imageToMove, Grid gridToMoveTo)
        {
            imageToMove.Margin = new Thickness(gridToMoveTo.Margin.Left, gridToMoveTo.Margin.Top, 0, 0);
            imageToMove.Tag = gridToMoveTo.Name;
        }

        /// <summary>
        /// Get the Image object from FigureImages collection based on the given name.
        /// </summary>
        /// <param name="figureName">Name of the figure which image should be found.</param>
        /// <returns>Image object of the relevant figure.</returns>
        private Image GetImageByFigureName(string figureName)
        {
            foreach (var image in FigureImages)
            {
                if (image.Name == figureName)
                {
                    return image;
                }
            }

            return null;
        }

        /// <summary>
        ///  EventHandler method that is subscribed to figure moves: on any figure move updates the UI accordingly.
        /// </summary>
        /// <param name="sender">Chess Engine object</param>
        /// <param name="e">GameEvent arguments</param>
        private void FigureMove(object sender, GameEventArgs e)
        {
            double top = GetGridByName(e.CellTo).Margin.Top;
            double left = GetGridByName(e.CellTo).Margin.Left;

            Image image = GetImageByFigureName(e.MovedFigure);

            image.Margin = new Thickness(left, top, 0, 0);
            image.Tag = e.CellTo;
            
            this.MessageText = GetMessageTextForDefaultGame(e.GameStatus, e.CurrentPlayer, e.WinnerPlayer);
        }

        /// <summary>
        /// Place image back to its initial place on the UI.
        /// </summary>
        /// <param name="figureImage">Image of the figure to be placed back.</param>
        public void ResetImage(Image figureImage)
        {
            var margins = GetImageDefaultMargins(figureImage.Name);

            figureImage.Width = 80;
            figureImage.Height = 80;
            figureImage.Margin = new Thickness(margins.Item1, margins.Item2, 0, 0);
            figureImage.Tag = null;
        }

        /// <summary>
        /// Returns the grid object based on the name.
        /// </summary>
        /// <param name="name">Chess board cell coordinate as a grid name.</param>
        /// <returns>Grid object</returns>
        public Grid GetGridByName(string name)
        {
            foreach (var grid in GridBoard)
            {
                if (grid.Name == name)
                {
                    return grid;
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the game info and returns the relevant game message.
        /// </summary>
        /// <param name="status">Status of the game: 0 = usual, 1 = check, 2 = mate, 3 = stalemate</param>
        /// <param name="currentPlayer">Current player's color</param>
        /// <param name="winner">Winner's color if any winner.</param>
        /// <returns>Message text</returns>
        public string GetMessageTextForDefaultGame(int status, string currentPlayer, string winner) 
        {
            switch (status) 
            {
                case 1:
                    return $"{currentPlayer} King is under check.";
                case 2:
                    return $"Mate! {winner} win.";
                case 3:
                    return "Stalemate...";
                default:
                    return $"Good luck! {currentPlayer}'s turn to play.";
            }
        }

        /// <summary>
        /// Changes the bakcground colors of grids that represent the possible-to-move cells.
        /// </summary>
        /// <param name="cellName">Current grid name that the figure stands on.</param>
        public void AddHelpers(string cellName)
        {
            var possibleMoves = _engine.GetPossibleMoves(cellName);

            foreach (var cell in possibleMoves)
            {
                Grid grid = GetGridByName(cell);
                grid.Background = grid.Background == _blackColor ? _selectedBlackColor : _selectedWhiteColor;
            }
        }

        /// <summary>
        /// Resets the bakcground colors of grids that represent the possible-to-move cells.
        /// </summary>
        /// <param name="cellName">Name of the grid that the figure was standing on.</param>
        public void RemoveHelpers(string cellName)
        {
            var possibleMoves = _engine.GetPossibleMoves(cellName);

            foreach (var cell in possibleMoves)
            {
                Grid grid = GetGridByName(cell);
                grid.Background = grid.Background == _selectedBlackColor ? _blackColor : Brushes.White;
            }
        }

        /// <summary>
        /// Checks if all the required figure images are moved to the main chess board.
        /// </summary>
        /// <returns>True if all the required figures are placed on the board, False if not.</returns>
        public bool AreAllFiguresOnBoard()
        {
            foreach (var image in FigureImages)
            {
                if (image.Tag == null)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Plays the endgame's algorithm.
        /// </summary>
        /// <param name="cellFrom">Name of the grid the figure moved from.</param>
        /// <param name="cellTo">Name of the grid the figure moved to.</param>
        public void Play(string cellFrom, string cellTo)
        {
            switch (GameChoice) 
            {
                case 4:
                    var moves = _engine.PlayMinKinghtMovesAlgorithm(cellFrom, cellTo);
                    Grid[] grids = new Grid[moves.Count];
                    for (int i = 0; i < moves.Count; i++)
                    {
                        grids[i] = GetGridByName(moves[i]);
                    }
                    this.MessageText = $"Knight needs {moves.Count - 1} move(s) to reach from " +
                                       $"{(string)FigureImages[1].Tag} to {(string)FigureImages[0].Tag}";
                    AnimateMoveKnight(FigureImages[1], grids);
                    break;
                case 2:
                    _engine.PlayUser(cellFrom, cellTo);
                    _engine.PlayWinningWithQueenAndRookAlgorithm();
                    break;
                case 3:
                    _engine.PlayUser(cellFrom, cellTo);
                    _engine.PlayWinningWithQueenAndTwoRooksAlgorithm();
                    break;
            }
        }

        /// <summary>
        /// Creates images of figures needed for the chosen game and placed them on the initial position.
        /// </summary>
        /// <returns>List of created images.</returns>
        public List<Image> CreateFigureImages()
        {
            FigureImages = new List<Image>();
            foreach (var figureName in FiguresForGame)
            {
                var margins = GetImageDefaultMargins(figureName);
                Image figureImage = new Image
                {
                    Name = figureName,
                    Stretch = Stretch.Fill,
                    Margin = new Thickness(margins.Item1, margins.Item2, 0, 0),
                    Width = 80,
                    Height = 80,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Top,
                    Source = GetImageSource(figureName)
                };

                FigureImages.Add(figureImage);
            }

            return FigureImages;
        }

        /// <summary>
        /// Creates a Chess Board from Grid objects.
        /// </summary>
        /// <returns>List of Grid that form the Chess Board.</returns>
        public List<Grid> CreateBoard()
        {
            Brush brushes = _blackColor;
            int marginTop = 150;

            for (int i = 0; i < 8; i++)
            {
                int marginLeft = 260;
                brushes = brushes == _blackColor? Brushes.White : _blackColor;
                for (int j = 0; j < 8; j++)
                {
                    string number = (8 - i).ToString();
                    char letter = (char)(j + 65);

                    Grid grid = new Grid
                    {
                        Name = letter + number,
                        Width = 60,
                        Height = 60,
                        Margin = new Thickness(marginLeft, marginTop, 0, 0),
                        VerticalAlignment = VerticalAlignment.Top,
                        HorizontalAlignment = HorizontalAlignment.Left,
                        Background = brushes,
                        IsEnabled = false,
                    };

                    GridBoard.Add(grid);

                    marginLeft += 59;
                    brushes = brushes == _blackColor ? Brushes.White : _blackColor;
                }
                marginTop += 59;
            }

            return GridBoard;
        }

        /// <summary>
        /// Based on the figures name gets the initial position coordinates.
        /// </summary>
        /// <param name="figureImageName">Name of the figure to be placed.</param>
        /// <returns>Tuple of initial left and top coordinates on the main window.</returns>
        private (double, double) GetImageDefaultMargins(string figureImageName)
        {
            double marginTop;
            double marginLeft = 280;

            if (figureImageName[0] == 'B')
            {
                marginTop = 30;

            }
            else
            {
                marginTop = 680;

            }

            switch (figureImageName[1])
            {
                case 'Q':
                    marginLeft += 1 * 70;
                    break;
                case 'R':
                    marginLeft += 2 * 70;
                    break;
                case 'B':
                    marginLeft += 3 * 70;
                    break;
                case 'N':
                    marginLeft += 4 * 70;
                    break;
                case 'P':
                    marginLeft += 5 * 70;
                    break;
                default:
                    break;
            }

            return (marginLeft, marginTop);
        }

        /// <summary>
        /// Gets the Image source based on teh figure name.
        /// </summary>
        /// <param name="figureName">Name of the figure to get the image source for.</param>
        /// <returns>BitmapImage as source for the image.</returns>
        private BitmapImage GetImageSource(string figureName)
        {
            if (figureName[0] == 'B')
            {
                switch (figureName[1])
                {
                    case 'K':
                        return new BitmapImage(new Uri(@"\Images\BK.png", UriKind.Relative));
                    case 'Q':
                        return new BitmapImage(new Uri(@"\Images\BQ.png", UriKind.Relative));
                    case 'R':
                        return new BitmapImage(new Uri(@"\Images\BR.png", UriKind.Relative));
                    case 'B':
                        return new BitmapImage(new Uri(@"\Images\BB.png", UriKind.Relative));
                    case 'N':
                        if (figureName.Length == 2)
                        {
                            return new BitmapImage(new Uri(@"\Images\BN.png", UriKind.Relative));
                        }
                        return new BitmapImage(new Uri(@"\Images\phantom.png", UriKind.Relative));
                    default:
                        return new BitmapImage(new Uri(@"\Images\BP.png", UriKind.Relative));
                }
            }
            else
            {
                return (figureName[1]) switch
                {
                    'K' => new BitmapImage(new Uri(@"\Images\WK.png", UriKind.Relative)),
                    'Q' => new BitmapImage(new Uri(@"\Images\WQ.png", UriKind.Relative)),
                    'R' => new BitmapImage(new Uri(@"\Images\WR.png", UriKind.Relative)),
                    'B' => new BitmapImage(new Uri(@"\Images\WB.png", UriKind.Relative)),
                    'N' => new BitmapImage(new Uri(@"\Images\WN.png", UriKind.Relative)),
                    _ => new BitmapImage(new Uri(@"\Images\WP.png", UriKind.Relative)),
                };
            }
        }

        /// <summary>
        /// Pn game start will send the required parameters ot create a figure for chess engine.
        /// </summary>
        /// <param name="cellString">Cell name the figure stands on.</param>
        /// <param name="typeString">Type name of the figure to be created.</param>
        /// <param name="colorString">Color name of the figure to be created.</param>
        public void CreateFigure(string cellString, string typeString, string colorString)
        {
            _engine.CreateFigure(cellString, typeString, colorString);
            this.MessageText = GetMessageTextForDefaultGame(0, "Black", string.Empty);
        }

        /// <summary>
        /// Asyncronously moves the knight figure's image on the window accrodig the given path.
        /// </summary>
        /// <param name="image">Image to move.</param>
        /// <param name="grids">Grids that represent the path.</param>
        private async void AnimateMoveKnight(Image image, Grid[] grids)
        {
            foreach (var grid in grids)
            {
                await Task.Run(() =>
                {
                    Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        image.Margin = new Thickness(grid.Margin.Left, grid.Margin.Top, 0, 0);
                        image.Tag = grid.Name;
                        grid.Tag = image;
                    }));
                    Task.Delay(1000).Wait();
                });
            }
        }

        /// <summary>
        /// Puts the image on its default position on the chess board
        /// </summary>
        /// <param name="figureImage">Figure's image to put on its default position on chess board</param>
        public void PutFigureOnDefaultPosition()
        {
            foreach (var figureImage in FigureImages)
            {
                figureImage.Width = 60;
                figureImage.Height = 60;
                Grid grid;

                if (figureImage.Name[0] == 'W')
                {
                    switch (figureImage.Name[1])
                    {
                        case 'K':
                            grid = GetEmtpyGridByName("E1");
                            break;
                        case 'Q':
                            grid = GetEmtpyGridByName("D1");
                            break;
                        case 'B':
                            grid = GetEmtpyGridByName("C1") ?? GetEmtpyGridByName("F1");
                            break;
                        case 'N':
                            grid = GetEmtpyGridByName("B1") ?? GetEmtpyGridByName("G1");
                            break;
                        case 'R':
                            grid = GetEmtpyGridByName("A1") ?? GetEmtpyGridByName("H1");
                            break;
                        default:
                            grid = GetEmtpyGridByName("A2") ?? GetEmtpyGridByName("B2") ?? GetEmtpyGridByName("C2") ?? GetEmtpyGridByName("D2") ??
                                 GetEmtpyGridByName("E2") ?? GetEmtpyGridByName("F2") ?? GetEmtpyGridByName("G2") ?? GetEmtpyGridByName("H2");
                            break;
                    }
                }
                else
                {
                    switch (figureImage.Name[1])
                    {
                        case 'K':
                            grid = GetEmtpyGridByName("E8");
                            break;
                        case 'Q':
                            grid = GetEmtpyGridByName("D8");
                            break;
                        case 'B':
                            grid = GetEmtpyGridByName("C8") ?? GetEmtpyGridByName("F8");
                            break;
                        case 'N':
                            grid = GetEmtpyGridByName("B8") ?? GetEmtpyGridByName("G8");
                            break;
                        case 'R':
                            grid = GetEmtpyGridByName("A8") ?? GetEmtpyGridByName("H8");
                            break;
                        default:
                            grid = GetEmtpyGridByName("A7") ?? GetEmtpyGridByName("B7") ?? GetEmtpyGridByName("C7") ?? GetEmtpyGridByName("D7") ??
                                 GetEmtpyGridByName("E7") ?? GetEmtpyGridByName("F7") ?? GetEmtpyGridByName("G7") ?? GetEmtpyGridByName("H7");
                            InsertImage(figureImage, grid);
                            break;
                    }
                }

                InsertImage(figureImage, grid);
            }
        }

        /// <summary>
        /// Gets a grid if it's empty
        /// </summary>
        /// <param name="name">Name of the grid to get</param>
        /// <returns>Grid if it is empty, Null if the grid was not found or was not empty</returns>
        private Grid GetEmtpyGridByName(string name)
        {
            foreach (var grid in GridBoard)
            {
                if (grid.Name == name && IsEmptyGrid(grid))
                {
                    return grid;
                }
            }

            return null;
        }

        /// <summary>
        /// Checks if there is an image standing on the grid.
        /// </summary>
        /// <param name="grid">Grid to check.</param>
        /// <returns>True if there is no image on the grid, False if there is a one.</returns>
        private bool IsEmptyGrid(Grid grid) 
        {
            string gridName = grid.Name;
            foreach (var image in FigureImages)
            {
                if ((string)image.Tag == gridName) 
                {
                    return false;
                }
            }

            return true;
        }
    }
}
