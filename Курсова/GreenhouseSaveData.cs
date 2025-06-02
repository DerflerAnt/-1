using System.Collections.Generic;

namespace SmartGreenhouseSimulator.Models
{
    public class GreenhouseSaveData
    {
        public List<PlantData> Plants { get; set; }
        public int[] Temperatures { get; set; }
        public int[] Humidities { get; set; }
        public int[] Lights { get; set; }
        public bool IsHeatingOn { get; set; }
        public bool IsVentilationOn { get; set; }
        public int HarvestedCount { get; set; }
        public int RemovedCount { get; set; }
        public int AddedCount { get; set; }
    }
}
