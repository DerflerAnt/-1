using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SmartGreenhouseSimulator.Models
{
    public class Greenhouse
    {
        public ObservableCollection<Plant> Plants { get; set; } = new ObservableCollection<Plant>();
        public Plant[,] CropField { get; private set; } // Карта посівів
        public bool IsHeatingOn { get; set; } // Стан опалення
        public bool IsVentilationOn { get; set; } // Стан вентиляції
        private int fieldWidth = 3; // Максимум 3 культури на ряд
        private int fieldHeight = 2; // Максимум 2 ряди

        public Greenhouse()
        {
            CropField = new Plant[fieldWidth, fieldHeight];
        }

        public void AddPlant(Plant plant)
        {
            Plants.Add(plant);
        }

        public void RemovePlant(Plant plant)
        {
            Plants.Remove(plant);
        }

        public bool PlantInField(Plant plant, int x, int y)
        {
            if (x >= 0 && x < fieldWidth && y >= 0 && y < fieldHeight && CropField[x, y] == null)
            {
                CropField[x, y] = plant;
                return true;
            }
            return false;
        }

        public void ApplyHeating(ref int[] temperatures)
        {
            for (int i = 0; i < temperatures.Length; i++)
                temperatures[i] += 2;
        }

        public void ApplyVentilation(ref int[] temperatures)
        {
            for (int i = 0; i < temperatures.Length; i++)
                temperatures[i] -= 2;
        }

        
        public string GetStatus()
        {
            string status = "Статус теплиці:\n";
            status += $"Опалення: {(IsHeatingOn ? "Увімкнено" : "Вимкнено")}, Вентиляція: {(IsVentilationOn ? "Увімкнено" : "Вимкнено")}\n";
            if (Plants.Count == 0)
                status += "У теплиці немає культур.\n";
            else
            {
                status += "Культури:\n";
                foreach (var plant in Plants)
                {
                    status += $"- {plant}\n";
                }
            }
            return status;
        }
        
    }
}
