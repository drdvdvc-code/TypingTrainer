using System.Windows.Controls;
using TypingTrainer.ViewModels;

namespace TypingTrainer.Views
{
    public partial class RegistrationView : UserControl
    {
        public RegistrationView() { InitializeComponent(); }

        private void PasswordInput_PasswordChanged(object sender, System.Windows.RoutedEventArgs e)
        {
            if (DataContext is RegistrationViewModel vm)
                vm.Password = PasswordInput.Password;
        }
    }
}