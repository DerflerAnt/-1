using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Controls;

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
        public string Status { get; protected set; }

        private Rectangle _visualRect;
        private ProgressBar _visualWater;

        public void BindVisual(Rectangle rect, ProgressBar water)
        {
            _visualRect = rect;
            _visualWater = water;
            UpdateVisual();
        }

        public abstract Brush GetColorByStatus();

        public virtual void UpdateStatus(int temp, int humidity, int light)
        {
            bool isTempOk = temp >= RequiredTemperature - 5 && temp <= RequiredTemperature + 5;
            bool isHumidityOk = humidity >= RequiredHumidity - 10 && humidity <= RequiredHumidity + 10;
            bool isLightOk = light >= RequiredLight - 100 && light <= RequiredLight + 100;
            bool isWaterLevelOk = WaterLevel >= 30;

            Status = (isTempOk && isHumidityOk && isLightOk && isWaterLevelOk)
                ? "Гарний стан" : "Поганий стан";

            UpdateVisual();
        }

        private void UpdateVisual()
        {
            if (_visualRect != null)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    var colorAnimation = new System.Windows.Media.Animation.ColorAnimation
                    {
                        To = ((SolidColorBrush)GetColorByStatus()).Color,
                        Duration = TimeSpan.FromMilliseconds(500)
                    };

                    var currentBrush = _visualRect.Fill as SolidColorBrush;
                    if (currentBrush == null || currentBrush.IsFrozen)
                    {
                        currentBrush = new SolidColorBrush(((SolidColorBrush)GetColorByStatus()).Color);
                        _visualRect.Fill = currentBrush;
                    }

                    currentBrush.BeginAnimation(SolidColorBrush.ColorProperty, colorAnimation);

                    if (_visualWater != null)
                    {
                        _visualWater.Value = WaterLevel;
                    }
                });
            }
        }
    }
}
