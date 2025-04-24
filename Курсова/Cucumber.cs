using System.Windows.Media;

namespace SmartGreenhouseSimulator.Models
{
    public class Cucumber : Plant
    {
        public Cucumber(int section)
        {
            Name = "Огірки";
            RequiredTemperature = 22;
            RequiredHumidity = 60;
            RequiredLight = 500;
            Section = section;
        }

        public override Brush GetColorByStatus()
        {
            return Status == "Гарний стан" ? Brushes.Green : Brushes.Red;
        }
    }
}