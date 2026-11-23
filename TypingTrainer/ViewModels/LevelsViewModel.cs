using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using TypingTrainer.Helpers;

namespace TypingTrainer.ViewModels
{
    public class LevelsViewModel : BaseViewModel
    {
        public class LevelItem
        {
            public int Number { get; set; }
            public string Text { get; set; }
        }

        public ObservableCollection<LevelItem> Levels { get; } = new();

        public ICommand SelectLevelCommand { get; }
        public ICommand GoBackCommand { get; }

        // Теперь передаем номер уровня (индекс)
        public event Action<int> LevelSelected;
        public event Action RequestGoBack;

        public LevelsViewModel()
        {
            var levelsList = Services.LevelService.GetLevels();
            for (int i = 0; i < levelsList.Count; i++)
            {
                Levels.Add(new LevelItem { Number = i + 1, Text = levelsList[i] });
            }

            SelectLevelCommand = new RelayCommand(param =>
            {
                if (param is LevelItem level)
                {
                    LevelSelected?.Invoke(level.Number - 1); // Передаем индекс (0, 1, 2...)
                }
            });

            GoBackCommand = new RelayCommand(_ => RequestGoBack?.Invoke());
        }
    }
}