using System;
using System.Windows.Input;
using TypingTrainer.Helpers;

namespace TypingTrainer.ViewModels
{
    public class AboutViewModel : BaseViewModel
    {
        public ICommand GoBackCommand { get; }
        public event Action RequestGoBack;

        public AboutViewModel()
        {
            GoBackCommand = new RelayCommand(_ => RequestGoBack?.Invoke());
        }
    }
}