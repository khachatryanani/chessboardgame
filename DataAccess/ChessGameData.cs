using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace DataAccess
{
    public class ChessGameData
    {
        private readonly string _connectionString;

        public ChessGameData(string connectionString)
        {
            _connectionString = connectionString;
        }

        public ChessGameData()
        {
            //_connectionString = ConfigurationManager.ConnectionStrings["SQLServer"].ConnectionString;
            _connectionString = @"Data Source = DESKTOP-TK7OBVA\SQLEXPRESS; Initial Catalog = ChessGame; Integrated Security = True";
        }

        // Games
        public List<Game> GetGames()
        {
            using (SqlConnection connection = new SqlConnection(this._connectionString))
            {
                connection.Open();
                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "[dbo].[GetGames]";

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        var games = new List<Game>();
                        if (reader.HasRows)
                        {
                            int ordGameId = reader.GetOrdinal("GameId");
                            int ordStartDate = reader.GetOrdinal("StartDate");
                            int ordStatus = reader.GetOrdinal("Status");
                            int ordResult = reader.GetOrdinal("Result");
                            int ordTurn = reader.GetOrdinal("Turn");
                            int ordWinner = reader.GetOrdinal("Winner");
                            int ordWhitesName = reader.GetOrdinal("WhitesName");
                            int ordWhitesId = reader.GetOrdinal("WhitesId");
                            int ordBlacksName = reader.GetOrdinal("BlacksName");
                            int ordBlacksId = reader.GetOrdinal("BlacksId");
                            int ordBoard = reader.GetOrdinal("Board");
                            int ordMovesLog = reader.GetOrdinal("MovesLog");

                            while (reader.Read())
                            {
                                var white = new Player
                                {
                                    PlayerId = reader.GetInt32(ordWhitesId),
                                    Name = reader.GetString(ordWhitesName),
                                };

                                var black = new Player
                                {
                                    PlayerId = reader.GetInt32(ordBlacksId),
                                    Name = reader.GetString(ordBlacksName),
                                };
                                games.Add(
                                    new Game
                                    {
                                        GameId = reader.GetInt32(ordGameId),
                                        StartDate = reader.GetDateTime(ordStartDate),
                                        Status = reader.GetInt32(ordStatus),
                                        Result = Convert.ToDouble( reader.GetDecimal(ordResult)),
                                        Turn = reader.GetBoolean(ordTurn),
                                        Winner = reader.GetValue(ordWinner)as bool?,
                                        White = white,
                                        Black= black,
                                        Board = ConvertStringToBoardDescription(reader.GetString(ordBoard)),
                                        MovesLog = ConvertStringToMovesLog(reader.GetString(ordMovesLog))
                                    });
                            }
                        }
                        return games;
                    }
                }
            }
        }

        public int CreateGame(Game game)
        {
            using (SqlConnection connection = new SqlConnection(this._connectionString))
            {
                connection.Open();
                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "[dbo].[CreateGame]";

                    command.Parameters.Add("@startDate", SqlDbType.DateTime2).Value = game.StartDate;
                    command.Parameters.Add("@status", SqlDbType.Int).Value = game.Status;
                    command.Parameters.Add("@result", SqlDbType.Decimal).Value = Convert.ToDecimal(game.Result);
                    command.Parameters.Add("@turn", SqlDbType.Bit).Value = game.Turn;
                    command.Parameters.Add("@winner", SqlDbType.Bit).Value = game.Winner;
                    command.Parameters.Add("@whitesId", SqlDbType.Int).Value = game.White.PlayerId;
                    command.Parameters.Add("@blacksId", SqlDbType.Int).Value = game.Black.PlayerId;
                    command.Parameters.Add("@board", SqlDbType.VarChar).Value = ConvertBoardDescriptionToString(game.Board);
                    command.Parameters.Add("@movesLog", SqlDbType.VarChar).Value = ConvertMovesLogToSring(game.MovesLog);
                    command.Parameters.Add("@gameId",SqlDbType.Int).Direction = ParameterDirection.Output;
                    command.ExecuteNonQuery();

                    int gameId =(int)command.Parameters["@gameId"].Value;
                    return gameId;
                }
            }
        }

        public void UpdateGame(Game game)
        {
            using (SqlConnection connection = new SqlConnection(this._connectionString))
            {
                connection.Open();
                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "[dbo].[UpdateGame]";

                    command.Parameters.Add("@gameId", SqlDbType.Int).Value = game.GameId;
                    command.Parameters.Add("@status", SqlDbType.Int).Value = game.Status;
                    command.Parameters.Add("@result", SqlDbType.Decimal).Value = game.Result;
                    command.Parameters.Add("@turn", SqlDbType.Bit).Value = game.Turn;
                    command.Parameters.Add("@winner", SqlDbType.Bit).Value = game.Winner;
                    command.Parameters.Add("@board", SqlDbType.VarChar).Value = ConvertBoardDescriptionToString(game.Board);
                    command.Parameters.Add("@movesLog", SqlDbType.VarChar).Value = ConvertMovesLogToSring(game.MovesLog);

                    command.ExecuteNonQuery();
                }
            }
        }

        // Players

        public List<Player> GetPlayers()
        {
            using (SqlConnection connection = new SqlConnection(this._connectionString))
            {
                connection.Open();
                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "[dbo].[GetPlayers]";

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        var players = new List<Player>();
                        if (reader.HasRows)
                        {
                            int ordPlayerId = reader.GetOrdinal("PlayerId");
                            int ordName = reader.GetOrdinal("Name");

                            while (reader.Read())
                            {
                                players.Add(
                                    new Player
                                    {
                                        PlayerId = reader.GetInt32(ordPlayerId),
                                        Name = reader.GetString(ordName),
                                    });
                            }
                        }
                        return players;
                    }
                }
            }
        }

        public List<Player> GetPlayersWithGames()
        {
            using (SqlConnection connection = new SqlConnection(this._connectionString))
            {
                connection.Open();
                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "[dbo].[GetPlayersResults]";

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        var players = new List<Player>();
                        if (reader.HasRows)
                        {
                            int ordPlayerId = reader.GetOrdinal("PlayerId");
                            int ordName = reader.GetOrdinal("Name");
                            int ordNumberOfGames = reader.GetOrdinal("NumberOfGames");
                            int ordTotalScore = reader.GetOrdinal("TotalScore");

                            while (reader.Read())
                            {
                                players.Add(
                                    new Player
                                    {
                                        PlayerId = reader.GetInt32(ordPlayerId),
                                        Name = reader.GetString(ordName),
                                        NumberOfGames = reader.GetInt32(ordNumberOfGames),
                                        TotalScore = reader.GetDouble(ordTotalScore)
                                    });
                            }
                        }
                        return players;
                    }
                }
            }

        }

        public List<Game> GetPlayerGames(int playerId)
        {
            using (SqlConnection connection = new SqlConnection(this._connectionString))
            {
                connection.Open();
                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "[dbo].[GetPlayerResults]";
                    command.Parameters.Add("@playerId", SqlDbType.Int).Value = playerId;

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        var games = new List<Game>();
                        if (reader.HasRows)
                        {
                            int ordGameId = reader.GetOrdinal("GameId");
                            int ordStartDate = reader.GetOrdinal("StartDate");
                            int ordStatus = reader.GetOrdinal("Status");
                            int ordResult = reader.GetOrdinal("Result");
                            int ordTurn = reader.GetOrdinal("Turn");
                            int ordWinner = reader.GetOrdinal("Winner");
                            int ordWhitesId = reader.GetOrdinal("WhitesId");
                            int ordWhitesName = reader.GetOrdinal("WhitesName");

                            int ordBlacksId = reader.GetOrdinal("BlacksId");
                            int ordBlacksName = reader.GetOrdinal("BlacksName");

                            int ordBoard = reader.GetOrdinal("Board");
                            int ordMovesLog = reader.GetOrdinal("MovesLog");

                            while (reader.Read())
                            {
                                var white = new Player
                                {
                                    PlayerId = reader.GetInt32(ordWhitesId),
                                    Name = reader.GetString(ordWhitesName),
                                };

                                var black = new Player
                                {
                                    PlayerId = reader.GetInt32(ordBlacksId),
                                    Name = reader.GetString(ordBlacksName),
                                };

                                games.Add(
                                    new Game
                                    {
                                        GameId = reader.GetInt32(ordGameId),
                                        StartDate = reader.GetDateTime(ordStartDate),
                                        Status = reader.GetInt32(ordStatus),
                                        Result = reader.GetDouble(ordResult),
                                        Turn = reader.GetBoolean(ordTurn),
                                        Winner = reader.GetBoolean(ordWinner),
                                         White = white,
                                        Black = black,
                                        Board = ConvertStringToBoardDescription(reader.GetString(ordBoard)),
                                        MovesLog = ConvertStringToMovesLog(reader.GetString(ordMovesLog))
                                    });
                            }
                        }
                        return games;
                    }
                }
            }
        }

        private Dictionary<string, string> ConvertStringToBoardDescription(string boardString)
        {
            var boardDescription = new Dictionary<string, string>();
            for (int i = 0; i < boardString.Length - 4; i += 4)
            {
                boardDescription.Add(boardString.Substring(i, 2), boardString.Substring(i + 2, 2));
            }

            return boardDescription;
        }

        private string ConvertBoardDescriptionToString(Dictionary<string, string> boardDescription)
        {
            var boardString = new StringBuilder();
            foreach (var item in boardDescription)
            {
                boardString.Append($"{item.Key}{item.Value}");
            }

            return boardString.ToString();
        }

        private List<Tuple<string, string, string>> ConvertStringToMovesLog(string movesLogString)
        {
            var movesLog = new List<Tuple<string, string, string>>();
            for (int i = 0; i < movesLogString.Length - 6; i += 6)
            {
                movesLog.Add(new Tuple<string, string, string>(movesLogString.Substring(i, 2),
                                                               movesLogString.Substring(i + 2, 2),
                                                               movesLogString.Substring(i + 4, 2)));
            }

            return movesLog;
        }

        private string ConvertMovesLogToSring(List<Tuple<string, string, string>> movesLog)
        {
            var movesLogString = new StringBuilder();
            foreach (var move in movesLog)
            {
                movesLogString.Append($"{move.Item1}{move.Item2}{move.Item3}");
            }

            return movesLogString.ToString();
        }


    }
}
