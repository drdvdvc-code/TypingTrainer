using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TypingTrainer.ViewModels;

namespace TypingTrainer.Views
{
    public partial class LoginView : UserControl
    {
        public LoginView()
        {
            InitializeComponent();
        }

        // ⚠️ НОВЫЙ МЕТОД: Обработка нажатия клавиш
        private void LoginView_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            // Если нажали Enter
            if (e.Key == Key.Enter)
            {
                // Если экран привязан к LoginViewModel и у команды входа есть разрешение на выполнение
                if (DataContext is LoginViewModel vm && vm.LoginCommand.CanExecute(null))
                {
                    // Выполняем команду "Войти"
                    vm.LoginCommand.Execute(null);

                    // Говорим системе, что мы обработали клавишу (чтобы она не "пикала")
                    e.Handled = true;
                }
            }
        }

        // Синхронизация: когда печатаем в скрытом поле
        private void PasswordInput_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is LoginViewModel vm)
            {
                vm.Password = PasswordInput.Password;
                if (PasswordTextBox.Visibility == Visibility.Visible)
                    PasswordTextBox.Text = PasswordInput.Password;
            }
        }

        // Синхронизация: когда печатаем в видимом поле
        private void PasswordTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (DataContext is LoginViewModel vm)
            {
                vm.Password = PasswordTextBox.Text;
                if (PasswordInput.Visibility == Visibility.Visible)
                    PasswordInput.Password = PasswordTextBox.Text;
            }
        }

        // Нажатие на глазик
        private void ShowPasswordBtn_Click(object sender, RoutedEventArgs e)
        {
            if (PasswordInput.Visibility == Visibility.Visible)
            {
                PasswordTextBox.Text = PasswordInput.Password;
                PasswordTextBox.Visibility = Visibility.Visible;
                PasswordInput.Visibility = Visibility.Collapsed;
                ShowPasswordBtn.Content = "🙈";
                PasswordTextBox.Focus();
                PasswordTextBox.CaretIndex = PasswordTextBox.Text.Length;
            }
            else
            {
                PasswordInput.Password = PasswordTextBox.Text;
                PasswordInput.Visibility = Visibility.Visible;
                PasswordTextBox.Visibility = Visibility.Collapsed;
                ShowPasswordBtn.Content = "👁️";
                PasswordInput.Focus();
            }
        }
    }
}