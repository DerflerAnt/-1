using SmartGreenhouseSimulator.Models;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.IO;
using System.Text.Json;
using System.Collections.ObjectModel;
using System.Numerics;
using System.Linq;


namespace SmartGreenhouseSimulator
{
    public partial class MainWindow : Window
    {
        private List<Plant> _plants = new List<Plant>();

        private int _harvestedCount = 0;
        private int _diedCount = 0;
        private Dictionary<string, int> _harvestedByType = new Dictionary<string, int>();
        private ObservableCollection<Plant> plantList = new ObservableCollection<Plant>();
        private Greenhouse greenhouse = new Greenhouse();

        private int currentTemp = 25;
        private int currentHumidity = 60;
        private int currentLight = 500;
        private Greenhouse _greenhouse = new Greenhouse();

        private int[] _sectionTemperatures = { 22, 25, 20, 30 };
        private int[] _sectionHumidities = { 60, 70, 50, 80 };
        private int[] _sectionLights = { 500, 600, 400, 700 };
        private System.Timers.Timer _fertilizationTimer;
        private bool[] _isFertilizing = new bool[4]; // по одній на секцію
        private Brush[] _originalSectionColors = new Brush[4];
        private int _removedCount = 0;
        private int _addedCount = 0;

        public MainWindow()
        {
            InitializeComponent();
            _originalSectionColors[0] = Section1.Background;
            _originalSectionColors[1] = Section2.Background;
            _originalSectionColors[2] = Section3.Background;
            _originalSectionColors[3] = Section4.Background;
            UpdateStatusDisplay();
            StartFertilizationTimer();
            plantDataGrid.ItemsSource = greenhouse.Plants;          
            plantDataGrid.ItemsSource = plantList;

        }
        private void ApplyHeating_Click(object sender, RoutedEventArgs e)
        {
            _greenhouse.ApplyHeating(ref _sectionTemperatures);
            UpdatePlantStatuses(); // щоб рослини оновилися
            UpdateStatusDisplay();
            MessageBox.Show("Температуру підвищено на 2°C у всіх секціях.", "Опалення");
        }

        private void ApplyVentilation_Click(object sender, RoutedEventArgs e)
        {
            _greenhouse.ApplyVentilation(ref _sectionTemperatures);
            UpdatePlantStatuses();
            UpdateStatusDisplay();
            MessageBox.Show("Температуру знижено на 2°C у всіх секціях.", "Вентиляція");
        }


        private void ShowStatistics_Click(object sender, RoutedEventArgs e)
        {
            string message = "Статистика теплиці:\n" +
                $"Зібрано культур: {_harvestedCount}\n" +
                $"Загинуло культур: {_diedCount}\n" +
                $"Наразі в теплиці: {_plants.Count}\n\n" +
                "Зібрано по культурах:\n";

            foreach (var entry in _harvestedByType)
            {
                message += $"{entry.Key}: {entry.Value}\n";
            }

            MessageBox.Show(message, "Статистика");
        }
        private void StartFertilizationTimer()
        {
            _fertilizationTimer = new System.Timers.Timer(30000); // кожні 30 секунд
            _fertilizationTimer.Elapsed += (s, e) =>
            {
                Dispatcher.Invoke(() => StartFertilization());
            };
            _fertilizationTimer.Start();
        }

        private void StartFertilization()
        {
            List<int> emptySections = new List<int>();

            for (int i = 0; i < 4; i++)
            {
                if (!_isFertilizing[i] && IsSectionEmpty(i))
                    emptySections.Add(i);
            }

            if (emptySections.Count == 0) return;

            Random rnd = new Random();
            int sectionIndex = emptySections[rnd.Next(emptySections.Count)];

            _isFertilizing[sectionIndex] = true;
            var canvas = GetSectionCanvas(sectionIndex);

            // 🔄 Змінюємо фон 
            canvas.Background = new SolidColorBrush(Colors.MediumPurple);

            // 🎯 Створюємо іконку удобрення
            TextBlock icon = new TextBlock
            {
                Text = "⚗️",
                FontSize = 60,
                Foreground = Brushes.Black,
                FontWeight = FontWeights.Bold
            };
            Canvas.SetLeft(icon, 110);
            Canvas.SetTop(icon, 56);
            canvas.Children.Add(icon);

            // 📝 Статус
            GreenhouseStatus.Text += $"\n⚠️ Секція {sectionIndex + 1} удобрюється 10 секунд.";
           // MessageBox.Show($"Секція {sectionIndex + 1} автоматично удобрюється. Не можна додавати культури 10 секунд.", "Удобрення");

            // ✅ Спільний таймер
            var timer = new System.Timers.Timer(10000);
            timer.Elapsed += (s, e) =>
            {
                Dispatcher.Invoke(() =>
                {
                    _isFertilizing[sectionIndex] = false;

                    // ❌ Повертаємо фон
                    canvas.Background = _originalSectionColors[sectionIndex];

                    // ❌ Видаляємо іконку
                    canvas.Children.Remove(icon);

                    UpdateStatusDisplay();
                   // MessageBox.Show($"Секція {sectionIndex + 1} знову доступна для посадки.", "Удобрення завершено");
                });
                timer.Stop();
                timer.Dispose();
            };
            timer.Start();
        }




        private void UpdateStatusDisplay()
        {
            GreenhouseStatus.Text = "Статус теплиці:\n" +
                $"Секція 1: {_sectionTemperatures[0]}°C, {_sectionHumidities[0]}%, {_sectionLights[0]} люменів\n" +
                $"Секція 2: {_sectionTemperatures[1]}°C, {_sectionHumidities[1]}%, {_sectionLights[1]} люменів\n" +
                $"Секція 3: {_sectionTemperatures[2]}°C, {_sectionHumidities[2]}%, {_sectionLights[2]} люменів\n" +
                $"Секція 4: {_sectionTemperatures[3]}°C, {_sectionHumidities[3]}%, {_sectionLights[3]} люменів\n";

            // Якщо якась секція удобрюється — показати це
            for (int i = 0; i < 4; i++)
            {
                if (_isFertilizing[i])
                {
                    GreenhouseStatus.Text += $"\n⚠️ Секція {i + 1} УДОБРЮЄТЬСЯ!";
                }
            }

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
                if (_isFertilizing[plant.Section])
                {
                    MessageBox.Show($"Секція {plant.Section + 1} наразі удобрюється. Додавання культур тимчасово неможливе.", "Увага", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }
                Canvas sectionCanvas = GetSectionCanvas(plant.Section);
                if (sectionCanvas.Children.Count >= 6)
                {
                    MessageBox.Show("У цій секції вже 6 культур. Зачекайте, поки щось виросте або зберіть урожай.", "Обмеження", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                Grid plantContainer = new Grid { Width = 80, Height = 100, Tag = plant };
    

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
                    Text = GetEmojiForPlant(plant.Name) + " " + plant.Name,

                    Foreground = Brushes.Black,
                    FontSize = 14,
                    TextWrapping = TextWrapping.Wrap,
                    TextAlignment = TextAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(0, 0, 0, 20)
                };
                plantContainer.Children.Add(plantName);
                ProgressBar fertilizerBar = new ProgressBar
                {
                    Minimum = 0,
                    Maximum = 10, // 10 секунд дії
                    Value = 0,
                    Height = 6,
                    VerticalAlignment = VerticalAlignment.Top,
                    Margin = new Thickness(5, 5, 5, 0),
                    Foreground = Brushes.MediumPurple
                };
                plantContainer.Children.Add(fertilizerBar);

                // зв'язати з рослиною (додати у Plant.cs поле для нього)
                plant.BindFertilizerBar(fertilizerBar);

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
                plantList.Add(plant);

                UpdateStatusDisplay();
            }
        }
        private string GetEmojiForPlant(string name)
        {
            switch (name)
            {
                case "Огірки": return "🥒";
                case "Помідори": return "🍅";
                case "Банани": return "🍌";
                case "Яблука": return "🍎";
                case "Лимон": return "🍋";
                default: return "🌱";
            }
        }
        private bool IsSectionEmpty(int sectionIndex)
        {
            var canvas = GetSectionCanvas(sectionIndex);
            return canvas.Children.Count == 0;
        }




        private void FertilizePlant_Click(object sender, RoutedEventArgs e)
        {
            if (plantDataGrid.SelectedItem is Plant selectedPlant)
            {
                selectedPlant.ApplyFertilizer(10); // добриво діє 10 секунд

                // Оновити відображення одразу
                selectedPlant.UpdateStatus(currentTemp, currentHumidity, currentLight);

                // Запускаємо таймер на завершення ефекту добрива
                var timer = new System.Windows.Threading.DispatcherTimer();
                timer.Interval = TimeSpan.FromSeconds(10); // тривалість добрива
                timer.Tick += (s, args) =>
                {
                    timer.Stop();
                    selectedPlant.UpdateStatus(currentTemp, currentHumidity, currentLight); // повторно оновити
                };
                timer.Start();
            }
            else
            {
                MessageBox.Show("Будь ласка, виберіть рослину зі списку.");
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

                // 🔄 Примусово оновити прогресбар
                plant.ForceVisualUpdate();
            }

            UpdateStatusDisplay();
            MessageBox.Show("Всі культури було полито.", "Інформація");
        }


        private void HarvestPlant_Click(object sender, RoutedEventArgs e)
        {
            // Збираємо всі рослини в гарному стані
            var harvestedPlants = _plants.Where(p => p.Status == "Гарний стан").ToList();

            if (harvestedPlants.Count == 0)
            {
                MessageBox.Show("Немає культур, готових до збору.", "Інформація");
                return;
            }

            foreach (var plant in harvestedPlants)
            {
                // Підрахунок по типах
                if (_harvestedByType.ContainsKey(plant.Name))
                    _harvestedByType[plant.Name]++;
                else
                    _harvestedByType[plant.Name] = 1;

                RemovePlantFromMap(plant);

                // Видалення з ObservableCollection
                plant.Dispose(); // перед _plants.Remove(plant);
                _plants.Remove(plant);
                plantList.Remove(plant);
                _harvestedCount++;
            }

            UpdateStatusDisplay();
            MessageBox.Show($"Зібрано {harvestedPlants.Count} культур.", "Успіх");
        }


        private void RemovePlantFromMap(Plant plant)
        {
            Canvas sectionCanvas = GetSectionCanvas(plant.Section);

            foreach (UIElement element in sectionCanvas.Children)
            {
                if (element is Grid grid && grid.Tag is Plant linkedPlant && linkedPlant == plant)
                {
                    sectionCanvas.Children.Remove(grid);
                    break;
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
                plant.Dispose(); // перед _plants.Remove(plant);
                _plants.Remove(plant);
                plantList.Remove(plant);
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
                    Status = plant.Status
                });
            }

            var saveData = new GreenhouseSaveData
            {
                Plants = plantDataList,
                Temperatures = _sectionTemperatures,
                Humidities = _sectionHumidities,
                Lights = _sectionLights,
                IsHeatingOn = _greenhouse.IsHeatingOn,
                IsVentilationOn = _greenhouse.IsVentilationOn,

                // ✅ Статистика
                HarvestedCount = _harvestedCount,
                RemovedCount = _removedCount,
                AddedCount = _addedCount
            };

            string json = JsonSerializer.Serialize(saveData, new JsonSerializerOptions { WriteIndented = true });
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
            var saveData = JsonSerializer.Deserialize<GreenhouseSaveData>(json);

            // Очистити старі рослини
            foreach (var plant in _plants.ToArray())
            {
                RemovePlantFromMap(plant);
                _plants.Remove(plant);
            }

            // Відновити параметри середовища
            _sectionTemperatures = saveData.Temperatures;
            _sectionHumidities = saveData.Humidities;
            _sectionLights = saveData.Lights;
            _greenhouse.IsHeatingOn = saveData.IsHeatingOn;
            _greenhouse.IsVentilationOn = saveData.IsVentilationOn;

            // ✅ Відновити статистику
            _harvestedCount = saveData.HarvestedCount;
            _removedCount = saveData.RemovedCount;
            _addedCount = saveData.AddedCount;

            // Відновлення культур
            foreach (var data in saveData.Plants)
            {
                Plant plant = PlantFactory.CreatePlant(data.Name, data.Section);
                plant.WaterLevel = data.WaterLevel;
                plant.SetStatus(data.Status);

                Canvas sectionCanvas = GetSectionCanvas(plant.Section);

                Grid plantContainer = new Grid { Width = 80, Height = 100, Tag = plant };

                Rectangle fertilizingBar = new Rectangle
                {
                    Fill = Brushes.LightGreen,
                    Height = 5,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Top
                };
                plantContainer.Children.Add(fertilizingBar);

                Rectangle plantField = new Rectangle
                {
                    Fill = plant.GetColorByStatus(),
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

                plant.BindVisual(plantField, waterBar);

                plant.UpdateStatus(
                    _sectionTemperatures[plant.Section],
                    _sectionHumidities[plant.Section],
                    _sectionLights[plant.Section]);

                int row = sectionCanvas.Children.Count / 3;
                int col = sectionCanvas.Children.Count % 3;
                Canvas.SetLeft(plantContainer, col * 105 + 10);
                Canvas.SetTop(plantContainer, row * 115 + 10);

                sectionCanvas.Children.Add(plantContainer);
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
