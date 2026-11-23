using System;

namespace TypingTrainer.Models
{
    /// <summary>
    /// Пользователь системы.
    /// </summary>
    public class User
    {
        public int Id { get; set; }
        public string Login { get; set; }
        public string PasswordHash { get; set; }
        public string Role { get; set; }       // "User" или "Admin"
        public string FullName { get; set; }   // ФИО
        public string Group { get; set; }      // Учебная группа
        public DateTime RegistrationDate { get; set; }
        public DateTime? LastLogin { get; set; }

        // Удобное свойство — правда ли это администратор?
        public bool IsAdmin => Role == "Admin";
    }
}