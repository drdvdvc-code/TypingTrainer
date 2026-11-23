using System;
using System.Windows.Input;
using TypingTrainer.Helpers;
using TypingTrainer.Models;
using TypingTrainer.Services;

namespace TypingTrainer.ViewModels
{
    public class ProfileViewModel : BaseViewModel
    {
        private readonly UserService _userSvc = new UserService();

        public string Login { get; }
        public int UserId { get; }

        private string _fullName;
        public string FullName { get => _fullName; set => SetProperty(ref _fullName, value); }

        private string _group;
        public string Group { get => _group; set => SetProperty(ref _group, value); }

        // Добавляем поле для старого пароля
        private string _oldPassword;
        public string OldPassword { get => _oldPassword; set => SetProperty(ref _oldPassword, value); }

        private string _newPassword;
        public string NewPassword { get => _newPassword; set => SetProperty(ref _newPassword, value); }

        private string _status;
        public string Status { get => _status; set => SetProperty(ref _status, value); }

        public ICommand SaveCommand { get; }
        public ICommand GoBackCommand { get; }
        public event Action RequestGoBack;

        public ProfileViewModel(User user)
        {
            UserId = user.Id;
            Login = user.Login;
            FullName = user.FullName;
            Group = user.Group;

            SaveCommand = new RelayCommand(Save);
            GoBackCommand = new RelayCommand(() => RequestGoBack?.Invoke());
        }

        private void Save()
        {
            Status = "";

            // 1. Сначала сохраняем изменения профиля (ФИО, группа)
            _userSvc.UpdateProfile(UserId, FullName, Group);

            // 2. Если пользователь ввел новый пароль
            if (!string.IsNullOrWhiteSpace(NewPassword))
            {
                // Проверяем, что он не слишком короткий
                if (!ValidationHelper.IsValidPassword(NewPassword))
                {
                    Status = "Новый пароль слишком короткий (мин. 6 символов)";
                    return;
                }

                // Проверяем, ввел ли он старый пароль
                if (string.IsNullOrWhiteSpace(OldPassword))
                {
                    Status = "Введите старый пароль для смены!";
                    return;
                }

                // Сверяем старый пароль с базой данных
                if (!_userSvc.VerifyPassword(UserId, OldPassword))
                {
                    Status = "Неверный старый пароль!";
                    return;
                }

                // Если всё верно — меняем пароль
                _userSvc.UpdatePassword(UserId, NewPassword);

                // Очищаем поля с паролями
                NewPassword = "";
                OldPassword = "";
            }

            Status = "Сохранено ✔";
        }
    }
}