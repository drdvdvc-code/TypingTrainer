using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media.Animation;
using TypingTrainer.Models;
using TypingTrainer.Services;
using TypingTrainer.ViewModels;
using TypingTrainer.Views;

namespace TypingTrainer
{
    public partial class MainWindow : Window
    {
        private User _currentUser;

        // Для перекраски стандартной шапки окна (Windows 11)
        [DllImport("dwmapi.dll")]
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);
        private const int DWMWA_USE_IMMERSIVE_DARK_MODE = 20;
        private const int DWMWA_CAPTION_COLOR = 35;

        public MainWindow()
        {
            InitializeComponent();

            // Инициализация БД при старте
            DatabaseHelper.InitializeDatabase();

            // Перекрашиваем стандартную шапку окна
            this.SourceInitialized += (s, e) => ColorizeTitleBar();

            // Показываем экран входа
            ShowLogin();
        }

        private void ColorizeTitleBar()
        {
            try
            {
                var hwnd = new WindowInteropHelper(this).Handle;

                // Включаем темную тему (чтобы текст и крестик были белыми)
                int useDarkMode = 1;
                DwmSetWindowAttribute(hwnd, DWMWA_USE_IMMERSIVE_DARK_MODE, ref useDarkMode, sizeof(int));

                // Красим в бордовый цвет (#6A1B1A)
                int bgrColor = (26 << 16) | (27 << 8) | 106;
                DwmSetWindowAttribute(hwnd, DWMWA_CAPTION_COLOR, ref bgrColor, sizeof(int));
            }
            catch { }
        }

        // ==========================================
        // НАВИГАЦИЯ
        // ==========================================

        private void ShowLogin()
        {
            var vm = new LoginViewModel();
            vm.LoginSucceeded += OnLoginSucceeded;
            vm.RequestRegister += ShowRegistration;
            var view = new LoginView { DataContext = vm };
            SwitchView(view);
        }

        private void ShowRegistration()
        {
            var vm = new RegistrationViewModel();
            vm.RegistrationSucceeded += ShowLogin;
            vm.RequestLogin += ShowLogin;
            var view = new RegistrationView { DataContext = vm };
            SwitchView(view);
        }

        private void OnLoginSucceeded(User user)
        {
            _currentUser = user;
            ShowMainMenu();
        }

        private void ShowMainMenu()
        {
            var vm = new MainMenuViewModel(_currentUser);
            vm.NavigateTo += NavigateToScreen;
            vm.Logout += () => { _currentUser = null; ShowLogin(); };
            var view = new MainMenuView { DataContext = vm };
            SwitchView(view);
        }

        private void NavigateToScreen(string name)
        {
            switch (name)
            {
                case "TypingTrainer": ShowTrainer(false); break;
                case "Test": ShowTrainer(true); break;
                case "Levels": ShowLevels(); break;
                case "Statistics": ShowStatistics(); break;
                case "Profile": ShowProfile(); break;
                case "Admin": ShowAdmin(); break;
                case "About": ShowAbout(); break;
            }
        }

        private void ShowLevels()
        {
            var vm = new LevelsViewModel();
            vm.LevelSelected += (levelIndex) =>
            {
                var levelsList = Services.LevelService.GetLevels();
                var trainerVm = new TypingTrainerViewModel(_currentUser, null, false, levelsList, levelIndex);
                trainerVm.RequestGoBack += ShowLevels;
                var trainerView = new TypingTrainerView { DataContext = trainerVm };
                SwitchView(trainerView);
            };
            vm.RequestGoBack += ShowMainMenu;
            var view = new LevelsView { DataContext = vm };
            SwitchView(view);
        }

        private void ShowTrainer(bool isTest)
        {
            var vm = new TypingTrainerViewModel(_currentUser, null, isTest);
            vm.RequestGoBack += ShowMainMenu;
            var view = new TypingTrainerView { DataContext = vm };
            SwitchView(view);
        }

        private void ShowStatistics()
        {
            var vm = new StatisticsViewModel(_currentUser);
            vm.RequestGoBack += ShowMainMenu;
            var view = new StatisticsView { DataContext = vm };
            SwitchView(view);
        }

        private void ShowProfile()
        {
            var vm = new ProfileViewModel(_currentUser);
            vm.RequestGoBack += ShowMainMenu;
            var view = new ProfileView { DataContext = vm };
            SwitchView(view);
        }

        private void ShowAdmin()
        {
            if (!_currentUser.IsAdmin) return;
            var vm = new AdminPanelViewModel();
            vm.RequestGoBack += ShowMainMenu;
            var view = new AdminPanelView { DataContext = vm };
            SwitchView(view);
        }

        private void ShowAbout()
        {
            var vm = new AboutViewModel();
            vm.RequestGoBack += ShowMainMenu;
            var view = new AboutView { DataContext = vm };
            SwitchView(view);
        }

        private void SwitchView(UserControl view)
        {
            view.Opacity = 0;
            MainContent.Content = view;
            var anim = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(350));
            view.BeginAnimation(OpacityProperty, anim);
        }
    }
}