using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using TypingTrainer.ViewModels;

namespace TypingTrainer.Views
{
    public partial class TypingTrainerView : UserControl
    {
        public TypingTrainerView()
        {
            InitializeComponent();

            DataContextChanged += TypingTrainerView_DataContextChanged;
            Loaded += (_, __) => TypingBox.Focus();
        }

        // Логика движения фона за мышкой
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

        private void TypingTrainerView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue is TypingTrainerViewModel oldVm)
            {
                oldVm.ErrorOccurred -= OnErrorOccurred;
                oldVm.PropertyChanged -= ViewModel_PropertyChanged;
            }

            if (e.NewValue is TypingTrainerViewModel newVm)
            {
                newVm.ErrorOccurred += OnErrorOccurred;
                newVm.PropertyChanged += ViewModel_PropertyChanged;
            }
        }

        // Тряска экрана при ошибке
        private void OnErrorOccurred()
        {
            var shakeAnimation = new DoubleAnimation
            {
                From = -8,
                To = 8,
                Duration = TimeSpan.FromMilliseconds(40),
                AutoReverse = true,
                RepeatBehavior = new RepeatBehavior(3)
            };

            ShakeTransform.BeginAnimation(System.Windows.Media.TranslateTransform.XProperty, shakeAnimation);
        }

        // Динамичное комбо (цвет, свечение, пружина)
        private void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(TypingTrainerViewModel.Combo))
            {
                var vm = (TypingTrainerViewModel)sender;

                // 1. Меняем цвета в зависимости от комбо
                if (vm.Combo == 0)
                {
                    ComboContainer.Opacity = 0.3;
                    ComboText.Foreground = Brushes.White;
                    ComboGlow.Color = Colors.Transparent;
                }
                else
                {
                    ComboContainer.Opacity = 1.0;

                    if (vm.Combo < 30)
                    {
                        ComboText.Foreground = Brushes.White;
                        ComboGlow.Color = Colors.White;
                    }
                    else if (vm.Combo < 60)
                    {
                        ComboText.Foreground = Brushes.Gold; // Желтый
                        ComboGlow.Color = Colors.Gold;
                    }
                    else if (vm.Combo < 100)
                    {
                        ComboText.Foreground = Brushes.Orange; // Оранжевый
                        ComboGlow.Color = Colors.Orange;
                    }
                    else
                    {
                        ComboText.Foreground = Brushes.Crimson; // Ярко-красный
                        ComboGlow.Color = Colors.Crimson;
                    }
                }

                // 2. Запускаем пружинистую анимацию (Pop)
                if (vm.Combo > 0)
                {
                    var bounceAnimation = new DoubleAnimationUsingKeyFrames();
                    bounceAnimation.KeyFrames.Add(new EasingDoubleKeyFrame(1.5, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(40))));
                    bounceAnimation.KeyFrames.Add(new EasingDoubleKeyFrame(1.0, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(150))));

                    ComboScale.BeginAnimation(System.Windows.Media.ScaleTransform.ScaleXProperty, bounceAnimation);
                    ComboScale.BeginAnimation(System.Windows.Media.ScaleTransform.ScaleYProperty, bounceAnimation);
                }
                else
                {
                    var breakAnimation = new DoubleAnimationUsingKeyFrames();
                    breakAnimation.KeyFrames.Add(new EasingDoubleKeyFrame(1.2, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(50))));
                    breakAnimation.KeyFrames.Add(new EasingDoubleKeyFrame(1.0, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(100))));

                    ComboScale.BeginAnimation(System.Windows.Media.ScaleTransform.ScaleXProperty, breakAnimation);
                    ComboScale.BeginAnimation(System.Windows.Media.ScaleTransform.ScaleYProperty, breakAnimation);
                }
            }
        }

        // Блокировка Backspace, Delete, Ctrl+V
        private void TypingBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Back || e.Key == Key.Delete)
                e.Handled = true;

            if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                if (e.Key == Key.V || e.Key == Key.X)
                    e.Handled = true;
            }
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            TypingBox.Focus();
        }

        private void NextTextButton_Click(object sender, RoutedEventArgs e)
        {
            TypingBox.Focus();
        }
    }
}