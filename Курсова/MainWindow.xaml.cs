using SmartGreenhouseSimulator.Models;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace SmartGreenhouseSimulator
{
    public partial class MainWindow : Window
    {
        private List<Plant> _plants = new List<Plant>();
        private int _harvestedCount = 0;
        private int _diedCount = 0;

        private int[] _sectionTemperatures = { 22, 25, 20, 30 };
        private int[] _sectionHumidities = { 60, 70, 50, 80 };
        private int[] _sectionLights = { 500, 600, 400, 700 };

        public MainWindow()
        {
            InitializeComponent();
            UpdateStatusDisplay();
        }
        private void ShowStatistics_Click(object sender, RoutedEventArgs e)
        {
            string message = "Статистика теплиці:\n" +
                             $"Зібрано культур: {_harvestedCount}\n" +
                             $"Загинуло культур: {_diedCount}\n" +
                             $"Наразі в теплиці: {_plants.Count}";

            MessageBox.Show(message, "Статистика");
        }
        private void UpdateStatusDisplay()
        {
            GreenhouseStatus.Text = "Статус секцій:\n" +
                $"Секція 1: {_sectionTemperatures[0]}°C, {_sectionHumidities[0]}%, {_sectionLights[0]} люменів\n" +
                $"Секція 2: {_sectionTemperatures[1]}°C, {_sectionHumidities[1]}%, {_sectionLights[1]} люменів\n" +
                $"Секція 3: {_sectionTemperatures[2]}°C, {_sectionHumidities[2]}%, {_sectionLights[2]} люменів\n" +
                $"Секція 4: {_sectionTemperatures[3]}°C, {_sectionHumidities[3]}%, {_sectionLights[3]} люменів\n";
        }

        private Canvas GetSectionCanvas(int sectionIndex)
        {
            switch (sectionIndex)
            {
                case 0: return Section1;
                case 1: return Section2;
                case 2: return Section3;
                case 3: return Section4;
                default: throw new ArgumentOutOfRangeException(nameof(sectionIndex));
            }
        }

        private void UpdatePlantStatuses()
        {
            foreach (var plant in _plants)
            {
                int section = plant.Section;
                plant.UpdateStatus(
                    _sectionTemperatures[section],
                    _sectionHumidities[section],
                    _sectionLights[section]
                );
            }
        }

        private void EditSectionParameters(int sectionIndex)
        {
            var dialog = new SectionSettingsWindow(
                _sectionTemperatures[sectionIndex],
                _sectionHumidities[sectionIndex],
                _sectionLights[sectionIndex]);

            if (dialog.ShowDialog() == true)
            {
                _sectionTemperatures[sectionIndex] = dialog.Temperature;
                _sectionHumidities[sectionIndex] = dialog.Humidity;
                _sectionLights[sectionIndex] = dialog.Light;

                UpdateStatusDisplay();
                UpdatePlantStatuses();
            }
        }

        private void EditSection1(object sender, RoutedEventArgs e) => EditSectionParameters(0);
        private void EditSection2(object sender, RoutedEventArgs e) => EditSectionParameters(1);
        private void EditSection3(object sender, RoutedEventArgs e) => EditSectionParameters(2);
        private void EditSection4(object sender, RoutedEventArgs e) => EditSectionParameters(3);

        private void AddPlant_Click(object sender, RoutedEventArgs e)
        {
            var addPlantWindow = new AddPlantWindow();
            if (addPlantWindow.ShowDialog() == true)
            {
                Plant plant = addPlantWindow.NewPlant;

                Canvas sectionCanvas = GetSectionCanvas(plant.Section);
                Grid plantContainer = new Grid { Width = 80, Height = 100 };

                Rectangle plantField = new Rectangle
                {
                    Fill = Brushes.Yellow,
                    Stroke = Brushes.Black,
                    StrokeThickness = 2,
                    Height = 60,
                    VerticalAlignment = VerticalAlignment.Top
                };
                plantContainer.Children.Add(plantField);

                TextBlock plantName = new TextBlock
                {
                    Text = plant.Name,
                    Foreground = Brushes.Black,
                    FontSize = 14,
                    TextWrapping = TextWrapping.Wrap,
                    TextAlignment = TextAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(0, 0, 0, 20)
                };
                plantContainer.Children.Add(plantName);

                ProgressBar waterBar = new ProgressBar
                {
                    Minimum = 0,
                    Maximum = 100,
                    Value = plant.WaterLevel,
                    Height = 10,
                    VerticalAlignment = VerticalAlignment.Bottom,
                    Margin = new Thickness(5, 0, 5, 5)
                };
                plantContainer.Children.Add(waterBar);

                int row = sectionCanvas.Children.Count / 3;
                int col = sectionCanvas.Children.Count % 3;
                Canvas.SetLeft(plantContainer, col * 105 + 10);
                Canvas.SetTop(plantContainer, row * 115 + 10);
                sectionCanvas.Children.Add(plantContainer);

                plant.BindVisual(plantField, waterBar);
                plant.UpdateStatus(_sectionTemperatures[plant.Section],
                                   _sectionHumidities[plant.Section],
                                   _sectionLights[plant.Section]);

                _plants.Add(plant);
                UpdateStatusDisplay();
            }
        }


        private void AutoWaterPlants_Click(object sender, RoutedEventArgs e)
        {
            foreach (var plant in _plants)
            {
                plant.WaterLevel += 20;
                if (plant.WaterLevel > 100) plant.WaterLevel = 100;

                plant.UpdateStatus(
                    _sectionTemperatures[plant.Section],
                    _sectionHumidities[plant.Section],
                    _sectionLights[plant.Section]);
            }

            UpdateStatusDisplay();
            MessageBox.Show("Всі культури було полито.", "Інформація");
        }

        private void HarvestPlant_Click(object sender, RoutedEventArgs e)
        {
            var harvestedPlants = _plants.FindAll(p => p.Status == "Гарний стан");

            if (harvestedPlants.Count == 0)
            {
                MessageBox.Show("Немає культур, готових до збору.", "Інформація");
                return;
            }

            foreach (var plant in harvestedPlants)
            {
                RemovePlantFromMap(plant);
                _plants.Remove(plant);
                _harvestedCount++;
            }

            UpdateStatusDisplay();
            MessageBox.Show($"Зібрано {harvestedPlants.Count} культур.", "Успіх");
        }

        private void RemovePlantFromMap(Plant plant)
        {
            Canvas sectionCanvas = GetSectionCanvas(plant.Section);

            for (int i = 0; i < sectionCanvas.Children.Count; i++)
            {
                var child = sectionCanvas.Children[i] as Grid;
                if (child != null && child.Children.Count > 1)
                {
                    var textBlock = child.Children[1] as TextBlock;
                    if (textBlock != null && textBlock.Text == plant.Name)
                    {
                        sectionCanvas.Children.Remove(child);
                        break;
                    }
                }
            }
        }
        private void RemoveBadPlants_Click(object sender, RoutedEventArgs e)
        {
            var badPlants = _plants.FindAll(p => p.Status == "Поганий стан");

            if (badPlants.Count == 0)
            {
                MessageBox.Show("Немає культур у поганому стані для видалення.", "Інформація");
                return;
            }

            foreach (var plant in badPlants)
            {
                RemovePlantFromMap(plant);
                _plants.Remove(plant);
                _diedCount++;
            }

            UpdateStatusDisplay();
            MessageBox.Show($"Видалено {badPlants.Count} культур у поганому стані.", "Інформація");
        }

        private void ExitApp(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
