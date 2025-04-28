using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Threading;

namespace SmartGreenhouseSimulator.Models
{
    public abstract class Plant
    {
        public string Name { get; protected set; }
        public int RequiredTemperature { get; protected set; }
        public int RequiredHumidity { get; protected set; }
        public int RequiredLight { get; protected set; }
        public int WaterLevel { get; set; } = 50;
        public int Section { get; set; }
        public string Status { get; protected set; } = "Росте";

        private Rectangle _visualRect;
        private ProgressBar _visualWater;
        private Timer _growthTimer;

        // Останні параметри середовища
        private int _currentTemperature;
        private int _currentHumidity;
        private int _currentLight;

        public void BindVisual(Rectangle rect, ProgressBar water)
        {
            _visualRect = rect;
            _visualWater = water;
            UpdateVisual();
        }

        public abstract Brush GetColorByStatus();

        public virtual void UpdateStatus(int currentTemp, int currentHumidity, int currentLight)
        {
            _currentTemperature = currentTemp;
            _currentHumidity = currentHumidity;
            _currentLight = currentLight;

            if (Status == "Росте")
            {
                // Якщо культура ще росте — запускаємо таймер
                if (_growthTimer == null)
                {
                    _growthTimer = new Timer(_ =>
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            CheckConditions();
                        });
                    }, null, 5000, Timeout.Infinite); // Через 5 секунд
                }
            }
            else
            {
                // Якщо культура вже сформована — перевіряємо статус одразу
                CheckConditions();
            }
        }

        private void CheckConditions()
        {
            bool isTempOk = _currentTemperature >= RequiredTemperature - 5 && _currentTemperature <= RequiredTemperature + 5;
            bool isHumidityOk = _currentHumidity >= RequiredHumidity - 10 && _currentHumidity <= RequiredHumidity + 10;
            bool isLightOk = _currentLight >= RequiredLight - 100 && _currentLight <= RequiredLight + 100;
            bool isWaterOk = WaterLevel >= 30;

            Status = (isTempOk && isHumidityOk && isLightOk && isWaterOk)
                ? "Гарний стан"
                : "Поганий стан";

            UpdateVisual();
        }
        public void SetStatus(string status)
        {
            Status = status;
        }

        private void UpdateVisual()
        {
            if (_visualRect != null)
            {
                Brush brush = Status == "Росте" ? Brushes.Yellow : GetColorByStatus();

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
