using System.Text.RegularExpressions;

namespace TypingTrainer.Helpers
{
    public static class ValidationHelper
    {
        // Логин: 3-20 символов, только латиница, цифры, _, -
        public static bool IsValidLogin(string login)
        {
            if (string.IsNullOrWhiteSpace(login)) return false;
            return Regex.IsMatch(login, @"^[a-zA-Z0-9_\-]{3,20}$");
        }

        // Пароль: минимум 6 символов
        public static bool IsValidPassword(string password)
        {
            return !string.IsNullOrEmpty(password) && password.Length >= 6;
        }

        // ФИО: минимум 2 слова, только буквы и пробелы
        public static bool IsValidFullName(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return false;
            return Regex.IsMatch(name, @"^[А-Яа-яЁёA-Za-z\s\-]{2,100}$");
        }
    }
}