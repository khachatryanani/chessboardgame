using AutoMapper;
using ChessGame;
using DataAccess;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

//GameLogic
namespace ChessEngineLogic
{
    public partial class ChessEngine
    {
        private readonly ChessBoardManager _boardManager;

        public event EventHandler<GameEventArgs> GameEvent;

        private readonly ChessGameData _data;
        private readonly Mapper _mapper;

        public GameModel CurrentGame { get; set; }

        public ChessEngine()
        {
            _boardManager = new ChessBoardManager();
            _boardManager.FigureMoved += OnFigureMoved;
            _data = new ChessGameData();
            _mapper = AutoMapperProfile.InitializeMapper();

        }

        public void SetTurn(bool turn)
        {
            _boardManager.Turn = turn;
        }

        public void CreateFigure(string cellString, string typeString, string colorString)
        {
            _boardManager.AddFigure(cellString, typeString, colorString);
        }

        public void Play(string cellFrom, string cellTo)
        {
            _boardManager.MoveFigure(cellFrom, cellTo);
        }

        private void OnFigureMoved(object sender, FigureMoveEventArgs e) 
        {
            CurrentGame.MovesLog.Add(new Tuple<string, string, string>(e.MovedFigureName,e.CellFrom, e.CellTo));
            CurrentGame.Turn = e.CurrentPlayer;
            CurrentGame.Winner = e.WinnerPlayer;
            CurrentGame.Status = e.GameStatus;
            GameEvent?.Invoke(this, new GameEventArgs(e));
        }
   
        public List<string> GetPossibleMovesFromCurrentPosition(string cellString) 
        {
            return _boardManager.GetPossibleMoves(cellString);
        }

        public Dictionary<string, string> GetBoard() 
        {
            return _boardManager.GetBoard();
        }

        public List<GameModel> LoadGameInfo()
        {
            var games = _mapper.Map<List<GameModel>>( _data.GetGames());
            return games;
            //string info = File.ReadAllText("gameresult.txt");
            //if (!string.IsNullOrEmpty(info))
            //{
            //    return JsonSerializer.Deserialize(info, typeof(List<GameModel>)) as List<GameModel>;
            //}
            //else
            //{
            //    return new List<GameModel>();
            //}
        }

        public void SaveGame()
        {
            if (CurrentGame != null)
            {
                double gameResult = 0;
                switch (CurrentGame.Status)
                {
                    case 2:
                        gameResult = 1;
                        break;
                    case 3:
                        gameResult = 0.5;
                        break;
                }

                CurrentGame.Result = gameResult;
                CurrentGame.Board = GetBoard();
                
                var games = LoadGameInfo();
                if (!games.ContainsGame(CurrentGame))
                {
                   CurrentGame.GameId = _data.CreateGame(_mapper.Map<Game>(CurrentGame));
                }
                else 
                {
                    _data.UpdateGame(_mapper.Map<Game>(CurrentGame));
                }

                //File.WriteAllText("gameresult.txt", JsonSerializer.Serialize(games));
            }
        }

        public void SetGame()
        {
            CurrentGame = new GameModel { StartDate = DateTime.Now };
            CurrentGame.MovesLog = new List<Tuple<string, string, string>>();
        }

        public  List<PlayerModel> GetPlayers() 
        {
            return _mapper.Map<List<PlayerModel>>(_data.GetPlayers());
        }
    }
}
