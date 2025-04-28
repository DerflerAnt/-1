using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartGreenhouseSimulator.Models
{
    public class PlantData
    {
        public string Name { get; set; }
        public int Section { get; set; }
        public int WaterLevel { get; set; }
        public string Status { get; set; } // ➡ Додали статус культури
    }
}


