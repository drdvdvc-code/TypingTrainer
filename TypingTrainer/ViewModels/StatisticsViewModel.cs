using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using TypingTrainer.Helpers;
using TypingTrainer.Models;
using TypingTrainer.Services;

namespace TypingTrainer.ViewModels
{
    public class StatisticsViewModel : BaseViewModel
    {
        public ObservableCollection<Result> MyResults { get; } = new();
        public ObservableCollection<RatingRow> GlobalRating { get; } = new();

        public class RatingRow
        {
            public int Place { get; set; }
            public string Login { get; set; }
            public double BestWpm { get; set; }
            public double AvgAccuracy { get; set; }
        }

        // 1. Добавляем команду и событие для возврата
        public ICommand GoBackCommand { get; }
        public event Action RequestGoBack;

        public StatisticsViewModel(User user)
        {
            var statSvc = new StatisticsService();

            // Загружаем мои результаты
            foreach (var r in statSvc.GetUserResults(user.Id))
                MyResults.Add(r);

            // Загружаем общий рейтинг
            int place = 1;
            foreach (var (login, wpm, acc) in statSvc.GetGlobalRating())
            {
                GlobalRating.Add(new RatingRow
                {
                    Place = place++,
                    Login = login,
                    BestWpm = wpm,
                    AvgAccuracy = acc
                });
            }

            // 2. Инициализируем команду возврата
            GoBackCommand = new RelayCommand(_ => RequestGoBack?.Invoke());
        }
    }
}