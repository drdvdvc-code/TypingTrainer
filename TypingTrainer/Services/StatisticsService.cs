using System;
using System.Collections.Generic;
using System.Data.SQLite;
using TypingTrainer.Models;

namespace TypingTrainer.Services
{
    public class StatisticsService
    {
        public double CalculateWPM(int totalChars, int seconds)
        {
            if (seconds <= 0) return 0;
            double words = totalChars / 5.0;  // стандарт: 1 слово = 5 символов
            return Math.Round(words / (seconds / 60.0), 1);
        }

        public double CalculateAccuracy(int totalChars, int errors)
        {
            if (totalChars == 0) return 0;
            return Math.Round(((totalChars - errors) / (double)totalChars) * 100, 1);
        }

        public void SaveResult(Result result)
        {
            using var conn = DatabaseHelper.GetConnection();
            using var cmd = new SQLiteCommand(
                @"INSERT INTO Results(UserId,TextId,Accuracy,ErrorsCount,TimeSeconds,WPM,DateCompleted)
                  VALUES(@u,@t,@a,@e,@s,@w,@d)", conn);
            cmd.Parameters.AddWithValue("@u", result.UserId);
            cmd.Parameters.AddWithValue("@t", result.TextId);
            cmd.Parameters.AddWithValue("@a", result.Accuracy);
            cmd.Parameters.AddWithValue("@e", result.ErrorsCount);
            cmd.Parameters.AddWithValue("@s", result.TimeSeconds);
            cmd.Parameters.AddWithValue("@w", result.WPM);
            cmd.Parameters.AddWithValue("@d", DateTime.UtcNow.ToString("o"));
            cmd.ExecuteNonQuery();
        }

        public List<Result> GetUserResults(int userId)
        {
            var list = new List<Result>();
            using var conn = DatabaseHelper.GetConnection();
            using var cmd = new SQLiteCommand(
                "SELECT * FROM Results WHERE UserId=@u ORDER BY DateCompleted DESC", conn);
            cmd.Parameters.AddWithValue("@u", userId);
            using var r = cmd.ExecuteReader();
            while (r.Read())
            {
                list.Add(new Result
                {
                    Id = Convert.ToInt32(r["Id"]),
                    UserId = Convert.ToInt32(r["UserId"]),
                    TextId = Convert.ToInt32(r["TextId"]),
                    Accuracy = Convert.ToDouble(r["Accuracy"]),
                    ErrorsCount = Convert.ToInt32(r["ErrorsCount"]),
                    TimeSeconds = Convert.ToInt32(r["TimeSeconds"]),
                    WPM = Convert.ToDouble(r["WPM"]),
                    DateCompleted = DateTime.Parse(r["DateCompleted"].ToString())
                });
            }
            return list;
        }

        public List<(string Login, double BestWpm, double AvgAccuracy)> GetGlobalRating()
        {
            var list = new List<(string, double, double)>();
            using var conn = DatabaseHelper.GetConnection();
            using var cmd = new SQLiteCommand(@"
                SELECT u.Login, MAX(r.WPM) as BestWpm, AVG(r.Accuracy) as AvgAccuracy
                FROM Results r
                JOIN Users u ON u.Id = r.UserId
                GROUP BY r.UserId
                ORDER BY BestWpm DESC
                LIMIT 50;", conn);
            using var r = cmd.ExecuteReader();
            while (r.Read())
            {
                list.Add((
                    r["Login"].ToString(),
                    Convert.ToDouble(r["BestWpm"]),
                    Convert.ToDouble(r["AvgAccuracy"])
                ));
            }
            return list;
        }
    }
}