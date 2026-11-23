using System.ComponentModel;
using System.Windows.Media;

namespace TypingTrainer.Models
{
    public class CharDisplay : INotifyPropertyChanged
    {
        private string _text;
        public string Text
        {
            get => _text;
            set { _text = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Text))); }
        }

        private Brush _foreground;
        public Brush Foreground
        {
            get => _foreground;
            set { _foreground = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Foreground))); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}