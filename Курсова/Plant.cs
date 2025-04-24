using System;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace SmartGreenhouseSimulator.Models
{
    public class Plant
    {
        public string Name { get; set; }
        public Brush Color { get; set; }
        public int RequiredTemperature { get; set; }
        public int RequiredHumidity { get; set; }
        public int RequiredLight { get; set; }
        public int WaterLevel { get; set; }
        public string Status { get; private set; }
        public int Section { get; set; }

        public Rectangle TargetRectangle { get; set; }

        private Timer _growthTimer;
        private Action<Brush, Rectangle> _animatedUpdate;

        public Plant(string name, Brush color, int temp, int humidity, int light, int section, Action<Brush, Rectangle> animatedUpdate)
        {
            Name = name;
            Color = color;
            RequiredTemperature = temp;
            RequiredHumidity = humidity;
            RequiredLight = light;
            WaterLevel = 50;
            Section = section;
            _animatedUpdate = animatedUpdate;

            UpdateStatus(temp, humidity, light);
            StartGrowthTimer();
        }

        public void UpdateStatus(int currentTemp, int currentHumidity, int currentLight)
        {
            bool isTemperatureOk = currentTemp >= RequiredTemperature - 5 && currentTemp <= RequiredTemperature + 5;
            bool isHumidityOk = currentHumidity >= RequiredHumidity - 10 && currentHumidity <= RequiredHumidity + 10;
            bool isLightOk = currentLight >= RequiredLight - 100 && currentLight <= RequiredLight + 100;
            bool isWaterLevelOk = WaterLevel >= 30;

            if (!isTemperatureOk || !isHumidityOk || !isLightOk || !isWaterLevelOk)
            {
                Status = "Поганий стан";
            }
            else
            {
                Status = "Гарний стан";
            }

            Brush targetColor = GetColorByStatus();
            if (TargetRectangle != null)
                _animatedUpdate?.Invoke(targetColor, TargetRectangle);
        }

        private Brush GetColorByStatus()
        {
            if (Status == "Гарний стан")
                return Brushes.Green;
            if (Status == "Поганий стан")
                return Brushes.Red;
            return Brushes.Yellow;
        }

        private void StartGrowthTimer()
        {
            _growthTimer = new Timer(_ =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    if (TargetRectangle != null)
                    {
                        Brush targetColor = GetColorByStatus();
                        _animatedUpdate?.Invoke(targetColor, TargetRectangle);
                    }
                });
            }, null, 5000, Timeout.Infinite);
        }
    }
}
