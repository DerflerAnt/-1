using System.Windows;

namespace SmartGreenhouseSimulator
{
    public partial class SectionSettingsWindow : Window
    {
        // Властивості для зберігання параметрів секції
        public int Temperature { get; private set; }
        public int Humidity { get; private set; }
        public int Light { get; private set; }

        // Конструктор для ініціалізації параметрів
        public SectionSettingsWindow(int temperature, int humidity, int light)
        {
            InitializeComponent();

            // Заповнюємо поля поточними значеннями
            TemperatureBox.Text = temperature.ToString();
            HumidityBox.Text = humidity.ToString();
            LightBox.Text = light.ToString();
        }

        // Обробник події натискання кнопки "Зберегти"
        private void SaveSettings_Click(object sender, RoutedEventArgs e)
        {
            // Перевіряємо, чи користувач ввів коректні числові значення
            if (int.TryParse(TemperatureBox.Text, out int temp) &&
                int.TryParse(HumidityBox.Text, out int hum) &&
                int.TryParse(LightBox.Text, out int light))
            {
                // Зберігаємо нові значення
                Temperature = temp;
                Humidity = hum;
                Light = light;

                // Позначаємо діалог як успішно завершений
                DialogResult = true;
                Close();
            }
            else
            {
                // Виводимо повідомлення про помилку
                MessageBox.Show("Введіть коректні числові значення.", "Помилка");
            }
        }
    }
}