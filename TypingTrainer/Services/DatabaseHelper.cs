using System;
using System.Data.SQLite;
using System.IO;
using TypingTrainer.Helpers;

namespace TypingTrainer.Services
{
    public static class DatabaseHelper
    {
        private static readonly string DbPath =
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "typingtrainer.db");

        private static string ConnectionString =>
            $"Data Source={DbPath};Version=3;foreign keys=true;";

        public static SQLiteConnection GetConnection()
        {
            var conn = new SQLiteConnection(ConnectionString);
            conn.Open();
            return conn;
        }

        public static void InitializeDatabase()
        {
            if (!File.Exists(DbPath))
                SQLiteConnection.CreateFile(DbPath);

            using var conn = GetConnection();
            using var cmd = conn.CreateCommand();

            // Создание таблицы Users. Слово [Group] обязательно в квадратных скобках!
            cmd.CommandText = @"
                CREATE TABLE IF NOT EXISTS Users (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Login TEXT NOT NULL UNIQUE,
                    PasswordHash TEXT NOT NULL,
                    Role TEXT NOT NULL DEFAULT 'User',
                    FullName TEXT,
                    [Group] TEXT,
                    RegistrationDate TEXT NOT NULL,
                    LastLogin TEXT
                );";
            cmd.ExecuteNonQuery();

            // Создание таблицы Texts (Тексты)
            cmd.CommandText = @"
                CREATE TABLE IF NOT EXISTS Texts (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Content TEXT NOT NULL,
                    DifficultyLevel TEXT NOT NULL,
                    Category TEXT,
                    WordCount INTEGER,
                    CreatedDate TEXT NOT NULL
                );";
            cmd.ExecuteNonQuery();

            // Создание таблицы Results (Результаты)
            cmd.CommandText = @"
                CREATE TABLE IF NOT EXISTS Results (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    UserId INTEGER NOT NULL,
                    TextId INTEGER NOT NULL,
                    Accuracy REAL NOT NULL,
                    ErrorsCount INTEGER NOT NULL,
                    TimeSeconds INTEGER NOT NULL,
                    WPM REAL NOT NULL,
                    DateCompleted TEXT NOT NULL,
                    FOREIGN KEY(UserId) REFERENCES Users(Id),
                    FOREIGN KEY(TextId) REFERENCES Texts(Id)
                );";
            cmd.ExecuteNonQuery();

            // Создание таблицы Tasks (Задания)
            cmd.CommandText = @"
                CREATE TABLE IF NOT EXISTS Tasks (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    UserId INTEGER NOT NULL,
                    TextId INTEGER NOT NULL,
                    TaskType TEXT NOT NULL,
                    IsCompleted INTEGER NOT NULL DEFAULT 0,
                    DifficultyLevel TEXT,
                    AssignedDate TEXT NOT NULL,
                    FOREIGN KEY(UserId) REFERENCES Users(Id),
                    FOREIGN KEY(TextId) REFERENCES Texts(Id)
                );";
            cmd.ExecuteNonQuery();

            SeedData();
        }

        // Метод для заполнения базы тестовыми данными
        private static void SeedData()
        {
            using var conn = GetConnection();

            // Проверяем, есть ли уже данные. Если есть — ничего не добавляем.
            using (var check = new SQLiteCommand("SELECT COUNT(*) FROM Users", conn))
            {
                long count = (long)check.ExecuteScalar();
                if (count > 0) return;
            }

            // --- ДОБАВЛЕНИЕ АДМИНИСТРАТОРОВ ---
            var admins = new[]
            {
                ("admin1", "admin123", "Иванов Иван Иванович", "ADM-01"),
                ("admin2", "admin456", "Петрова Мария Сергеевна", "ADM-01"),
                ("admin3", "admin789", "Сидоров Алексей Петрович", "ADM-01")
            };
            foreach (var (login, pass, name, group) in admins)
            {
                using var cmd = new SQLiteCommand(
                    @"INSERT INTO Users(Login,PasswordHash,Role,FullName,[Group],RegistrationDate)
                      VALUES(@l,@p,'Admin',@n,@g,@d)", conn);
                cmd.Parameters.AddWithValue("@l", login);
                cmd.Parameters.AddWithValue("@p", PasswordHasher.Hash(pass));
                cmd.Parameters.AddWithValue("@n", name);
                cmd.Parameters.AddWithValue("@g", group);
                cmd.Parameters.AddWithValue("@d", DateTime.UtcNow.ToString("o"));
                cmd.ExecuteNonQuery();
            }

            // --- ДОБАВЛЕНИЕ СТУДЕНТОВ ---
            for (int i = 1; i <= 20; i++)
            {
                using var cmd = new SQLiteCommand(
                    @"INSERT INTO Users(Login,PasswordHash,Role,FullName,[Group],RegistrationDate)
                      VALUES(@l,@p,'User',@n,@g,@d)", conn);
                cmd.Parameters.AddWithValue("@l", $"student{i}");
                cmd.Parameters.AddWithValue("@p", PasswordHasher.Hash("pass" + i));
                cmd.Parameters.AddWithValue("@n", $"Студентов Студент №{i}");
                cmd.Parameters.AddWithValue("@g", $"ПИ-{20 + (i % 4)}-1");
                cmd.Parameters.AddWithValue("@d", DateTime.UtcNow.ToString("o"));
                cmd.ExecuteNonQuery();
            }

            // ==========================================
            // БОЛЬШИЕ ТЕКСТЫ ДЛЯ ТРЕНИРОВКИ
            // ==========================================
            string[] texts = {
                // --- ЛЕГКИЕ (около 150-200 символов) ---
                "Слепая печать — это важный навык для любого человека, который много работает за компьютером. Когда вы печатаете не глядя на клавиатуру, ваши мысли не прерываются на поиск нужной буквы. Это позволяет вам работать быстрее, меньше уставать и полностью сосредоточиться на решении важных задач.",
                "Программирование — это не просто написание кода, это искусство решения проблем. Чтобы стать хорошим разработчиком, нужно постоянно учиться, читать документацию и практиковаться каждый день. Начните с малого и постепенно усложняйте задачи, и вы обязательно добьетесь успеха в этом непростом деле.",
                "Чтение книг расширяет наш кругозор и обогащает словарный запас. Чем больше мы читаем, тем легче нам формулировать свои мысли. Психологи утверждают, что регулярное чтение художественной литературы помогает снизить уровень стресса и улучшает работу мозга. Найдите время на хорошую книгу каждый вечер.",
                
                // --- СРЕДНИЕ (около 200-250 символов) ---
                "Алан Тьюринг, выдающийся математик и криптограф, однажды сказал, что машина может считаться разумной, если человек в процессе общения с ней не сможет понять, кто перед ним — человек или компьютер. Этот мысленный эксперимент, известный как тест Тьюринга, до сих пор вызывает жаркие споры среди философов и ученых.",
                "В современном мире информация имеет огромную ценность. Однако не всякая информация полезна. Критическое мышление помогает нам анализировать факты, отделять правду от вымысла и принимать взвешенные решения. Не верьте всему, что видите в интернете, всегда проверяйте источники и смотрите на проблему с разных сторон.",
                "Утро — это самое продуктивное время для работы. Когда мозг отдохнул, он способен генерировать самые неожиданные и креативные идеи. Многие известные писатели и предприниматели встают в пять утра, чтобы посвятить первые часы дня самому важному делу. Попробуйте изменить свой график и посмотрите, как изменится ваша жизнь.",
                "История развития вычислительной техники насчитывает не одно десятилетие. От огромных ламповых машин, занимавших целые комнаты, до современных смартфонов, которые помещаются в кармане, — прогресс не стоит на месте. Каждое новое поколение компьютеров становится меньше, быстрее и доступнее для обычных людей.",

                // --- СЛОЖНЫЕ (около 250-350 символов) ---
                "Кодирование — это процесс создания инструкций для компьютера. Независимо от выбранного языка, будь то Python, C# или JavaScript, основные принципы остаются неизменными: переменные, циклы, условия и функции. Понимание алгоритмов и структур данных является фундаментом, без которого невозможно построить сложное и надежное программное обеспечение. Пишите код так, чтобы его могли прочитать другие.",
                "Технологии развиваются с невероятной скоростью. То, что вчера казалось фантастикой, сегодня становится реальностью. Искусственный интеллект, машинное обучение и нейронные сети уже прочно вошли в нашу жизнь. Они помогают врачам ставить диагнозы, банкам выявлять мошенничество, а программистам писать более эффективный код. Но главным остается человек и его ответственность за технологии.",
                "В языке C# существует множество продвинутых возможностей, таких как асинхронное программирование, LINQ-запросы и рефлексия. Использование этих инструментов требует глубокого понимания архитектуры .NET и принципов работы сборщика мусора. Однако именно они позволяют создавать высокопроизводительные и масштабируемые приложения для любых платформ. Разработка — это постоянное самообразование и адаптация к новым стандартам."
            };

            for (int i = 0; i < texts.Length; i++)
            {
                // Распределяем сложность: 3 легких, 4 средних, 3 сложных
                string difficulty = i < 3 ? "Easy" : (i < 7 ? "Medium" : "Hard");
                int wordCount = texts[i].Split(' ', StringSplitOptions.RemoveEmptyEntries).Length;

                using var cmd = new SQLiteCommand(
                    @"INSERT INTO Texts(Content,DifficultyLevel,Category,WordCount,CreatedDate)
                      VALUES(@c,@d,@cat,@w,@dt)", conn);
                cmd.Parameters.AddWithValue("@c", texts[i]);
                cmd.Parameters.AddWithValue("@d", difficulty);
                cmd.Parameters.AddWithValue("@cat", "Общее");
                cmd.Parameters.AddWithValue("@w", wordCount);
                cmd.Parameters.AddWithValue("@dt", DateTime.UtcNow.ToString("o"));
                cmd.ExecuteNonQuery();
            }
        }
    }
}