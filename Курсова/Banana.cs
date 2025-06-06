﻿using System.Windows.Media;

namespace SmartGreenhouseSimulator.Models
{
    public class Banana : Plant
    {
        public Banana(int section)
        {
            Name = "Банани";
            RequiredTemperature = 20;
            RequiredHumidity = 50;
            RequiredLight = 400;
            Section = section;
        }

        public override Brush GetColorByStatus()
        {
            return Status == "Гарний стан" ? Brushes.Green : Brushes.Red;
        }
    }
}