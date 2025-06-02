using System;

namespace SmartGreenhouseSimulator.Models
{
    public static class PlantFactory
    {
        public static Plant CreatePlant(string name, int section)
        {
            if (name == "Помідори") return new Tomato(section);
            if (name == "Огірки") return new Cucumber(section);
            if (name == "Банани") return new Banana(section);
            if (name == "Яблука") return new Apple(section);
            if (name == "Лимон") return new Lemon(section);
            throw new ArgumentException("Невідома культура");
        }

    }
}