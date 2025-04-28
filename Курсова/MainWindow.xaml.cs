using SmartGreenhouseSimulator.Models;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.IO;
using System.Text.Json;
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
                    Height = 20,
                    VerticalAlignment = VerticalAlignment.Top,
                    RenderTransformOrigin = new Point(0.5, 0.5),
                    RenderTransform = new ScaleTransform(1.0, 1.0)
                };
                plantContainer.Children.Add(plantField);

                double finalHeight = 60;
                double growDuration = 1.0; // за замовчуванням 1 секунда

                if (plant.Name == "Огірки")
                {
                    finalHeight = 50;
                    growDuration = 5.8;
                }
                else if (plant.Name == "Помідори")
                {
                    finalHeight = 60;
                    growDuration = 5.8;
                }
                else if (plant.Name == "Банани")
                {
                    finalHeight = 80;
                    growDuration = 5.5;
                }
                else if (plant.Name == "Яблука")
                {
                    finalHeight = 70;
                    growDuration = 5.2;
                }
                else if (plant.Name == "Лимон")
                {
                    finalHeight = 60;
                    growDuration = 5.0;
                }

                // Анімація росту з різними параметрами
                // Анімація росту висоти
                // 👉 Спочатку створюємо анімації
                var growAnimation = new System.Windows.Media.Animation.DoubleAnimation
                {
                    From = 20,
                    To = finalHeight,
                    Duration = TimeSpan.FromSeconds(growDuration),
                    EasingFunction = new System.Windows.Media.Animation.CubicEase
                    {
                        EasingMode = System.Windows.Media.Animation.EasingMode.EaseOut
                    }
                };

                var scaleTransform = plantField.RenderTransform as ScaleTransform;

                // Пульсація по X
                var scaleXAnimation = new System.Windows.Media.Animation.DoubleAnimation
                {
                    From = 1.0,
                    To = 1.05,
                    Duration = TimeSpan.FromSeconds(0.5),
                    AutoReverse = true,
                    RepeatBehavior = System.Windows.Media.Animation.RepeatBehavior.Forever
                };

                // Пульсація по Y
                var scaleYAnimation = new System.Windows.Media.Animation.DoubleAnimation
                {
                    From = 1.0,
                    To = 1.05,
                    Duration = TimeSpan.FromSeconds(0.5),
                    AutoReverse = true,
                    RepeatBehavior = System.Windows.Media.Animation.RepeatBehavior.Forever
                };

                // 👉 Тепер обов'язково ПЕРЕД запуском росту підписуємо завершення росту:
                growAnimation.Completed += (s, args) =>
                {
                    scaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, null);
                    scaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, null);
                    scaleTransform.ScaleX = 1.0;
                    scaleTransform.ScaleY = 1.0;
                };

                // 👉 Тільки тепер запускаємо спочатку пульсацію
                scaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, scaleXAnimation);
                scaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, scaleYAnimation);

                // 👉 І тепер запускаємо ріст
                plantField.BeginAnimation(Rectangle.HeightProperty, growAnimation);



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
        private void SaveGreenhouse()
        {
            var plantDataList = new List<PlantData>();

            foreach (var plant in _plants)
            {
                plantDataList.Add(new PlantData
                {
                    Name = plant.Name,
                    Section = plant.Section,
                    WaterLevel = plant.WaterLevel,
                    Status = plant.Status  // ➡ додали
                });
            }

            string json = JsonSerializer.Serialize(plantDataList, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText("greenhouse.json", json);

            MessageBox.Show("Теплицю збережено!", "Інформація");
        }

        private void LoadGreenhouse()
        {
            if (!File.Exists("greenhouse.json"))
            {
                MessageBox.Show("Файл з теплицею не знайдено!", "Помилка");
                return;
            }

            string json = File.ReadAllText("greenhouse.json");
            var plantDataList = JsonSerializer.Deserialize<List<PlantData>>(json);

            // Очищаємо старі культури
            foreach (var plant in _plants.ToArray())
            {
                RemovePlantFromMap(plant);
                _plants.Remove(plant);
            }

            // Відновлюємо культури
            foreach (var data in plantDataList)
            {
                Plant plant = PlantFactory.CreatePlant(data.Name, data.Section);
                plant.WaterLevel = data.WaterLevel;
                plant.SetStatus(data.Status);

                // Далі додаємо їх на карту так само, як у AddPlant_Click
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

                // Оновлюємо візуальний вигляд без змін температури
                plant.UpdateStatus(
                    _sectionTemperatures[plant.Section],
                    _sectionHumidities[plant.Section],
                    _sectionLights[plant.Section]);

                _plants.Add(plant);
            }

            UpdateStatusDisplay();
            MessageBox.Show("Теплицю завантажено!", "Інформація");
        }
        private void SaveGreenhouse_Click(object sender, RoutedEventArgs e)
        {
            SaveGreenhouse();
        }

        private void LoadGreenhouse_Click(object sender, RoutedEventArgs e)
        {
            LoadGreenhouse();
        }

        private void ExitApp(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
