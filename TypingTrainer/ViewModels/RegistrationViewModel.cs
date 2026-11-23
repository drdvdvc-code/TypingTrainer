using System;
using System.Windows.Input;
using TypingTrainer.Helpers;
using TypingTrainer.Services;

namespace TypingTrainer.ViewModels
{
    public class RegistrationViewModel : BaseViewModel
    {
        private readonly AuthService _auth = new AuthService();

        private string _login;
        public string Login { get => _login; set => SetProperty(ref _login, value); }

        private string _password;
        public string Password { get => _password; set => SetProperty(ref _password, value); }

        private string _fullName;
        public string FullName { get => _fullName; set => SetProperty(ref _fullName, value); }

        private string _group;
        public string Group { get => _group; set => SetProperty(ref _group, value); }

        private string _errorMessage;
        public string ErrorMessage { get => _errorMessage; set => SetProperty(ref _errorMessage, value); }

        public ICommand RegisterCommand { get; }
        public ICommand GoToLoginCommand { get; }
        public event Action RegistrationSucceeded;
        public event Action RequestLogin;

        public RegistrationViewModel()
        {
            RegisterCommand = new RelayCommand(DoRegister);
            GoToLoginCommand = new RelayCommand(() => RequestLogin?.Invoke());
        }

        private void DoRegister()
        {
            ErrorMessage = "";
            string err = _auth.Register(Login, Password, FullName, Group);
            if (err != null) { ErrorMessage = err; return; }
            RegistrationSucceeded?.Invoke();
        }
    }
}