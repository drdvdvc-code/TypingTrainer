using System;

namespace TypingTrainer.Helpers
{
    /// <summary>
    /// Класс для безопасного хеширования и проверки паролей.
    /// Использует алгоритм BCrypt (библиотека BCrypt.Net-Next).
    /// </summary>
    public static class PasswordHasher
    {
        /// <summary>
        /// Превращает обычный пароль в хеш (необратимую строку).
        /// </summary>
        public static string Hash(string password)
        {
            if (string.IsNullOrEmpty(password))
                throw new ArgumentException("Пароль не может быть пустым", nameof(password));

            // workFactor: 12 — оптимальная сложность (баланс между безопасностью и скоростью).
            // Обратите внимание на полное имя: BCrypt.Net.BCrypt
            return BCrypt.Net.BCrypt.HashPassword(password, workFactor: 12);
        }

        /// <summary>
        /// Проверяет, соответствует ли введенный пароль сохраненному хешу.
        /// </summary>
        public static bool Verify(string password, string hash)
        {
            // Если пароль или хеш пустые — сразу возвращаем false
            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(hash))
                return false;

            try
            {
                // Полное имя метода для проверки
                return BCrypt.Net.BCrypt.Verify(password, hash);
            }
            catch
            {
                // Если хеш поврежден (например, в БД записана ерунда), 
                // метод Verify может выбросить ошибку. Ловим её и считаем пароль неверным.
                return false;
            }
        }
    }
}