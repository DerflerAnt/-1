using System.Windows.Media;

namespace SmartGreenhouseSimulator.Models
{
    public class Lemon : Plant
    {
        public Lemon(int section)
        {
            Name = "Лимон";
            RequiredTemperature = 28;
            RequiredHumidity = 65;
            RequiredLight = 550;
            Section = section;
        }

        
    }
}