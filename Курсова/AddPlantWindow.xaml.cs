using SmartGreenhouseSimulator.Models;
using System.Windows;
using System.Windows.Controls;

namespace SmartGreenhouseSimulator
{
    public partial class AddPlantWindow : Window
    {
        public Plant NewPlant { get; private set; }

        public AddPlantWindow()
        {
            InitializeComponent();
        }

        private void PlantTypePicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem selectedItem = (ComboBoxItem)PlantTypePicker.SelectedItem;
            if (selectedItem != null)
            {
                string name = selectedItem.Content.ToString();
                switch (name)
                {
                    case "Огірки": OptimalConditionsDisplay.Text = "Температура: 22°C, Вологість: 60%, Освітленість: 500 люменів"; break;
                    case "Помідори": OptimalConditionsDisplay.Text = "Температура: 25°C, Вологість: 70%, Освітленість: 600 люменів"; break;
                    case "Банани": OptimalConditionsDisplay.Text = "Температура: 20°C, Вологість: 50%, Освітленість: 400 люменів"; break;
                    case "Яблука": OptimalConditionsDisplay.Text = "Температура: 30°C, Вологість: 80%, Освітленість: 700 люменів"; break;
                    case "Лимон": OptimalConditionsDisplay.Text = "Температура: 28°C, Вологість: 65%, Освітленість: 550 люменів"; break;
                    default: OptimalConditionsDisplay.Text = "Невідомо"; break;
                }
            }
        }

        private void AddPlant_Click(object sender, RoutedEventArgs e)
        {
            ComboBoxItem selectedItem = (ComboBoxItem)PlantTypePicker.SelectedItem;
            if (selectedItem == null)
            {
                MessageBox.Show("Виберіть вид культури.", "Помилка");
                return;
            }

            string plantName = selectedItem.Content.ToString();
            int sectionIndex = SectionPicker.SelectedIndex;

            try
            {
                NewPlant = PlantFactory.CreatePlant(plantName, sectionIndex);
                DialogResult = true;
                Close();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Помилка створення культури: {ex.Message}", "Помилка");
            }
        }
    }
}
