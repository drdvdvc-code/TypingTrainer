using System;
using System.Windows;
using System.Windows.Input;
using TypingTrainer.Helpers;
using TypingTrainer.Models;
using TypingTrainer.Services;

namespace TypingTrainer.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private readonly AuthService _auth = new AuthService();

        private string _login;
        public string Login
        {
            get => _login;
            set => SetProperty(ref _login, value);
        }

        private string _password;
        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        private string _errorMessage;
        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        public ICommand LoginCommand { get; }
        public ICommand GoToRegisterCommand { get; }

        // События для смены экрана — обрабатываются в MainWindow
        public event Action<User> LoginSucceeded;
        public event Action RequestRegister;

        public LoginViewModel()
        {
            LoginCommand = new RelayCommand(DoLogin);
            GoToRegisterCommand = new RelayCommand(() => RequestRegister?.Invoke());
        }

        private void DoLogin()
        {
            ErrorMessage = "";
            var user = _auth.Authenticate(Login, Password);
            if (user == null)
            {
                ErrorMessage = "Неверный логин или пароль";
                return;
            }
            LoginSucceeded?.Invoke(user);
        }
    }
}