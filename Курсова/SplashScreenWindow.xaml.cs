using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Animation;

namespace SmartGreenhouseSimulator
{
    public partial class SplashScreenWindow : Window
    {
        public SplashScreenWindow()
        {
            InitializeComponent();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Плавне проявлення (fade in)
            var fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(1));
            MainGrid.BeginAnimation(OpacityProperty, fadeIn);

            await Task.Delay(2000); // Загалом чекаємо 2 секунди

            // Плавне зникнення (fade out)
            var fadeOut = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(1));
            fadeOut.Completed += (s, a) =>
            {
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
                this.Close();
            };
            MainGrid.BeginAnimation(OpacityProperty, fadeOut);
        }
    }
}
