using ChessEngineLogic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

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

        private Grid PawnLastCell { get; set; }

        private bool PlayingWithWhites { get; set; }

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
            if (MovingImage != null && e.LeftButton == MouseButtonState.Pressed)
            {
                var position = e.GetPosition(this);

                double left = position.X;
                double top = position.Y;

                MovingImage.Width = 80;
                MovingImage.Height = 80;
                MovingImage.Margin = new Thickness(left - MovingImage.Width / 2, top - MovingImage.Height / 2, 0, 0);
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
                        if (_manager.GameChoice == 1)
                        {
                            PlayingWithWhites = !PlayingWithWhites;
                            LetPlayerMoveImages(PlayingWithWhites);
                        }

                        if (_manager.IsPawnUpgrade(cellFrom, cellTo))
                        {
                            this.PawnLastCell = grid;
                            DisplayFigureChoice(grid.Margin.Left, grid.Margin.Top);
                            DisableBoard();
                        }

                        //if (_manager.IsPawnFork(cellFrom, cellTo)) 
                        //{
                        //    string imageToRemoveCell = $"{cellTo[0]}{cellFrom[1]}";
                        //    _manager.RemoveFigureImageFromBoard(imageToRemoveCell);
                        //}
                        _manager.Play(cellFrom, cellTo);
                        _manager.InsertImage(MovingImage, grid);
                        UpdateGameStatusBox(_manager.GameStatus, _manager.CurrentPlayer);
                        MovingImage = null;

                        //MessageBox.Text = _manager.MessageText;
                        return;
                    }
                }
            }
            var startGrid = _manager.GetGridByName(cellFrom);
            _manager.InsertImage(MovingImage, startGrid);
            MovingImage = null;
        }

        private void UpdateGameStatusBox(int gameStatus, bool player)
        {
            switch (gameStatus)
            {
                case 1:
                    GameStatusImage.Source = _manager.GetImageSource(player ? "RWK" : "RBK");
                    CurrentPlayersName.Content = $"{_manager.GetCurrentPlayerName()}'s turn";
                    break;
                case 2:
                    GameStatusImage.Source = _manager.GetImageSource(player ? "MBP" : "MWP");
                    CurrentPlayersName.Content = $"{_manager.GetWinnerName()} wins!";
                    DisableBoard();
                    break;
                case 3:
                    GameStatusImage.Source = _manager.GetImageSource("MSF");
                    CurrentPlayersName.Content = $"It's a stalemate";
                    DisableBoard();
                    break;
                default:
                    GameStatusImage.Source = _manager.GetImageSource(player ? "WP" : "BP");
                    CurrentPlayersName.Content = $"{_manager.GetCurrentPlayerName()}'s turn";
                    break;
            }

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
            int choice = _manager.GameChoice;

            switch (choice)
            {
                case 1:
                    foreach (var image in _manager.FigureImages)
                    {
                        image.MouseDown -= Placement_MouseDown;
                        image.MouseUp -= Placement_MouseUp;
                        _manager.CreateFigure((string)image.Tag, image.Name.Substring(1, 1), image.Name.Substring(0, 1));
                    }
                    PlayingWithWhites = true;
                    GameStatusImage.Source = _manager.GetImageSource("WP");
                    CurrentPlayersName.Content = $"{_manager.GetCurrentPlayerName()}'s turn";
                    LetPlayerMoveImages(PlayingWithWhites);
                    break;

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

                        _manager.SetTurn(false);
                        GameStatusImage.Source = _manager.GetImageSource("BP");

                    }

                    break;
                // Play Knight moves
                case 4:
                    if (_manager.AreAllFiguresOnBoard())
                    {
                        string cellFrom = (string)_manager.FigureImages[1].Tag;
                        string cellTo = (string)_manager.FigureImages[0].Tag;
                        _manager.Play(cellFrom, cellTo);
                    }

                    break;
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
            switch (_manager.GameChoice)
            {
                case 1:
                    DefaultGame_Click(sender, e);
                    break;
                case 2:
                    QueenEndgame_Click(sender, e);
                    break;
                case 3:
                    RookEndgame_Click(sender, e);
                    break;
                case 4:
                    KnightGame_Click(sender, e);
                    break;
            }
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

        private void DisableBoard()
        {
            foreach (var image in _manager.FigureImages)
            {
                image.MouseDown -= Figure_MouseDown;
                image.MouseMove -= Figure_MouseMove;
                image.MouseUp -= Figure_MouseUp;
            }
        }
        private void SetDefault_MouseEnter(object sender, MouseEventArgs e)
        {
            SetDefault.Source = new BitmapImage(new Uri(@"\Images\SetSelected.png", System.UriKind.Relative));
        }

        private void SetDefault_MouseLeave(object sender, MouseEventArgs e)
        {
            SetDefault.Source = new BitmapImage(new Uri(@"\Images\Set.png", System.UriKind.Relative));
        }

        private void Start_MouseEnter(object sender, MouseEventArgs e)
        {
            Start.Source = new BitmapImage(new Uri(@"\Images\StartSelected.png", System.UriKind.Relative));
        }

        private void Start_MouseLeave(object sender, MouseEventArgs e)
        {
            Start.Source = new BitmapImage(new Uri(@"\Images\Start.png", System.UriKind.Relative));
        }

        private void Reset_MouseEnter(object sender, MouseEventArgs e)
        {
            Reset.Source = new BitmapImage(new Uri(@"\Images\ResetSelected.png", System.UriKind.Relative));
        }

        private void Reset_MouseLeave(object sender, MouseEventArgs e)
        {
            Reset.Source = new BitmapImage(new Uri(@"\Images\Reset.png", System.UriKind.Relative));
        }

        private void Exit_MouseEnter(object sender, MouseEventArgs e)
        {
            Exit.Source = new BitmapImage(new Uri(@"\Images\ExitSelected.png", System.UriKind.Relative));
        }

        private void Exit_MouseLeave(object sender, MouseEventArgs e)
        {
            Exit.Source = new BitmapImage(new Uri(@"\Images\Exit.png", System.UriKind.Relative));
        }

        private void Exit_MouseDown(object sender, MouseButtonEventArgs e)
        {
            _manager.SaveGame();
            this.Close();
        }


        private void DefaultGame_Click(object sender, RoutedEventArgs e)
        {
            _manager.GameChoice = 1;
            _manager.FiguresForGame = new List<string> { "BK", "WK", "BQ", "WQ", "BR","WR", "BR", "WR", "BN","WN", "BN", "WN", "BB","WB", "BB", "WB",
                "BP", "WP", "BP","WP","BP","WP","BP","WP", "BP", "WP","BP","WP","BP","WP","BP","WP" };

            InitializeFigureHolder();
            _manager.SetGame();
        }

        private void QueenEndgame_Click(object sender, RoutedEventArgs e)
        {
            _manager.GameChoice = 2;
            _manager.FiguresForGame = new List<string> { "BK", "WK", "WQ", "WR" };

            InitializeFigureHolder();
        }

        private void RookEndgame_Click(object sender, RoutedEventArgs e)
        {
            _manager.GameChoice = 3;
            _manager.FiguresForGame = new List<string> { "BK", "WK", "WQ", "WR", "WR" };

            InitializeFigureHolder();
        }

        private void KnightGame_Click(object sender, RoutedEventArgs e)
        {
            _manager.GameChoice = 4;
            _manager.FiguresForGame = new List<string> { "BNP", "BN" };

            InitializeFigureHolder();
        }

        private void DisplayFigureChoice(double leftMargin, double marginTop)
        {
            FigureChoice.Visibility = Visibility.Visible;
            FigureChoice.Margin = new Thickness(leftMargin - 60, marginTop - 50, 0, 0);
        }

        private void Image_MouseEnter(object sender, MouseEventArgs e)
        {
            if (FigureChoice.Margin.Top < 300)
            {
                switch ((sender as Image).Name)
                {
                    case "Q":
                        (sender as Image).Source = _manager.GetImageSource("WQ");
                        break;
                    case "B":
                        (sender as Image).Source = _manager.GetImageSource("WB");
                        break;
                    case "R":
                        (sender as Image).Source = _manager.GetImageSource("WR");
                        break;
                    case "N":
                        (sender as Image).Source = _manager.GetImageSource("WN");
                        break;
                }
            }
            else
            {
                switch ((sender as Image).Name)
                {
                    case "Q":
                        (sender as Image).Source = _manager.GetImageSource("BQ");
                        break;
                    case "B":
                        (sender as Image).Source = _manager.GetImageSource("BB");
                        break;
                    case "R":
                        (sender as Image).Source = _manager.GetImageSource("BR");
                        break;
                    case "N":
                        (sender as Image).Source = _manager.GetImageSource("BN");
                        break;
                }

            }
        }

        private void Image_MouseLeave(object sender, MouseEventArgs e)
        {
            switch ((sender as Image).Name)
            {
                case "Q":
                    (sender as Image).Source = _manager.GetImageSource("YQ");
                    break;
                case "B":
                    (sender as Image).Source = _manager.GetImageSource("YB");
                    break;
                case "R":
                    (sender as Image).Source = _manager.GetImageSource("YR");
                    break;
                case "N":
                    (sender as Image).Source = _manager.GetImageSource("YN");
                    break;
            }
        }

        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            _manager.ChangeFigure(PawnLastCell, (sender as Image).Name);
            FigureChoice.Visibility = Visibility.Hidden;

            LetPlayerMoveImages(PlayingWithWhites);
        }

        private void Save_MouseLeave(object sender, MouseEventArgs e)
        {
            Save.Source = new BitmapImage(new Uri(@"\Images\Save.png", System.UriKind.Relative));
        }

        private void Save_MouseEnter(object sender, MouseEventArgs e)
        {
            Save.Source = new BitmapImage(new Uri(@"\Images\SaveSelected.png", System.UriKind.Relative));
        }

        private void SaveBtn_Click(object sender, MouseButtonEventArgs e)
        {
            _manager.SaveGame();
            MessageBox.Show("Game succefully saved.", "Saved", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void Dashboard_Click(object sender, RoutedEventArgs e)
        {
            MainGrid.Visibility = Visibility.Hidden;
            DashboardGrid.Visibility = Visibility.Visible;
            
            GamesListView.ItemsSource = _manager.LoadGameInfo();
        }

        private void GamePicker_Click(object sender, RoutedEventArgs e)
        {
            DashboardGrid.Visibility = Visibility.Hidden;
            MainGrid.Visibility = Visibility.Visible;
        }

        private void ContinueBtn_Click(object sender, RoutedEventArgs e)
        {
            if (GamesListView.SelectedItem != null)
            {
                var game = GamesListView.SelectedItem as GameViewModel;

                _manager.GameChoice = 1;
                _manager.FiguresForGame = new List<string> { "BK", "WK", "BQ", "WQ", "BR","WR", "BR", "WR", "BN","WN", "BN", "WN", "BB","WB", "BB", "WB",
                "BP", "WP", "BP","WP","BP","WP","BP","WP", "BP", "WP","BP","WP","BP","WP","BP","WP" };

                InitializeFigureHolder();
                foreach (var image in _manager.FigureImages)
                {
                    image.MouseDown -= Placement_MouseDown;
                    image.MouseUp -= Placement_MouseUp;
                    
                }
                _manager.ContinueGame(game);
                PlayingWithWhites = game.Turn == game.White.Name;
                LetPlayerMoveImages(PlayingWithWhites);
                DashboardGrid.Visibility = Visibility.Hidden;
                MainGrid.Visibility = Visibility.Visible;
            }
        }
    }
}

