using Quiz.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace Quiz.Repository
{
    public class AuthRepository
    {
        private readonly string _filePath;
        private const string FileName = "auth_users.json";

        public AuthRepository(string dataFolder)
        {
            _filePath = Path.Combine(dataFolder, FileName);
        }
        public void SaveAuthUser(AuthUser user, string password)
        {
            var users = LoadAuthUsers();

            var existingUser = users.FirstOrDefault(u => u.Username == user.Username);

            if (existingUser != null)
            {
                if (existingUser.PasswordHash == password)
                {
                    existingUser.LastLogin = DateTime.Now;
                    existingUser.ActiveNow = true; 
                }
                else
                {
                    return;
                }
            }
            else
            {
          
                if (users.Any())
                {
                    var lastUser = users.OrderByDescending(u => u.UserId).FirstOrDefault();
                    user.UserId = lastUser != null ? lastUser.UserId + 1 : 1;
                }
                else
                {
                    user.UserId = 1;
                }

                user.ActiveNow = true;
                user.LastLogin = DateTime.Now;
                users.Add(user);
            }

  
            foreach (var u in users)
            {
                if (u.UserId != user.UserId)
                {
                    u.ActiveNow = false; 
                }
            }

          
            using (var fileStream = new FileStream(_filePath, FileMode.Create, FileAccess.Write))
            using (var writer = new StreamWriter(fileStream, Encoding.UTF8))
            {
                var json = JsonSerializer.Serialize(users, new JsonSerializerOptions { WriteIndented = true });
                writer.Write(json);
            }
        }
        public List<AuthUser> LoadAuthUsers()
        {
            if (!File.Exists(_filePath))
            {
                return new List<AuthUser>();
            }

            using (var fileStream = new FileStream(_filePath, FileMode.Open, FileAccess.Read))
            using (var reader = new StreamReader(fileStream, Encoding.UTF8))
            {
                var jsonData = reader.ReadToEnd();
                return JsonSerializer.Deserialize<List<AuthUser>>(jsonData);
            }
        }

        public AuthUser GetAuthUserByUsername(string username)
        {
            var users = LoadAuthUsers();
            return users.FirstOrDefault(u => u.Username == username);
        }
    }
}
