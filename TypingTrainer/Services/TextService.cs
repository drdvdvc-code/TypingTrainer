using System;
using System.Collections.Generic;
using System.Data.SQLite;
using TypingTrainer.Models;

namespace TypingTrainer.Services
{
    public class TextService
    {
        public List<Text> GetAll(string difficultyFilter = null)
        {
            var list = new List<Text>();
            using var conn = DatabaseHelper.GetConnection();
            string sql = "SELECT * FROM Texts";
            if (!string.IsNullOrEmpty(difficultyFilter))
                sql += " WHERE DifficultyLevel = @d";
            sql += " ORDER BY Id";

            using var cmd = new SQLiteCommand(sql, conn);
            if (!string.IsNullOrEmpty(difficultyFilter))
                cmd.Parameters.AddWithValue("@d", difficultyFilter);

            using var r = cmd.ExecuteReader();
            while (r.Read())
            {
                list.Add(new Text
                {
                    Id = Convert.ToInt32(r["Id"]),
                    Content = r["Content"].ToString(),
                    DifficultyLevel = r["DifficultyLevel"].ToString(),
                    Category = r["Category"]?.ToString(),
                    WordCount = Convert.ToInt32(r["WordCount"]),
                    CreatedDate = DateTime.Parse(r["CreatedDate"].ToString())
                });
            }
            return list;
        }

        public void Add(string content, string difficulty, string category)
        {
            int wordCount = content.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length;
            using var conn = DatabaseHelper.GetConnection();
            using var cmd = new SQLiteCommand(
                @"INSERT INTO Texts(Content,DifficultyLevel,Category,WordCount,CreatedDate)
                  VALUES(@c,@d,@cat,@w,@dt)", conn);
            cmd.Parameters.AddWithValue("@c", content);
            cmd.Parameters.AddWithValue("@d", difficulty);
            cmd.Parameters.AddWithValue("@cat", category ?? "Общее");
            cmd.Parameters.AddWithValue("@w", wordCount);
            cmd.Parameters.AddWithValue("@dt", DateTime.UtcNow.ToString("o"));
            cmd.ExecuteNonQuery();
        }

        public void Delete(int id)
        {
            using var conn = DatabaseHelper.GetConnection();
            using var cmd = new SQLiteCommand("DELETE FROM Texts WHERE Id=@id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
        }
    }
}