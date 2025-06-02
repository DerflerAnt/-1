using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Threading;
using System.ComponentModel;
using System.ComponentModel; // 🔔
namespace SmartGreenhouseSimulator.Models
{
    public abstract class Plant : INotifyPropertyChanged
    {

        public string Name { get; protected set; }
        public int RequiredTemperature { get; protected set; }
        public int RequiredHumidity { get; protected set; }
        public int RequiredLight { get; protected set; }
        public int WaterLevel { get; set; } = 50;
        public int Section { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        private Timer _waterLossTimer;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private string _status = "Росте";
        public string Status
        {
            get => _status;
            protected set
            {
                if (_status != value)
                {
                    _status = value;
                    OnPropertyChanged(nameof(Status)); // 🔔 оновлює таблицю
                    UpdateVisual(); // оновлення візуалу
                }
            }
        }

        // 🔧 Додано для добрив
        public bool IsFertilized { get; set; }
        private DateTime fertilizerEndTime;

        private Rectangle _visualRect;
        private ProgressBar _visualWater;
        private Timer _growthTimer;

        private int _currentTemperature;
        private int _currentHumidity;
        private int _currentLight;
        private ProgressBar _fertilizerBar;
        public void BindFertilizerBar(ProgressBar bar)
        {
            _fertilizerBar = bar;
        }

        public void BindVisual(Rectangle rect, ProgressBar water)
        {
            _visualRect = rect;
            _visualWater = water;
            UpdateVisual();

            // 🔄 Запускаємо таймер втрати води кожні 8 секунд
            _waterLossTimer = new Timer(_ =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    WaterLevel -= 5;
                    if (WaterLevel < 0) WaterLevel = 0;

                    UpdateStatus(_currentTemperature, _currentHumidity, _currentLight);
                    ForceVisualUpdate(); // оновлюємо прогресбар вручну
                });
            }, null, 8000, 8000); // перше спрацювання через 8 с, потім кожні 8 с
        }


        public abstract Brush GetColorByStatus();

        public virtual void UpdateStatus(int currentTemp, int currentHumidity, int currentLight)
        {
            _currentTemperature = currentTemp;
            _currentHumidity = currentHumidity;
            _currentLight = currentLight;

            // ⏳ Оновлюємо стан добрива
            if (IsFertilized && DateTime.Now > fertilizerEndTime)
            {
                IsFertilized = false;
                UpdateVisual(); // 🔔 Тепер візуально оновлюється
            }

            if (Status == "Росте")
            {
                if (_growthTimer == null)
                {
                    _growthTimer = new Timer(_ =>
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            CheckConditions();
                        });
                    }, null, 5000, Timeout.Infinite);
                }
            }
            else
            {
                CheckConditions();
            }
        }


        private void CheckConditions()
        {
            int tempTolerance = IsFertilized ? 8 : 5;
            int humidityTolerance = IsFertilized ? 15 : 10;
            int lightTolerance = IsFertilized ? 150 : 100;

            bool isTempOk = _currentTemperature >= RequiredTemperature - tempTolerance &&
                            _currentTemperature <= RequiredTemperature + tempTolerance;

            bool isHumidityOk = _currentHumidity >= RequiredHumidity - humidityTolerance &&
                                _currentHumidity <= RequiredHumidity + humidityTolerance;

            bool isLightOk = _currentLight >= RequiredLight - lightTolerance &&
                             _currentLight <= RequiredLight + lightTolerance;

            bool isWaterOk = WaterLevel >= 30;

            Status = (isTempOk && isHumidityOk && isLightOk && isWaterOk)
                ? "Гарний стан"
                : "Поганий стан";
        }


        public void SetStatus(string status)
        {
            Status = status;
        }

        // 🧪 ДОБРИВО — запускаємо ефект
        public void ApplyFertilizer(int seconds)
        {
            IsFertilized = true;
            fertilizerEndTime = DateTime.Now.AddSeconds(seconds);
            UpdateVisual();

            if (_fertilizerBar != null)
            {
                int elapsed = 0;
                var timer = new System.Windows.Threading.DispatcherTimer();
                timer.Interval = TimeSpan.FromSeconds(1);
                timer.Tick += (s, e) =>
                {
                    elapsed++;
                    _fertilizerBar.Value = elapsed;

                    if (elapsed >= seconds)
                    {
                        timer.Stop();
                        IsFertilized = false;

                        // Перевіряємо умови заново та оновлюємо колір
                        UpdateStatus(_currentTemperature, _currentHumidity, _currentLight);
                    }
                };
                timer.Start();
            }
            else
            {
                // Якщо нема індикатора, все одно запускаємо завершення дії
                var fallbackTimer = new System.Windows.Threading.DispatcherTimer();
                fallbackTimer.Interval = TimeSpan.FromSeconds(seconds);
                fallbackTimer.Tick += (s, e) =>
                {
                    fallbackTimer.Stop();
                    IsFertilized = false;
                    UpdateStatus(_currentTemperature, _currentHumidity, _currentLight);
                };
                fallbackTimer.Start();
            }
        }
        public void ForceVisualUpdate()
        {
            if (_visualWater != null)
            {
                _visualWater.Value = WaterLevel;
            }
        }
        public void Dispose()
        {
            _growthTimer?.Dispose();
            _waterLossTimer?.Dispose();
        }



        private void UpdateVisual()
        {
            if (_visualRect != null)
            {
                Brush brush;

                // 🔮 Якщо удобрюється — колір фіолетовий
                if (IsFertilized)
                    brush = Brushes.MediumPurple;
                else if (Status == "Росте")
                    brush = Brushes.Yellow;
                else
                    brush = GetColorByStatus();

                var colorAnimation = new System.Windows.Media.Animation.ColorAnimation
                {
                    To = ((SolidColorBrush)brush).Color,
                    Duration = TimeSpan.FromMilliseconds(500)
                };

                var currentBrush = _visualRect.Fill as SolidColorBrush;
                if (currentBrush == null || currentBrush.IsFrozen)
                {
                    currentBrush = new SolidColorBrush(((SolidColorBrush)brush).Color);
                    _visualRect.Fill = currentBrush;
                }

                currentBrush.BeginAnimation(SolidColorBrush.ColorProperty, colorAnimation);

                if (_visualWater != null)
                {
                    _visualWater.Value = WaterLevel;
                }
            }
        }
    }
}
