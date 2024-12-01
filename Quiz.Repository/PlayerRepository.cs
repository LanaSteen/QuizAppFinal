using Quiz.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Text.Json;

namespace Quiz.Repository
{
    public class PlayerRepository
    {
        private string dataFolder;
        private string playerFilePath;

        public PlayerRepository(string dataFolder)
        {
            this.dataFolder = dataFolder;
            playerFilePath = Path.Combine(dataFolder, "players.json");
        }

        public List<Player> LoadPlayers()
        {
            if (!File.Exists(playerFilePath))
            {
                return new List<Player>();
            }

            var json = File.ReadAllText(playerFilePath);
            return JsonSerializer.Deserialize<List<Player>>(json) ?? new List<Player>();
        }


        public void SavePlayers(List<Player> players)
        {
            var json = JsonSerializer.Serialize(players, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(playerFilePath, json);
        }

        public void AddOrUpdatePlayer(Player player)
        {
            var players = LoadPlayers();
            var existingPlayer = players.Find(p => p.UserId == player.UserId);

            if (existingPlayer != null)
            {
                existingPlayer.Username = player.Username;
                existingPlayer.BestScore = player.BestScore;
                existingPlayer.QuizzesCreated = player.QuizzesCreated ?? new List<Quize>();  
                existingPlayer.ActiveNow = player.ActiveNow;
                existingPlayer.LastLogin = player.LastLogin;
            }
            else
            {
               
                players.Add(new Player
                {
                    UserId = player.UserId,
                    Username = player.Username,
                    BestScore = player.BestScore,
                    QuizzesCreated = player.QuizzesCreated ?? new List<Quize>(),  
                    ActiveNow = player.ActiveNow,
                    LastLogin = player.LastLogin
                });
            }

            SavePlayers(players);
        }
     
    }
}
