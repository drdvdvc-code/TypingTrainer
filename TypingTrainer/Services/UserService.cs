using System;
using System.Collections.Generic;
using System.Data.SQLite;
using TypingTrainer.Helpers;
using TypingTrainer.Models;

namespace TypingTrainer.Services
{
    public class UserService
    {
        public List<User> GetAll()
        {
            var list = new List<User>();
            using var conn = DatabaseHelper.GetConnection();
            using var cmd = new SQLiteCommand("SELECT * FROM Users ORDER BY Id", conn);
            using var r = cmd.ExecuteReader();
            while (r.Read())
            {
                list.Add(new User
                {
                    Id = Convert.ToInt32(r["Id"]),
                    Login = r["Login"].ToString(),
                    Role = r["Role"].ToString(),
                    FullName = r["FullName"]?.ToString(),
                    Group = r["Group"]?.ToString(),
                    RegistrationDate = DateTime.Parse(r["RegistrationDate"].ToString())
                });
            }
            return list;
        }

        public void Delete(int id)
        {
            using var conn = DatabaseHelper.GetConnection();
            using var cmd = new SQLiteCommand("DELETE FROM Users WHERE Id = @id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
        }

        public void UpdateProfile(int id, string fullName, string group)
        {
            using var conn = DatabaseHelper.GetConnection();
            // ВАЖНО: [Group] в квадратных скобках
            using var cmd = new SQLiteCommand(
                "UPDATE Users SET FullName=@n, [Group]=@g WHERE Id=@id", conn);
            cmd.Parameters.AddWithValue("@n", fullName);
            cmd.Parameters.AddWithValue("@g", group);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
        }

        public void UpdatePassword(int id, string newPassword)
        {
            using var conn = DatabaseHelper.GetConnection();
            using var cmd = new SQLiteCommand(
                "UPDATE Users SET PasswordHash=@p WHERE Id=@id", conn);
            cmd.Parameters.AddWithValue("@p", PasswordHasher.Hash(newPassword));
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
        }
   
            // Метод для проверки правильности текущего пароля
        public bool VerifyPassword(int userId, string password)
        {
            using var conn = DatabaseHelper.GetConnection();
            using var cmd = new SQLiteCommand("SELECT PasswordHash FROM Users WHERE Id=@id", conn);
            cmd.Parameters.AddWithValue("@id", userId);

            var hash = cmd.ExecuteScalar()?.ToString();
            if (string.IsNullOrEmpty(hash)) return false;

            return PasswordHasher.Verify(password, hash);
        }
    }
}
