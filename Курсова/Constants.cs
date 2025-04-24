using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Курсова
{
    public static class Constants
    {
        public static readonly Dictionary<string, (int Temperature, int Humidity, int Light)> OptimalConditions =
            new Dictionary<string, (int Temperature, int Humidity, int Light)>
            {
            { "Огірки", (22, 60, 500) },
            { "Помідори", (25, 70, 600) },
            { "Банани", (20, 50, 400) },
            { "Яблука", (30, 80, 700) },
            { "Лимон", (28, 65, 550) }
            };
    }
}
