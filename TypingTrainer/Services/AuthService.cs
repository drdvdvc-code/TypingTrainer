using System;
using System.Data.SQLite;
using TypingTrainer.Helpers;
using TypingTrainer.Models;

namespace TypingTrainer.Services
{
    public class AuthService
    {
        public User Authenticate(string login, string password)
        {
            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrEmpty(password))
                return null;

            using var conn = DatabaseHelper.GetConnection();
            using var cmd = new SQLiteCommand(
                "SELECT * FROM Users WHERE Login = @l", conn);
            cmd.Parameters.AddWithValue("@l", login);

            using var reader = cmd.ExecuteReader();
            if (!reader.Read()) return null;

            var hash = reader["PasswordHash"].ToString();
            if (!PasswordHasher.Verify(password, hash)) return null;

            var user = new User
            {
                Id = Convert.ToInt32(reader["Id"]),
                Login = reader["Login"].ToString(),
                PasswordHash = hash,
                Role = reader["Role"].ToString(),
                FullName = reader["FullName"]?.ToString(),
                Group = reader["Group"]?.ToString(),
                RegistrationDate = DateTime.Parse(reader["RegistrationDate"].ToString()),
                LastLogin = reader["LastLogin"] == DBNull.Value
                    ? (DateTime?)null
                    : DateTime.Parse(reader["LastLogin"].ToString())
            };
            reader.Close();

            using var upd = new SQLiteCommand(
                "UPDATE Users SET LastLogin = @d WHERE Id = @id", conn);
            upd.Parameters.AddWithValue("@d", DateTime.UtcNow.ToString("o"));
            upd.Parameters.AddWithValue("@id", user.Id);
            upd.ExecuteNonQuery();

            return user;
        }

        public string Register(string login, string password, string fullName, string group)
        {
            if (!ValidationHelper.IsValidLogin(login))
                return "Логин: 3-20 символов, латиница/цифры/_/-";
            if (!ValidationHelper.IsValidPassword(password))
                return "Пароль: минимум 6 символов";
            if (!ValidationHelper.IsValidFullName(fullName))
                return "ФИО: минимум 2 слова, только буквы";

            using var conn = DatabaseHelper.GetConnection();

            using (var check = new SQLiteCommand(
                "SELECT COUNT(*) FROM Users WHERE Login = @l", conn))
            {
                check.Parameters.AddWithValue("@l", login);
                long exists = (long)check.ExecuteScalar();
                if (exists > 0) return "Логин уже занят";
            }

            // ВАЖНО: [Group] в квадратных скобках
            using var cmd = new SQLiteCommand(
                @"INSERT INTO Users(Login,PasswordHash,Role,FullName,[Group],RegistrationDate)
                  VALUES(@l,@p,'User',@n,@g,@d)", conn);
            cmd.Parameters.AddWithValue("@l", login);
            cmd.Parameters.AddWithValue("@p", PasswordHasher.Hash(password));
            cmd.Parameters.AddWithValue("@n", fullName);
            cmd.Parameters.AddWithValue("@g", group ?? "");
            cmd.Parameters.AddWithValue("@d", DateTime.UtcNow.ToString("o"));
            cmd.ExecuteNonQuery();

            return null;
        }
    }
}