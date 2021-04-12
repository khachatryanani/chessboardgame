using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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

        public MainWindow()
        {
            _manager = new GameUIManager();

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
                    break;
                case 1:
                    _manager.FiguresForGame = new List<string> { "BK", "WK", "BQ", "WQ", "BR","WR", "BR", "WR", "BN","WN", "BN", "WN", "BB","WB", "BB", "WB",
                "BP", "WP", "BP","WP","BP","WP","BP","WP", "BP", "WP","BP","WP","BP","WP","BP","WP" };

                    InitializeFigureHolder();
                    break;
                case 2:

                    _manager.FiguresForGame = new List<string> { "BK", "WK", "WQ", "WR" };
                    InitializeFigureHolder();
                    break;
                case 3:
                    _manager.FiguresForGame = new List<string> { "BK", "WK", "WQ", "WR", "WR" };
                    break;
                case 4:
                    _manager.FiguresForGame = new List<string> { "BNP", "BN" };
                    InitializeFigureHolder();
                    break;
            }
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
            image.Width = 60;
            image.Height = 60;

            foreach (var grid in _manager.GridBoard)
            {
                if (grid.Margin.Top <= top && grid.Margin.Left <= left &&
                    grid.Margin.Top + grid.Height >= top && grid.Margin.Left + grid.Width >= left)
                {
                    _manager.InsertImage(image, grid);
                    return;
                }
            }

            _manager.ResetImage(image);
        }

        /// <summary>
        /// Moves the image by pressing mouse left button.
        /// </summary>
        /// <param name="sender">Image object</param>
        /// <param name="e">MouseEventArgs argument</param>
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
        /// Places the image on the center of the relevant grid and plays the game.
        /// </summary>
        /// <param name="sender">Image object</param>
        /// <param name="e">MouseButtonEventArgs argument</param>
        private void Figure_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var position = e.GetPosition(this);

            double left = position.X;
            double top = position.Y;

            Image image = sender as Image;
            image.Width = 60;
            image.Height = 60;

            string cellFrom = (string)image.Tag;

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
                        _manager.InsertImage(image, grid);

                        return;
                    }
                }
            }
            var startGrid = _manager.GetGridByName(cellFrom);
            _manager.InsertImage(image, startGrid);
        }

        /// <summary>
        /// Add the helper cells for the image figure.
        /// </summary>
        /// <param name="sender">Image object.</param>
        /// <param name="e">MouseButtonEventArgs argument</param>
        private void Figure_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Image image = sender as Image;
            _manager.AddHelpers((string)image?.Tag);
        }

        /// <summary>
        /// Start the game according to the user selection.
        /// </summary>
        /// <param name="sender">Button object</param>
        /// <param name="e">RoutedEventArgs argument</param>
        private void StartBtn_Click(object sender, RoutedEventArgs e)
        {
            int choice = GameChoiceCombo.SelectedIndex;
            switch (choice)
            {
                case 2:
                case 3:
                    if (_manager.AreAllFiguresOnBoard())
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
                                image.MouseMove -= Figure_MouseMove;
                            }

                            image.MouseUp -= Placement_MouseUp;

                            _manager.CreateFigure((string)image.Tag, image.Name.Substring(1, 1), image.Name.Substring(0, 1));
                        }
                    }
                    break;
                default:
                    _manager.Play();
                    break;
            }
        }
    }
}

