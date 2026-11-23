using System.Windows.Controls;
using System.Windows.Input;

namespace TypingTrainer.Views
{
    public partial class AboutView : UserControl
    {
        public AboutView()
        {
            InitializeComponent();
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
    }
}