using SmartGreenhouseSimulator.Models;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SmartGreenhouseSimulator
{
    public partial class AddPlantWindow : Window
    {
        public Plant NewPlant { get; private set; }

        public AddPlantWindow()
        {
            InitializeComponent();
        }

        // Обробник події вибору культури
        private void PlantTypePicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem selectedItem = (ComboBoxItem)PlantTypePicker.SelectedItem;
            if (selectedItem != null)
            {
                string[] optimalValues = selectedItem.Tag.ToString().Split(',');
                OptimalConditionsDisplay.Text = $"Температура: {optimalValues[0].Trim()}°C, " +
                                               $"Вологість: {optimalValues[1].Trim()}%, " +
                                               $"Освітленість: {optimalValues[2].Trim()} люменів";
            }
        }

        // Обробник події додавання культури
        private void AddPlant_Click(object sender, RoutedEventArgs e)
        {
            ComboBoxItem plantSelectedItem = (ComboBoxItem)PlantTypePicker.SelectedItem;
            if (plantSelectedItem == null)
            {
                MessageBox.Show("Виберіть вид культури.", "Помилка");
                return;
            }

            string[] optimalValues = plantSelectedItem.Tag.ToString().Split(',');
            if (optimalValues.Length < 3)
            {
                MessageBox.Show("Неправильний формат оптимальних параметрів культури.", "Помилка");
                return;
            }

            if (!int.TryParse(optimalValues[0].Trim().Trim('('), out int temp) ||
                !int.TryParse(optimalValues[1].Trim(), out int humidity) ||
                !int.TryParse(optimalValues[2].Trim().Trim(')'), out int light))
            {
                MessageBox.Show("Помилка у форматі параметрів.", "Помилка");
                return;
            }

            int selectedSection = SectionPicker.SelectedIndex;
            string plantName = plantSelectedItem.Content.ToString();
            Brush color = Brushes.Yellow;

            // Створюємо об'єкт без updateColorAction — він буде призначений у MainWindow
            NewPlant = new Plant(plantName, color, temp, humidity, light, selectedSection, null);

            DialogResult = true;
            Close();
        }

    }
}