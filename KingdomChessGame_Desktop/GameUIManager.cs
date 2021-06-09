using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ChessEngineLogic;
using System.Threading.Tasks;
using System.Linq;
using System.IO;
using System.Text.Json;
using AutoMapper;

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

        private readonly Mapper _mapper;

        private List<string> _possibleMovesForSelectedFigure;

        /// <summary>
        /// Predefined colors for board cells on UI
        /// </summary>
        private readonly Brush _selectedBlackColor = (Brush)(new BrushConverter().ConvertFrom("#96944B"));
        private readonly Brush _selectedWhiteColor = (Brush)(new BrushConverter().ConvertFrom("#C3B776"));
        private readonly Brush _blackColor = (Brush)(new BrushConverter().ConvertFrom("#43655A"));
        private readonly Brush _whiteColor = (Brush)(new BrushConverter().ConvertFrom("#A6B2BA"));

        public bool CurrentPlayer { get; set; }
        public int GameStatus { get; set; }

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
            FiguresForGame = new List<string>();
            FigureImages = new List<Image>();
            GridBoard = new List<Grid>();
            _mapper = AutoMapperProfile.InitializeMapper();
            SetGameEngine();
        }

        /// <summary>
        /// Initializes the objecs required for the game.
        /// </summary>
        private void SetGameEngine()
        {
            // To be changed into a dynamic choice
            _engine = new ChessEngine();
            _engine.GameEvent += FigureMove;
        }

        /// <summary>
        /// Resets the objecs required for the game.
        /// </summary>
        public void ResetGame()
        {
            SetGameEngine();
        }

        /// <summary>
        /// For the given Figure checks if the grid to move on is a valid cell in terms of chess game.
        /// </summary>
        /// <param name="cellFrom">Grid Name that the moving figure stansd on.</param>
        /// <param name="cellTo">Grid Name that the figure wants to move on.</param>
        /// <returns></returns>
        public bool IsValidCellToMove(string cellFrom, string cellTo)
        {
            return _possibleMovesForSelectedFigure.Contains(cellTo);
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

        public void RemoveFigureImageFromBoard(string position) 
        {
            var imageToRemove = GetImageByPosition(position);
            if (imageToRemove != null)
            {
                var margins = GetImageDefaultMargins(imageToRemove.Name);
                imageToRemove.Margin = new Thickness(margins.Item1, margins.Item2, 0, 0);
                imageToRemove.Tag = null;
            }
        }

        /// <summary>
        /// Get the Image object from FigureImages collection based on the given name.
        /// </summary>
        /// <param name="figureName">Name of the figure which image should be found.</param>
        /// <returns>Image object of the relevant figure.</returns>
        private Image GetImageByFigureName(string figureName, string cellString)
        {
            foreach (var image in FigureImages)
            {
                if (image.Name == figureName && (string)image.Tag == cellString)
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
            // beaten Figure if any
            if (!String.IsNullOrEmpty(e.BeatenFigureName)) 
            {
                var beatenFigureImage = GetImageByFigureName(e.BeatenFigureName, e.BeatenFigureCell);
                if (beatenFigureImage != null)
                {
                    var margins = GetImageDefaultMargins(beatenFigureImage.Name);
                    beatenFigureImage.Margin = new Thickness(margins.Item1, margins.Item2, 0, 0);
                    beatenFigureImage.Width = 80;
                    beatenFigureImage.Height = 80;
                    beatenFigureImage.Tag = null;
                }
            }

            // Moved image
            if (!String.IsNullOrEmpty(e.MovedFigureName))
            {
                double top = GetGridByName(e.CellTo).Margin.Top;
                double left = GetGridByName(e.CellTo).Margin.Left;

                Image image = GetImageByFigureName(e.MovedFigureName, e.CellFrom);

                image.Margin = new Thickness(left, top, 0, 0);
                image.Tag = e.CellTo;

            }

            // Casteling if any
            if (!String.IsNullOrEmpty(e.CastelingCellFrom)) 
            {
                var castelingRookImage = GetImageByPosition(e.CastelingCellFrom);
                var castelingRookGrid = GetGridByName(e.CastelingCellTo);
                InsertImage(castelingRookImage, castelingRookGrid);
            }

            GameStatus = e.GameStatus;
            CurrentPlayer = e.CurrentPlayer;
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
            return status switch
            {
                1 => $"{currentPlayer} King is under check.",
                2 => $"Mate! {winner} win.",
                3 => "Stalemate...",
                _ => $"Good luck! {currentPlayer}'s turn to play.",
            };
        }

        /// <summary>
        /// Changes the bakcground colors of grids that represent the possible-to-move cells.
        /// </summary>
        /// <param name="cellName">Current grid name that the figure stands on.</param>
        public void AddHelpers(string cellName)
        {
            if (string.IsNullOrEmpty(cellName))
            {
                return;
            }

            _possibleMovesForSelectedFigure = _engine.GetPossibleMovesFromCurrentPosition(cellName);

            foreach (var cell in _possibleMovesForSelectedFigure)
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
            //var possibleMoves = _engine.GetPossibleMoves(cellName);

            foreach (var cell in _possibleMovesForSelectedFigure)
            {
                Grid grid = GetGridByName(cell);
                grid.Background = grid.Background == _selectedBlackColor ? _blackColor : _whiteColor;
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
                    _engine.Play(cellFrom, cellTo);
                    _engine.PlayWinningWithQueenAndRookAlgorithm();
                    break;
                case 1:
                    _engine.Play(cellFrom, cellTo);
                    break;
            }
        }

        public bool IsPawnUpgrade(string cellFrom, string cellTo) 
        {
            var figureImage = GetImageByPosition(cellFrom);
            return (figureImage.Name == "BP" && cellTo[1] == '1')
                || (figureImage.Name == "WP" && cellTo[1] == '8');
        }

        public void SetTurn(bool whites)
        {
            _engine.SetTurn(whites);
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
            int marginTop = 200;

            for (int i = 0; i < 8; i++)
            {
                int marginLeft = 260;
                brushes = brushes == _blackColor ? _whiteColor : _blackColor;
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
                        Opacity = 0.85
                    };

                    GridBoard.Add(grid);

                    marginLeft += 59;
                    brushes = brushes == _blackColor ? _whiteColor : _blackColor;
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
                marginTop = 90;

            }
            else
            {
                marginTop = 730;

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
        public BitmapImage GetImageSource(string figureName)
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
            else if (figureName[0] == 'W')
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
            else if (figureName[0] == 'R') 
            {
                return (figureName[1]) switch
                {
                    'B' => new BitmapImage(new Uri(@"\Images\RBK.png", UriKind.Relative)),
                    'W' => new BitmapImage(new Uri(@"\Images\RWK.png", UriKind.Relative)),
                    _ => new BitmapImage(new Uri(@"\Images\WP.png", UriKind.Relative)),
                };

            }
            else if (figureName[0] == 'M')
            {
                return (figureName[1]) switch
                {
                    'B' => new BitmapImage(new Uri(@"\Images\MBP.png", UriKind.Relative)),
                    'W' => new BitmapImage(new Uri(@"\Images\MWP.png", UriKind.Relative)),
                    _ => new BitmapImage(new Uri(@"\Images\MSF.png", UriKind.Relative)),
                };

            }
            else
            {
                return (figureName[1]) switch
                {
                    'Q' => new BitmapImage(new Uri(@"\Images\YQ.png", UriKind.Relative)),
                    'R' => new BitmapImage(new Uri(@"\Images\YR.png", UriKind.Relative)),
                    'B' => new BitmapImage(new Uri(@"\Images\YB.png", UriKind.Relative)),
                    'N' => new BitmapImage(new Uri(@"\Images\YN.png", UriKind.Relative)),
                };
            }
        }

        public List<GameViewModel> LoadGameInfo() 
        {
            var games =  _engine.LoadGameInfo();
            var gamesViewModels = new List<GameViewModel>();
            foreach (var game in games)
            {
                gamesViewModels.Add(_mapper.Map<GameViewModel>(game));
            }

            return gamesViewModels;
        }

        public List<PlayerViewModel> GetPlayers() 
        {
            return _mapper.Map<List<PlayerViewModel>>(_engine.GetPlayers());
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
            this.MessageText = GetMessageTextForDefaultGame(0, "White", string.Empty);
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
                    Task.Delay(1000).Wait();
                });

                image.Margin = new Thickness(grid.Margin.Left, grid.Margin.Top, 0, 0);
                image.Tag = grid.Name;
            }
        }

        public Dictionary<string, string> GetCurrentBoard() 
        {
            return _engine.GetBoard();
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
                    grid = figureImage.Name[1] switch
                    {
                        'K' => GetEmtpyGridByName("E1"),
                        'Q' => GetEmtpyGridByName("D1"),
                        'B' => GetEmtpyGridByName("C1") ?? GetEmtpyGridByName("F1"),
                        'N' => GetEmtpyGridByName("B1") ?? GetEmtpyGridByName("G1"),
                        'R' => GetEmtpyGridByName("A1") ?? GetEmtpyGridByName("H1"),
                        _ => GetEmtpyGridByName("A2") ?? GetEmtpyGridByName("B2") ?? GetEmtpyGridByName("C2") ?? GetEmtpyGridByName("D2") ??
GetEmtpyGridByName("E2") ?? GetEmtpyGridByName("F2") ?? GetEmtpyGridByName("G2") ?? GetEmtpyGridByName("H2"),
                    };
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
            return GridBoard.FirstOrDefault(grid => grid.Name == name && IsEmptyGrid(grid));
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

        /// <summary>
        /// Gets the image object based on the chess board cell its figure is standing one.
        /// </summary>
        /// <param name="gridName">Grid Name as a chess board cell name</param>
        /// <returns>Image object of the figure standing on the given chess cell position.</returns>
        private Image GetImageByPosition(string gridName)
        {
            return this.FigureImages.FirstOrDefault(image => (string)image.Tag == gridName);
        }

        
        public void ChangeFigure(Grid cellToChange, string chosenFigure) 
        {
            var figureImage = GetImageByPosition(cellToChange.Name);
            string color = cellToChange.Name[1] == '8' ? "W" : "B";
            if (color == "W") 
            {
                switch (chosenFigure)
                {
                    case "Q":
                        figureImage.Source = GetImageSource("WQ");
                        figureImage.Name = "WQ";
                    break;
                    case "B":
                        figureImage.Source = GetImageSource("WB");
                        figureImage.Name = "WB";

                        break;
                    case "R":
                        figureImage.Source = GetImageSource("WR");
                        figureImage.Name = "WR";

                        break;
                    case "N":
                        figureImage.Source = GetImageSource("WN");
                        figureImage.Name = "WN";

                        break;
                }
            }
            else 
            {
                switch (chosenFigure)
                {
                    case "Q":
                        figureImage.Source = GetImageSource("BQ");
                        figureImage.Name = "BQ";

                        break;
                    case "B":
                        figureImage.Source = GetImageSource("BB");
                        figureImage.Name = "BB";

                        break;
                    case "R":
                        figureImage.Source = GetImageSource("BR");
                        figureImage.Name = "BR";

                        break;
                    case "N":
                        figureImage.Source = GetImageSource("BN");
                        figureImage.Name = "BN";

                        break;
                }
            }

            CreateFigure(cellToChange.Name, chosenFigure, color);
        }

        public bool IsPawnFork(string cellFrom, string cellTo) 
        {
            var figureImage = GetImageByPosition(cellFrom);
            return figureImage.Name[1] == 'P' && GetImageByPosition(cellTo) == null && cellFrom[0] != cellTo[0];
        }

        public void SaveGame() 
        {
            _engine.SaveGame();
        }

        public string GetCurrentPlayerName() 
        {
            if (CurrentPlayer)
            {
                return _engine.CurrentGame.White.Name;
            }
            else 
            {
                return _engine.CurrentGame.Black.Name;
            }
        }


        public string GetWinnerName()
        {
            switch (_engine.CurrentGame.Winner) 
            {
                case null:
                    return string.Empty;
                case true:
                    return _engine.CurrentGame.White.Name;
                case false:
                default:
                    return _engine.CurrentGame.Black.Name;
            }
        }

        public void SetGame()
        {
            var playerSetter = new Players();
            playerSetter.ShowDialog();
            if (playerSetter.DialogResult.Value)
            {
                _engine.SetGame();
                _engine.CurrentGame.White = _mapper.Map<PlayerModel>( playerSetter.White);
                _engine.CurrentGame.Black = _mapper.Map<PlayerModel>(playerSetter.Black);
                CurrentPlayer = true;
            }
        }

        public void ContinueGame(GameViewModel game) 
        {
            // Set current game board
            ResetGame();
            _engine.CurrentGame = ConvertViewModelToModel(game);
            foreach (var item in game.Board)
            {
               
                var image = GetImageByFigureName(item.Value, null);
                image.Width = 60;
                image.Height = 60;
                var grid = GetGridByName(item.Key);
                InsertImage(image, grid);
                CreateFigure((string)image.Tag, image.Name.Substring(1, 1), image.Name.Substring(0, 1));
            }
            // give turn to play
            SetTurn(game.Turn == game.White.Name);
        }

        public GameModel ConvertViewModelToModel(GameViewModel gameViewModel) 
        {
            var game = new GameModel();
            game.StartDate = Convert.ToDateTime(gameViewModel.StartDate);
            game.Board = gameViewModel.Board;
            game.White.Name = gameViewModel.White.Name;
            game.Black.Name = gameViewModel.Black.Name;
            game.Turn = gameViewModel.Turn == gameViewModel.White.Name;
            game.MovesLog = gameViewModel.MovesLog;

            return game;
        }
    }
}
