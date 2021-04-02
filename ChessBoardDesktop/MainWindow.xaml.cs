using ChessBoard.Figures;
using ChessBoard.BoardAttributes;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace ChessBoardDesktop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private GameManager _manager;
        private List<string> _figuresToCreate;
        private List<Image> _Figures;
        private Brush _selectedBlackColor = (Brush)(new BrushConverter().ConvertFrom("#CF6868"));
        private Brush _selectedWhiteColor = (Brush)(new BrushConverter().ConvertFrom("#FDB6B6"));

        public MainWindow()
        {
            InitializeFields();
            InitializeComponent();
            InitializeBoard();
            InitializeFigureHolder();
        }

        /// <summary>
        /// Initializing private fields of MainWindow
        /// </summary>
        private void InitializeFields()
        {
            _manager = new GameManager();
            _figuresToCreate = new List<string> 
            {
                "BK", "WK", "BQ", "WQ", "BR","WR", "BR", "WR", "BN","WN", "BN", "WN", "BB","WB", "BB", "WB",
                "BP", "WP", "BP","WP","BP","WP","BP","WP", "BP", "WP","BP","WP","BP","WP","BP","WP"
            };
           

            _Figures = new List<Image>();

            _selectedBlackColor = (Brush)(new BrushConverter().ConvertFrom("#CF6868"));
            _selectedWhiteColor = (Brush)(new BrushConverter().ConvertFrom("#FDB6B6"));
        }

        /// <summary>
        /// Creates Chess Game board with Grid Cells and adds grid to Game Manager's Board Of Desktop
        /// </summary>
        private void InitializeBoard()
        {
            SolidColorBrush brushes = Brushes.RosyBrown;
            int marginTop = 150;

            for (int i = 0; i < 8; i++)
            {
                int marginLeft = 260;
                brushes = brushes == Brushes.RosyBrown ? Brushes.White : Brushes.RosyBrown;
                for (int j = 0; j < 8; j++)
                {
                    Grid grid = new Grid
                    {
                        Name = ConvertIndexesToCell(i, j),
                        Width = 60,
                        Height = 60,
                        Margin = new Thickness(marginLeft, marginTop, 0, 0),
                        VerticalAlignment = VerticalAlignment.Top,
                        HorizontalAlignment = HorizontalAlignment.Left,
                        Background = brushes,
                        IsEnabled = false,
                    };

                    _manager.BoardOfDesktop[grid.Name] = grid;
                    MainGrid.Children.Add(grid);

                    marginLeft += 59;
                    brushes = brushes == Brushes.RosyBrown ? Brushes.White : Brushes.RosyBrown;
                }
                marginTop += 59;
            }
        }  

        /// <summary>
        /// Get the Uri source for the image to be created
        /// </summary>
        /// <param name="figure">Figure for which the image shoul be created</param>
        /// <returns>BitmapImage of the figure to be created</returns>
        private BitmapImage GetImageSource((CellColor, Type) figure)
        {
            bool isBlack = figure.Item1 == CellColor.Black;
            BitmapImage btmImage = new BitmapImage();

            btmImage.BeginInit();
            switch (figure.Item2.Name)
            {
                case "King":
                    btmImage.UriSource = isBlack ? new Uri(@"\Images\BK.png", UriKind.Relative) : new Uri(@"\Images\WK.png", UriKind.Relative);
                    break;
                case "Queen":
                    btmImage.UriSource = isBlack ? new Uri(@"\Images\BQ.png", UriKind.Relative) : new Uri(@"\Images\WQ.png", UriKind.Relative);
                    break;
                case "Rook":
                    btmImage.UriSource = isBlack ? new Uri(@"\Images\BR.png", UriKind.Relative) : new Uri(@"\Images\WR.png", UriKind.Relative);
                    break;
            }

            btmImage.EndInit();
            return btmImage;
        }

        /// <summary>
        /// Starts the game by giving the turn to Blacks. Change the visibility of Log Window
        /// </summary>
        /// <param name="sender">Button Control</param>
        /// <param name="e">Event Arguments</param>
        private void StartTheGameBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_figuresToCreate.Count != _Figures.Count)
            {
                TopLabel.Content = "Not all required figures are on the board.";
            }
            else 
            {
                _manager.Turn = CellColor.Black;


                //Holder.Visibility = Visibility.Hidden;
                LogGrid.Visibility = Visibility.Visible;

                TopLabel.Content = $"{_manager.Turn.ToString()}'s turn to play.";

                foreach (var image in _Figures)
                {
                    if ((image as Image).Name == "BK")
                    {
                        image.MouseDown += Figure_MouseDown;
                        image.MouseMove += Figure_MouseMove;
                        image.MouseUp -= Placement_MouseUp;
                        image.MouseUp += Figure_MouseUp;
                    }
                    else
                    {
                        (image as Image).MouseMove -= Figure_MouseMove;
                        (image as Image).MouseUp -= Placement_MouseUp;
                    }

                    _manager.CreateFigure((string)(image as Image).Tag, (image as Image).Name.Substring(1, 1), (image as Image).Name.Substring(0, 1));
                }
            }
          

        }

        /// <summary>
        /// Alanyzes the Chess Board situation and displays relevant message in Message Label. Stops the Game if it's a Mate.
        /// </summary>
        /// <param name="result">Integer representation of Game situation after Whites moved</param>
        private void AnalyzeMove(int result)
        {
            if (result == 1)
            {
                TopLabel.Content = $"{_manager.Turn.ToString()} King is under Check!";
            }
            else if (result != 0)
            {
                if (result == 2)
                {
                    string player = _manager.Turn == CellColor.Black ? "Whites" : "Blacks";
                    TopLabel.Content = $"Mate! {player} win.";
                }
                else
                {
                    TopLabel.Content = $"StalMate...";
                }

                Grid cover = new Grid
                {
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Top,
                    Width = 473,
                    Height = 473,
                    Margin = new Thickness(_manager.BoardOfDesktop["A8"].Margin.Left, _manager.BoardOfDesktop["A8"].Margin.Top, 0, 0),
                    Background = Brushes.RosyBrown,
                    Opacity = 0.5

                };

                MainGrid.Children.Add(cover);
            }
        }

        /// <summary>
        /// Recolors the Grids that are possible cells to move for the selected figure.
        /// </summary>
        /// <param name="cellName">String representation of cell name which the selected figure stands on.</param>
        private void AddHelperCells(string cellName)
        {
            var possibleMoves = _manager.GetPossibleMoves(cellName);

            foreach (var cell in possibleMoves)
            {
                _manager.BoardOfDesktop[cell].Background = _manager.BoardOfDesktop[cell].Background == Brushes.RosyBrown ? _selectedBlackColor : _selectedWhiteColor;
            }
        }

        /// <summary>
        /// Recolors the Grids back to their original color.
        /// </summary>
        /// <param name="cellName">String representation of cell name which the selected figure was standing on.</param>
        private void RemoveHelperCells(string cellName)
        {
            var possibleMoves = _manager.GetPossibleMoves(cellName);

            foreach (var cell in possibleMoves)
            {
                _manager.BoardOfDesktop[cell].Background = _manager.BoardOfDesktop[cell].Background == _selectedBlackColor ? Brushes.RosyBrown : Brushes.White;
            }
        }

        /// <summary>
        /// Gets the last moved figure and logs the move into the Logger TextBlock
        /// </summary>
        private void LogMove()
        {
            string turn = _manager.LastMoved.Color.ToString();
            string figure = _manager.LastMoved.GetType().Name;
            string movedTo = _manager.LastMoved.CurrentCell.ToString();

            Logger.Text += $"{turn}: {figure} {movedTo}" + Environment.NewLine;
        }

        /// <summary>
        /// On Mouse Down event on figure's image ass the helper cells for the cell on which the figure stands.
        /// </summary>
        /// <param name="sender">Image UIElement</param>
        /// <param name="e">Mouse Button Event Arguments</param>
        private void Figure_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Image image = sender as Image;
            AddHelperCells((string)image?.Tag);
        }

        /// <summary>
        /// On Mouse move Events hols the figure image near the cursor.
        /// </summary>
        /// <param name="sender">Image UIElement</param>
        /// <param name="e">Mouse Event Arguments</param>
        private void Figure_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var position = e.GetPosition(this);

                double left = position.X;
                double top = position.Y;

                Image image = sender as Image;

                image.Width = 80;
                image.Height = 80;
                image.Margin = new Thickness(left - image.Width / 2, top - image.Height / 2, 0, 0);
            }
        }

        /// <summary>
        /// On Mouse Up Event drops the figure's image into the Grid Cell by changing Image's margin accordingly.
        /// </summary>
        /// <param name="sender">Image UIElement</param>
        /// <param name="e">Mouse Button Event Arguments</param>
        private void Figure_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var position = e.GetPosition(this);

            double left = position.X;
            double top = position.Y;

            Image image = sender as Image;
            image.Width = 60;
            image.Height = 60;
            string cellFrom = (string)image.Tag;

            RemoveHelperCells(cellFrom);

            var possibleMoves = _manager.GetPossibleMoves(cellFrom);

            foreach (var grid in _manager.BoardOfDesktop.Values)
            {
                if (grid.Margin.Top <= top && grid.Margin.Left <= left &&
                    grid.Margin.Top + grid.Height >= top && grid.Margin.Left + grid.Width >= left)
                {
                    string cellTo = grid.Name;
                    if (possibleMoves.Contains(cellTo))
                    {
                        DoMoves(cellFrom, cellTo);
                        MoveImage(image, grid);

                        return;
                    }
                }
            }
            var startGrid = _manager.BoardOfDesktop[(string)image.Tag];
            MoveImage(image, startGrid);
        }


        private void Placement_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var position = e.GetPosition(this);

            double left = position.X;
            double top = position.Y;

            Image image = sender as Image;
            image.Width = 60;
            image.Height = 60;

            foreach (var grid in _manager.BoardOfDesktop.Values)
            {
                if (grid.Margin.Top <= top && grid.Margin.Left <= left &&
                    grid.Margin.Top + grid.Height >= top && grid.Margin.Left + grid.Width >= left)
                {
                    MoveImage(image, grid);
                    if (!_Figures.Contains(image)) 
                    {
                        _Figures.Add(image);
                    }

                    return;
                }
            }


            var margins = GetImageDefaultMargins(image.Name);
            image.Width = 80;
            image.Height = 80;
            image.Margin = new Thickness(margins.Item1, margins.Item2, 0, 0);
            image.Tag = null;
            _Figures.Remove(image);
        }


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
                    marginLeft = marginLeft + 1 * 70;
                    break;
                case 'R':
                    marginLeft = marginLeft + 2 * 70;
                    break;
                case 'B':
                    marginLeft = marginLeft + 3 * 70;
                    break;
                case 'N':
                    marginLeft = marginLeft + 4 * 70;
                    break;
                case 'P':
                    marginLeft = marginLeft + 5 * 70;
                    break;
                default:
                    break;
            }

            return (marginLeft, marginTop);
        }
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
                        return new BitmapImage(new Uri(@"\Images\BN.png", UriKind.Relative));
                    default:
                        return new BitmapImage(new Uri(@"\Images\BP.png", UriKind.Relative));
                }
            }
            else 
            {
                switch (figureName[1])
                {
                    case 'K':
                        return new BitmapImage(new Uri(@"\Images\WK.png", UriKind.Relative));
                    case 'Q':
                        return new BitmapImage(new Uri(@"\Images\WQ.png", UriKind.Relative));
                    case 'R':
                        return new BitmapImage(new Uri(@"\Images\WR.png", UriKind.Relative));
                    case 'B':
                        return new BitmapImage(new Uri(@"\Images\WB.png", UriKind.Relative));
                    case 'N':
                        return new BitmapImage(new Uri(@"\Images\WN.png", UriKind.Relative));
                    default:
                        return new BitmapImage(new Uri(@"\Images\WP.png", UriKind.Relative));
                }
            }
        }

        private void InitializeFigureHolder()
        {
            foreach (var figureName in _figuresToCreate)
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

                figureImage.MouseMove += Figure_MouseMove;
                figureImage.MouseUp += Placement_MouseUp;

                MainGrid.Children.Add(figureImage);
            }
           
        }

        private void MoveImage(Image imageToMove, Grid gridToMoveTo)
        {
            imageToMove.Margin = new Thickness(gridToMoveTo.Margin.Left, gridToMoveTo.Margin.Top, 0, 0);
            imageToMove.Tag = gridToMoveTo.Name;
            gridToMoveTo.Tag = imageToMove;
        }

        /// <summary>
        /// Does a cicle of moves: user move and computer brain move.
        /// </summary>
        /// <param name="cellFrom">Chess Board Cell from which user moves</param>
        /// <param name="cellTo">Chess Board Cell to which user moves</param>
        private void DoMoves(string cellFrom, string cellTo)
        {
            _manager.UserMove(cellFrom, cellTo);
            LogMove();

            int result = _manager.BrainMove();
            LogMove();

            AnalyzeMove(result);
        }

        /// <summary>
        /// Converts indexes from 0 to 8 into Chess Board Cell's names
        /// </summary>
        /// <param name="i">Index from 0 to 8</param>
        /// <param name="j">Index from 0 to 8</param>
        /// <returns>String representation of Chess Board Cell</returns>
        private string ConvertIndexesToCell(int i, int j)
        {
            string number = (8 - i).ToString();
            char letter = (char)(j + 65);

            return letter + number;
        }

    }
}
