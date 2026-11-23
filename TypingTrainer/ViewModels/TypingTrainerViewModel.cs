using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using TypingTrainer.Helpers;
using TypingTrainer.Models;
using TypingTrainer.Services;

namespace TypingTrainer.ViewModels
{
    public class TypingTrainerViewModel : BaseViewModel
    {
        private readonly User _user;
        private readonly TextService _textService = new TextService();
        private readonly StatisticsService _stats = new StatisticsService();
        private readonly bool _isTestMode;

        private Stopwatch _sw = new Stopwatch();
        private DispatcherTimer _timer;

        public ObservableCollection<CharDisplay> DisplayChars { get; } = new();

        private string _targetText;
        public string TargetText
        {
            get => _targetText;
            private set => SetProperty(ref _targetText, value);
        }

        private int _textId;
        public int TextId
        {
            get => _textId;
            private set => SetProperty(ref _textId, value);
        }

        private int _combo = 0;
        public int Combo { get => _combo; set => SetProperty(ref _combo, value); }

        public event Action ErrorOccurred;

        // Текст для кнопки (меняется в зависимости от режима)
        private string _nextButtonText = "Следующий текст";
        public string NextButtonText { get => _nextButtonText; set => SetProperty(ref _nextButtonText, value); }

        private string _typed = "";
        public string Typed
        {
            get => _typed;
            set
            {
                if (value == null) value = "";
                if (value.Length < _typed.Length || value.Length > (TargetText?.Length ?? 0)) { OnPropertyChanged(nameof(Typed)); return; }

                if (value.Length > _typed.Length && value.Length > 0)
                {
                    if (value[value.Length - 1] == TargetText[value.Length - 1]) Combo++;
                    else { Combo = 0; ErrorOccurred?.Invoke(); }
                }

                if (SetProperty(ref _typed, value))
                {
                    if (!_sw.IsRunning && value.Length > 0) { _sw.Start(); _timer.Start(); }
                    CalculateErrors();
                    UpdateCharColors();
                    OnPropertyChanged(nameof(CurrentChar));
                    OnPropertyChanged(nameof(Progress));

                    if (value.Length == (TargetText?.Length ?? 0) && TargetText?.Length > 0)
                        Finish();
                }
            }
        }

        public char CurrentChar => string.IsNullOrEmpty(TargetText) ? ' ' : Typed.Length < TargetText.Length ? TargetText[Typed.Length] : ' ';
        public string Progress => string.IsNullOrEmpty(TargetText) ? "0%" : $"{Math.Min(100, (Typed.Length * 100) / TargetText.Length)}%";

        private string _time = "0 сек";
        public string Time { get => _time; set => SetProperty(ref _time, value); }

        private string _status = "Начни печатать…";
        public string Status { get => _status; set => SetProperty(ref _status, value); }

        private int _errors = 0;
        public int Errors { get => _errors; set => SetProperty(ref _errors, value); }

        private bool _isCompleted = false;
        public bool IsCompleted { get => _isCompleted; set { SetProperty(ref _isCompleted, value); OnPropertyChanged(nameof(CanType)); } }
        public bool CanType => !IsCompleted;

        public ICommand ResetCommand { get; }
        public ICommand NextTextCommand { get; }
        public ICommand GoBackCommand { get; }

        public event Action RequestGoBack;

        // Переменные для режима уровней
        private List<string> _levelTexts = null;
        private int _currentLevelIndex = 0;

        // Конструктор теперь принимает список уровней и индекс
        public TypingTrainerViewModel(User user, Text selectedText = null, bool isTestMode = false, List<string> levelTexts = null, int levelIndex = 0)
        {
            _user = user;
            _isTestMode = isTestMode;
            _levelTexts = levelTexts;
            _currentLevelIndex = levelIndex;

            if (_levelTexts != null)
            {
                NextButtonText = "Следующий уровень";
            }

            _timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(200) };
            _timer.Tick += Timer_Tick;

            ResetCommand = new RelayCommand(_ => Reset());

            // Логика кнопки "Следующий..."
            NextTextCommand = new RelayCommand(_ =>
            {
                if (_levelTexts != null)
                {
                    // Если это уровни, идем к следующему
                    _currentLevelIndex++;
                    if (_currentLevelIndex < _levelTexts.Count)
                    {
                        LoadText();
                    }
                    else
                    {
                        // Если уровни закончились - выходим в меню выбора
                        RequestGoBack?.Invoke();
                    }
                }
                else
                {
                    // Обычная тренировка (рандомный текст)
                    LoadText(null);
                }
            });

            GoBackCommand = new RelayCommand(_ => { _timer.Stop(); _sw.Stop(); RequestGoBack?.Invoke(); });

            LoadText(selectedText);
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (_isTestMode)
            {
                int remaining = 60 - (int)_sw.Elapsed.TotalSeconds;
                if (remaining <= 0) { Time = "0 сек"; Finish(); }
                else Time = $"{remaining} сек";
            }
            else Time = $"{(int)_sw.Elapsed.TotalSeconds} сек";
        }

        private void LoadText(Text selected = null)
        {
            if (_levelTexts != null && _levelTexts.Count > 0)
            {
                TargetText = _levelTexts[_currentLevelIndex];
                TextId = 0;
            }
            else if (selected != null)
            {
                TargetText = selected.Content;
                TextId = selected.Id;
            }
            else
            {
                var all = _textService.GetAll();
                if (all.Count == 0) { Status = "Нет текстов в БД"; return; }
                var rnd = new Random();
                var t = all[rnd.Next(all.Count)];
                TargetText = t.Content;
                TextId = t.Id;
            }
            Reset();
        }

        private void CalculateErrors()
        {
            int errs = 0;
            for (int i = 0; i < Typed.Length; i++)
                if (i >= TargetText.Length || Typed[i] != TargetText[i]) errs++;
            Errors = errs;
        }

        private void UpdateCharColors()
        {
            if (string.IsNullOrEmpty(TargetText)) return;
            for (int i = 0; i < DisplayChars.Count; i++)
            {
                DisplayChars[i].Text = TargetText[i].ToString();
                if (i < Typed.Length) DisplayChars[i].Foreground = (Typed[i] == TargetText[i]) ? Brushes.Green : Brushes.Red;
                else if (i == Typed.Length) DisplayChars[i].Foreground = Brushes.Black;
                else DisplayChars[i].Foreground = Brushes.Gray;
            }
        }

        private void Finish()
        {
            if (IsCompleted) return;
            IsCompleted = true;
            _sw.Stop(); _timer.Stop();

            int timeSec = (int)_sw.Elapsed.TotalSeconds;
            if (timeSec == 0) timeSec = 1;
            int totalChars = Typed.Length;

            if (totalChars > 0)
            {
                double wpm = _stats.CalculateWPM(totalChars, timeSec);
                double acc = _stats.CalculateAccuracy(totalChars, Errors);

                if (TextId > 0)
                {
                    var res = new Result { UserId = _user.Id, TextId = TextId, Accuracy = acc, ErrorsCount = Errors, TimeSeconds = timeSec, WPM = wpm };
                    _stats.SaveResult(res);
                }
                Status = $"Готово! WPM={wpm}, точность={acc}%, ошибок={Errors}";
            }
            else Status = "Завершено. Вы ничего не напечатали.";
        }

        private void Reset()
        {
            _typed = ""; OnPropertyChanged(nameof(Typed)); Errors = 0; Combo = 0; IsCompleted = false;
            _sw = new Stopwatch();
            Time = _isTestMode ? "60 сек" : "0 сек";
            Status = _isTestMode ? "Тест на 60 секунд. Начни печатать!" : "Начни печатать…";
            _timer.Stop();

            DisplayChars.Clear();
            if (!string.IsNullOrEmpty(TargetText))
                foreach (char c in TargetText) DisplayChars.Add(new CharDisplay { Text = c.ToString(), Foreground = Brushes.Gray });

            UpdateCharColors();
            OnPropertyChanged(nameof(CurrentChar));
            OnPropertyChanged(nameof(Progress));
        }
    }
}