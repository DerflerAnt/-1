using System.Windows.Media;

namespace SmartGreenhouseSimulator.Models
{
    public class Tomato : Plant
    {
        public Tomato(int section)
        {
            Name = "Помідори";
            RequiredTemperature = 25;
            RequiredHumidity = 70;
            RequiredLight = 600;
            Section = section;
        }

        public override Brush GetColorByStatus()
        {
            return Status == "Гарний стан" ? Brushes.Green : Brushes.Red;
        }
    }
}