using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace KingdomChessGame_Desktop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Game UI manager that helps during the game.
        /// </summary>
        private readonly GameUIManager _manager;

        private Image MovingImage { get; set; }

        private bool PlayingWithWhites { get; set; }

        private Grid UppderGrid { get; set; }

        public MainWindow()
        {
            _manager = new GameUIManager();

            PlayingWithWhites = true;

            InitializeComponent();
            InitializeBoard();
        }

        /// <summary>
        /// Draw figures on their initial positions on main window.
        /// </summary>
        private void InitializeFigureHolder()
        {
            foreach (var image in _manager.FigureImages)
            {
                MainGrid.Children.Remove(image);
            }

            var figureImages = _manager.CreateFigureImages();
            foreach (var figureImage in figureImages)
            {
                figureImage.MouseDown += Placement_MouseDown;
                figureImage.MouseMove += Figure_MouseMove;
                figureImage.MouseUp += Placement_MouseUp;
                if (!MainGrid.Children.Contains(figureImage))
                {
                    MainGrid.Children.Add(figureImage);
                }
            }
        }

        /// <summary>
        /// Draws the Chess board with grids.
        /// </summary>
        private void InitializeBoard()
        {
            var boardGrid = _manager.CreateBoard();

            foreach (var grid in boardGrid)
            {
                MainGrid.Children.Add(grid);
            }

        }

        /// <summary>
        /// Combobox for user to choose the game to play.
        /// </summary>
        /// <param name="sender">Combobox object</param>
        /// <param name="e">SelectionChangedEventArgs argument</param>
        private void GameChoiceCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int choice = (sender as ComboBox).SelectedIndex;
            switch (choice)
            {
                case 0:
                    return;
                case 1:
                    _manager.FiguresForGame = new List<string> { "BK", "WK", "BQ", "WQ", "BR","WR", "BR", "WR", "BN","WN", "BN", "WN", "BB","WB", "BB", "WB",
                "BP", "WP", "BP","WP","BP","WP","BP","WP", "BP", "WP","BP","WP","BP","WP","BP","WP" };
                    break;
                case 2:
                    _manager.FiguresForGame = new List<string> { "BK", "WK", "WQ", "WR" };
                    break;
                case 3:
                    _manager.FiguresForGame = new List<string> { "BK", "WK", "WQ", "WR", "WR" };
                    break;
                case 4:
                    _manager.FiguresForGame = new List<string> { "BNP", "BN" };
                    break;
            }

            InitializeFigureHolder();
            _manager.GameChoice = choice;
        }

        /// <summary>
        /// On mouse up places the image on the center of relevant grid.
        /// </summary>
        /// <param name="sender">Iamge object</param>
        /// <param name="e">MouseButtonEventArgs argument</param>
        private void Placement_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var position = e.GetPosition(this);

            double left = position.X;
            double top = position.Y;

            Image image = sender as Image;
            MovingImage.Width = 60;
            MovingImage.Height = 60;
           

            foreach (var grid in _manager.GridBoard)
            {
                if (grid.Margin.Top <= top && grid.Margin.Left <= left &&
                    grid.Margin.Top + grid.Height >= top && grid.Margin.Left + grid.Width >= left)
                {
                    _manager.InsertImage(MovingImage, grid);
                    return;
                }
            }

            _manager.ResetImage(MovingImage);
            MovingImage = null;
        }

        /// <summary>
        /// Moves the image by pressing mouse left button.
        /// </summary>
        /// <param name="sender">Image object</param>
        /// <param name="e">MouseEventArgs argument</param>
        private void Figure_MouseMove(object sender, MouseEventArgs e)
        {
            if (MovingImage != null) 
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    var position = e.GetPosition(this);

                    double left = position.X;
                    double top = position.Y;

                    //Image image = sender as Image;
                    MovingImage.Width = 80;
                    MovingImage.Height = 80;
                    MovingImage.Margin = new Thickness(left - MovingImage.Width / 2, top - MovingImage.Height / 2, 0, 0);
                }
            }
        }

        /// <summary>
        /// Places the image on the center of the relevant grid and plays the game.
        /// </summary>
        /// <param name="sender">Image object</param>
        /// <param name="e">MouseButtonEventArgs argument</param>
        private void Figure_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (MovingImage == null) 
            {
                return;
            }

            var position = e.GetPosition(this);

            double left = position.X;
            double top = position.Y;

            int iZIndex = Canvas.GetZIndex(MovingImage);
            Canvas.SetZIndex(MovingImage, iZIndex - 1);

            MovingImage.Width = 60;
            MovingImage.Height = 60;
            
            string cellFrom = (string)MovingImage.Tag;

            _manager.RemoveHelpers(cellFrom);

            foreach (var grid in _manager.GridBoard)
            {
                if (grid.Margin.Top <= top && grid.Margin.Left <= left &&
                    grid.Margin.Top + grid.Height >= top && grid.Margin.Left + grid.Width >= left)
                {
                    string cellTo = grid.Name;
                    if (_manager.IsValidCellToMove(cellFrom, cellTo))
                    {
                        _manager.Play(cellFrom, cellTo);
                        _manager.InsertImage(MovingImage, grid);

                        PlayingWithWhites = !PlayingWithWhites;
                        LetPlayerMoveImages(PlayingWithWhites);
                       
                        MovingImage = null;

                        MessageBox.Text = _manager.MessageText;
                        return;
                    }
                }
            }
            var startGrid = _manager.GetGridByName(cellFrom);
            _manager.InsertImage(MovingImage, startGrid);
            MovingImage = null;
        }

        /// <summary>
        /// Add the helper cells for the image figure.
        /// </summary>
        /// <param name="sender">Image object.</param>
        /// <param name="e">MouseButtonEventArgs argument</param>
        private void Figure_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if ((sender as Image)?.Tag == null) 
            {
                return;
            }
           
            MovingImage = sender as Image;
            int iZIndex = Canvas.GetZIndex(MovingImage);
            Canvas.SetZIndex(MovingImage, iZIndex + 1);

            _manager.AddHelpers((string)MovingImage.Tag);
        }

        /// <summary>
        /// Add the helper cells for the image figure.
        /// </summary>
        /// <param name="sender">Image object.</param>
        /// <param name="e">MouseButtonEventArgs argument</param>
        private void Placement_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MovingImage = sender as Image;
            int iZIndex = Canvas.GetZIndex(MovingImage);
            Canvas.SetZIndex(MovingImage, iZIndex + 1);
        }

        /// <summary>
        /// Start the game according to the user selection.
        /// </summary>
        /// <param name="sender">Button object</param>
        /// <param name="e">RoutedEventArgs argument</param>
        private void StartBtn_Click(object sender, RoutedEventArgs e)
        {
            int choice = GameChoiceCombo.SelectedIndex;
            if (_manager.AreAllFiguresOnBoard())
            {
                switch (choice)
                {
                    // Play winning algorithms
                    case 1:
                        foreach (var image in _manager.FigureImages)
                        {
                            image.MouseUp -= Placement_MouseUp;
                            _manager.CreateFigure((string)image.Tag, image.Name.Substring(1, 1), image.Name.Substring(0, 1));
                        }

                        LetPlayerMoveImages(PlayingWithWhites);
                        break;

                    case 2:
                    case 3:
                        foreach (var image in _manager.FigureImages)
                        {
                            if (image.Name[0] == 'B')
                            {
                                image.MouseDown += Figure_MouseDown;
                                image.MouseMove += Figure_MouseMove;
                                image.MouseUp += Figure_MouseUp;
                            }
                            else
                            {
                                image.MouseMove -= Figure_MouseMove;
                            }

                            image.MouseUp -= Placement_MouseUp;

                            _manager.CreateFigure((string)image.Tag, image.Name.Substring(1, 1), image.Name.Substring(0, 1));
                        }

                        break;
                    // Play Knight moves
                    case 4:
                        string cellFrom = (string)_manager.FigureImages[1].Tag;
                        string cellTo = (string)_manager.FigureImages[0].Tag;
                        _manager.Play(cellFrom, cellTo);
                        break;
                }

                MessageBox.Text = _manager.MessageText;
            }
        }

        /// <summary>
        /// Puts the figure images in figure holder into their chess game default positions.
        /// </summary>
        /// <param name="sender">Button object</param>
        /// <param name="e">RoutedEventArgs arguments</param>
        private void DefaultSetBtn_Click(object sender, RoutedEventArgs e)
        {
            _manager.PutFigureOnDefaultPosition();
        }

        /// <summary>
        /// Resets the game and clears the board.
        /// </summary>
        /// <param name="sender">Button object</param>
        /// <param name="e">RoutedEventArgs arguments</param>
        private void ResetBtn_Click(object sender, RoutedEventArgs e)
        {
            _manager.ResetGame();
        }

        private void LetPlayerMoveImages(bool whitesTurn) 
        {
            if (whitesTurn)
            {
                foreach (var image in _manager.FigureImages)
                {
                    if (image.Name[0] == 'W')
                    {
                        image.MouseDown += Figure_MouseDown;
                        image.MouseMove += Figure_MouseMove;
                        image.MouseUp += Figure_MouseUp;
                    }
                    else
                    {
                        image.MouseDown -= Figure_MouseDown;
                        image.MouseMove -= Figure_MouseMove;
                        image.MouseUp -= Figure_MouseUp;
                    }
                }
            }
            else 
            {
                foreach (var image in _manager.FigureImages)
                {
                    if (image.Name[0] == 'B')
                    {
                        image.MouseDown += Figure_MouseDown;
                        image.MouseMove += Figure_MouseMove;
                        image.MouseUp += Figure_MouseUp;
                    }
                    else
                    {
                        image.MouseDown -= Figure_MouseDown;
                        image.MouseMove -= Figure_MouseMove;
                        image.MouseUp -= Figure_MouseUp;
                    }
                }
            }
        }
    }
}

