using System;
using System.Data.SQLite;
using System.IO;

namespace TypingTrainer.Services
{
    public class ReportService
    {
        /// <summary>Экспорт прогресса пользователя в CSV.</summary>
        public void ExportUserProgressToCsv(int userId, string filePath)
        {
            using var conn = DatabaseHelper.GetConnection();
            using var cmd = new SQLiteCommand(
                @"SELECT DateCompleted, WPM, Accuracy, ErrorsCount, TimeSeconds
                  FROM Results WHERE UserId = @u ORDER BY DateCompleted", conn);
            cmd.Parameters.AddWithValue("@u", userId);

            using var writer = new StreamWriter(filePath, false, System.Text.Encoding.UTF8);
            writer.WriteLine("Дата,WPM,Точность,Ошибки,Секунды");

            using var r = cmd.ExecuteReader();
            while (r.Read())
            {
                writer.WriteLine(
                    $"{r["DateCompleted"]},{r["WPM"]},{r["Accuracy"]},{r["ErrorsCount"]},{r["TimeSeconds"]}");
            }
        }

        /// <summary>Экспорт общего рейтинга в CSV.</summary>
        public void ExportGlobalRatingToCsv(string filePath)
        {
            var stats = new StatisticsService();
            var rating = stats.GetGlobalRating();

            using var writer = new StreamWriter(filePath, false, System.Text.Encoding.UTF8);
            writer.WriteLine("Место,Логин,Лучший WPM,Средняя точность");

            for (int i = 0; i < rating.Count; i++)
            {
                writer.WriteLine($"{i + 1},{rating[i].Login},{rating[i].BestWpm},{rating[i].AvgAccuracy:F1}");
            }
        }
    }
}