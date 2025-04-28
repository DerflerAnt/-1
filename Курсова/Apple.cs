using System.Windows.Media;

namespace SmartGreenhouseSimulator.Models
{
    public class Apple : Plant
    {
        public Apple(int section)
        {
            Name = "Яблука";
            RequiredTemperature = 30;
            RequiredHumidity = 80;
            RequiredLight = 700;
            Section = section;
        }

        
    }
}
