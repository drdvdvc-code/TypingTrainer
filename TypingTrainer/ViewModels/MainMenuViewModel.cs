using System;
using System.Windows.Input;
using TypingTrainer.Helpers;
using TypingTrainer.Models;

namespace TypingTrainer.ViewModels
{
    public class MainMenuViewModel : BaseViewModel
    {
        public User CurrentUser { get; }

        public ICommand OpenTrainerCommand { get; }
        public ICommand OpenTestCommand { get; }
        public ICommand OpenLevelsCommand { get; } // НОВАЯ КОМАНДА
        public ICommand OpenStatisticsCommand { get; }
        public ICommand OpenProfileCommand { get; }
        public ICommand OpenAboutCommand { get; }
        public ICommand OpenAdminCommand { get; }
        public ICommand LogoutCommand { get; }

        public event Action<string> NavigateTo;
        public event Action Logout;

        public MainMenuViewModel(User user)
        {
            CurrentUser = user;

            OpenTrainerCommand = new RelayCommand(_ => NavigateTo?.Invoke("TypingTrainer"));
            OpenTestCommand = new RelayCommand(_ => NavigateTo?.Invoke("Test"));
            OpenLevelsCommand = new RelayCommand(_ => NavigateTo?.Invoke("Levels")); // СВЯЗЬ С УРОВНЯМИ
            OpenStatisticsCommand = new RelayCommand(_ => NavigateTo?.Invoke("Statistics"));
            OpenProfileCommand = new RelayCommand(_ => NavigateTo?.Invoke("Profile"));
            OpenAboutCommand = new RelayCommand(_ => NavigateTo?.Invoke("About"));

            OpenAdminCommand = new RelayCommand(
                _ => NavigateTo?.Invoke("Admin"),
                _ => CurrentUser != null && CurrentUser.IsAdmin);

            LogoutCommand = new RelayCommand(_ => Logout?.Invoke());
        }
    }
}