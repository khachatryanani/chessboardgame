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
        private readonly ChessEngine _engine;


        /// <summary>
        /// Predefined colors for board cells on UI
        /// </summary>
        private readonly Brush _selectedBlackColor;
        private readonly Brush _selectedWhiteColor;


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
            FiguresForGame = new List<string>();
            FigureImages = new List<Image>();
            GridBoard = new List<Grid>();

            _engine = new ChessEngine("Black");
            _engine.FigureMoved += OnFigureMove;

            _selectedBlackColor = (Brush)(new BrushConverter().ConvertFrom("#CF6868"));
            _selectedWhiteColor = (Brush)(new BrushConverter().ConvertFrom("#FDB6B6"));
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
        /// EventHandler method that is subscribed to figure moves: on any figure move updates the UI accordingly.
        /// </summary>
        /// <param name="figureName">Name of the figure that moved</param>
        /// <param name="cellFrom">Chess board cell from which figure moved.</param>
        /// <param name="cellTo">Chess board cell to which figure moved.</param>
        private void OnFigureMove(string figureName, string cellFrom, string cellTo)
        {
            double top = GetGridByName(cellTo).Margin.Top;
            double left = GetGridByName(cellTo).Margin.Left;

            Image image = GetImageByFigureName(figureName);

            image.Margin = new Thickness(left, top, 0, 0);
            image.Tag = cellTo;
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
        /// Changes the bakcground colors of grids that represent the possible-to-move cells.
        /// </summary>
        /// <param name="cellName">Current grid name that the figure stands on.</param>
        public void AddHelpers(string cellName)
        {
            var possibleMoves = _engine.GetPossibleMoves(cellName);

            foreach (var cell in possibleMoves)
            {
                Grid grid = GetGridByName(cell);
                grid.Background = grid.Background == Brushes.RosyBrown ? _selectedBlackColor : _selectedWhiteColor;
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
                grid.Background = grid.Background == _selectedBlackColor ? Brushes.RosyBrown : Brushes.White;
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
            _engine.PlayUser(cellFrom, cellTo);
            _engine.PlayEngine(GameChoice);
        }

        /// <summary>
        /// Plays the other games available in Chess Engine.
        /// </summary>
        public void Play()
        {
            if (GameChoice == 4)
            {
                var moves = _engine.PlayMinKinghtMovesAlgorithm((string)FigureImages[1].Tag, (string)FigureImages[0].Tag);
                Grid[] grids = new Grid[moves.Count];
                for (int i = 0; i < moves.Count; i++)
                {
                    grids[i] = GetGridByName(moves[i]);

                }

                AnimateMoveKnight(FigureImages[1], grids);
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
            SolidColorBrush brushes = Brushes.RosyBrown;
            int marginTop = 150;

            for (int i = 0; i < 8; i++)
            {
                int marginLeft = 260;
                brushes = brushes == Brushes.RosyBrown ? Brushes.White : Brushes.RosyBrown;
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
                    brushes = brushes == Brushes.RosyBrown ? Brushes.White : Brushes.RosyBrown;
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
    }
}
