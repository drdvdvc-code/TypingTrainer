using System;

namespace TypingTrainer.Models
{
    public class Task
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int TextId { get; set; }
        public string TaskType { get; set; }     // "daily" или "permanent"
        public bool IsCompleted { get; set; }
        public string DifficultyLevel { get; set; }
        public DateTime AssignedDate { get; set; }
    }
}