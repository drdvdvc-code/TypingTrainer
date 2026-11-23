using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TypingTrainer.ViewModels;

namespace TypingTrainer.Views
{
    public partial class ProfileView : UserControl
    {
        public ProfileView() { InitializeComponent(); }

        // ⚠️ Логика движения фона за мышкой
        private void MainGrid_MouseMove(object sender, MouseEventArgs e)
        {
            var position = e.GetPosition(this);
            double centerX = this.ActualWidth / 2;
            double centerY = this.ActualHeight / 2;

            double maxOffset = 25;

            double offsetX = ((position.X - centerX) / centerX) * maxOffset;
            double offsetY = ((position.Y - centerY) / centerY) * maxOffset;

            if (ImageMovement != null)
            {
                ImageMovement.X = -offsetX;
                ImageMovement.Y = -offsetY;
            }
        }

        // ================= СТАРЫЙ ПАРОЛЬ =================

        private void OldPasswordInput_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is ProfileViewModel vm)
            {
                vm.OldPassword = OldPasswordInput.Password;
                if (OldPasswordTextBox.Visibility == Visibility.Visible)
                    OldPasswordTextBox.Text = OldPasswordInput.Password;
            }
        }

        private void OldPasswordTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (DataContext is ProfileViewModel vm)
            {
                vm.OldPassword = OldPasswordTextBox.Text;
                if (OldPasswordInput.Visibility == Visibility.Visible)
                    OldPasswordInput.Password = OldPasswordTextBox.Text;
            }
        }

        private void ShowOldPasswordBtn_Click(object sender, RoutedEventArgs e)
        {
            if (OldPasswordInput.Visibility == Visibility.Visible)
            {
                OldPasswordTextBox.Text = OldPasswordInput.Password;
                OldPasswordTextBox.Visibility = Visibility.Visible;
                OldPasswordInput.Visibility = Visibility.Collapsed;
                ShowOldPasswordBtn.Content = "🙈";
                OldPasswordTextBox.Focus();
                OldPasswordTextBox.CaretIndex = OldPasswordTextBox.Text.Length;
            }
            else
            {
                OldPasswordInput.Password = OldPasswordTextBox.Text;
                OldPasswordInput.Visibility = Visibility.Visible;
                OldPasswordTextBox.Visibility = Visibility.Collapsed;
                ShowOldPasswordBtn.Content = "👁️";
                OldPasswordInput.Focus();
            }
        }

        // ================= НОВЫЙ ПАРОЛЬ =================

        private void NewPasswordInput_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is ProfileViewModel vm)
            {
                vm.NewPassword = NewPasswordInput.Password;
                if (NewPasswordTextBox.Visibility == Visibility.Visible)
                    NewPasswordTextBox.Text = NewPasswordInput.Password;
            }
        }

        private void NewPasswordTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (DataContext is ProfileViewModel vm)
            {
                vm.NewPassword = NewPasswordTextBox.Text;
                if (NewPasswordInput.Visibility == Visibility.Visible)
                    NewPasswordInput.Password = NewPasswordTextBox.Text;
            }
        }

        private void ShowNewPasswordBtn_Click(object sender, RoutedEventArgs e)
        {
            if (NewPasswordInput.Visibility == Visibility.Visible)
            {
                NewPasswordTextBox.Text = NewPasswordInput.Password;
                NewPasswordTextBox.Visibility = Visibility.Visible;
                NewPasswordInput.Visibility = Visibility.Collapsed;
                ShowNewPasswordBtn.Content = "🙈";
                NewPasswordTextBox.Focus();
                NewPasswordTextBox.CaretIndex = NewPasswordTextBox.Text.Length;
            }
            else
            {
                NewPasswordInput.Password = NewPasswordTextBox.Text;
                NewPasswordInput.Visibility = Visibility.Visible;
                NewPasswordTextBox.Visibility = Visibility.Collapsed;
                ShowNewPasswordBtn.Content = "👁️";
                NewPasswordInput.Focus();
            }
        }
    }
}