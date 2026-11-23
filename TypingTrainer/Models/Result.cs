using System;

namespace TypingTrainer.Models
{
    public class Result
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int TextId { get; set; }
        public double Accuracy { get; set; }    // процент правильных
        public int ErrorsCount { get; set; }
        public int TimeSeconds { get; set; }    // сколько секунд печатал
        public double WPM { get; set; }         // слов в минуту
        public DateTime DateCompleted { get; set; }
    }
}