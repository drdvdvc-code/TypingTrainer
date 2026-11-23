using System;

namespace TypingTrainer.Models
{
    public class Text
    {
        public int Id { get; set; }
        public string Content { get; set; }       // сам текст для печати
        public string DifficultyLevel { get; set; } // "Easy", "Medium", "Hard"
        public string Category { get; set; }      // "Общее", "Программирование" и т.п.
        public int WordCount { get; set; }
        public DateTime CreatedDate { get; set; }

        // Сколько символов в тексте — считаем один раз при загрузке
        public int CharCount => Content?.Length ?? 0;
    }
}