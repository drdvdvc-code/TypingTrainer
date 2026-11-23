using System;
using System.Windows;
using System.Windows.Threading;

namespace TypingTrainer
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Ловушка для ошибок интерфейса (XAML)
            this.DispatcherUnhandledException += (s, args) =>
            {
                MessageBox.Show(args.Exception.Message + "\n\n" + args.Exception.StackTrace,
                                "ОШИБКА XAML/UI", MessageBoxButton.OK, MessageBoxImage.Error);
                args.Handled = true;
            };

            // Ловушка для общих ошибок
            AppDomain.CurrentDomain.UnhandledException += (s, args) =>
            {
                var ex = args.ExceptionObject as Exception;
                MessageBox.Show(ex?.Message + "\n\n" + ex?.StackTrace,
                                "ОШИБКА", MessageBoxButton.OK, MessageBoxImage.Error);
            };

            // Запускаем главное окно вручную
            try
            {
                MainWindow window = new MainWindow();
                window.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n\n" + ex.StackTrace,
                                "ОШИБКА ЗАПУСКА", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}