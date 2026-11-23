using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using TypingTrainer.Helpers;
using TypingTrainer.Models;
using TypingTrainer.Services;

namespace TypingTrainer.ViewModels
{
    public class AdminPanelViewModel : BaseViewModel
    {
        private readonly UserService _userSvc = new UserService();
        private readonly TextService _textSvc = new TextService();

        public ObservableCollection<User> Users { get; } = new();
        public ObservableCollection<Text> Texts { get; } = new();

        private User _selectedUser;
        public User SelectedUser
        {
            get => _selectedUser;
            set
            {
                SetProperty(ref _selectedUser, value);
                // Принудительно обновляем кнопки (чтобы кнопка удаления сразу погасла, если выбран админ)
                CommandManager.InvalidateRequerySuggested();
            }
        }

        private Text _selectedText;
        public Text SelectedText
        {
            get => _selectedText;
            set
            {
                SetProperty(ref _selectedText, value);
                CommandManager.InvalidateRequerySuggested();
            }
        }

        // Свойства для добавления нового текста
        private string _newTextContent = "";
        public string NewTextContent
        {
            get => _newTextContent;
            set => SetProperty(ref _newTextContent, value);
        }

        private string _newTextDifficulty = "Easy";
        public string NewTextDifficulty
        {
            get => _newTextDifficulty;
            set => SetProperty(ref _newTextDifficulty, value);
        }

        public string[] Difficulties { get; } = { "Easy", "Medium", "Hard" };

        public ICommand DeleteUserCommand { get; }
        public ICommand DeleteTextCommand { get; }
        public ICommand AddTextCommand { get; }
        public ICommand GoBackCommand { get; }

        public event Action RequestGoBack;

        public AdminPanelViewModel()
        {
            // ⚠️ ВАЖНО: Здесь добавлено условие, запрещающее удаление админов.
            // Кнопка "Удалить" будет активна, только если выбран пользователь, и его роль НЕ "Admin".
            DeleteUserCommand = new RelayCommand(
                _ => DeleteUser(),
                _ => SelectedUser != null && SelectedUser.Role != "Admin");

            // Для текстов: кнопка активна, если выбран хотя бы один текст
            DeleteTextCommand = new RelayCommand(
                _ => DeleteText(),
                _ => SelectedText != null);

            AddTextCommand = new RelayCommand(_ => AddText());
            GoBackCommand = new RelayCommand(_ => RequestGoBack?.Invoke());

            Reload();
        }

        public void Reload()
        {
            Users.Clear(); Texts.Clear();
            foreach (var u in _userSvc.GetAll()) Users.Add(u);
            foreach (var t in _textSvc.GetAll()) Texts.Add(t);
        }

        private void DeleteUser()
        {
            if (SelectedUser == null) return;

            // Дополнительная защита (на случай, если кто-то обойдет интерфейс)
            if (SelectedUser.Role == "Admin")
            {
                MessageBox.Show("Удаление администраторов запрещено!", "Ошибка доступа",
                                MessageBoxButton.OK, MessageBoxImage.Stop);
                return;
            }

            var res = MessageBox.Show(
                $"Вы уверены, что хотите удалить пользователя {SelectedUser.Login}?",
                "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (res != MessageBoxResult.Yes) return;

            _userSvc.Delete(SelectedUser.Id);
            Reload();
        }

        private void DeleteText()
        {
            if (SelectedText == null) return;

            var res = MessageBox.Show(
                $"Удалить текст #{SelectedText.Id}?",
                "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (res != MessageBoxResult.Yes) return;

            _textSvc.Delete(SelectedText.Id);
            Reload();
        }

        private void AddText()
        {
            if (string.IsNullOrWhiteSpace(NewTextContent))
            {
                MessageBox.Show("Введите текст для добавления!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            _textSvc.Add(NewTextContent, NewTextDifficulty, "Общее");

            NewTextContent = "";
            Reload();
        }
    }
}