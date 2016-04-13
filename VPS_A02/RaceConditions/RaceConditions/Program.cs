using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaceConditions
{
    class Program
    {
        static void Main(string[] args)
        {
            //var example = new RaceConditionExample();
            //example.Run();

            var example = new FixedRaceConditionExample();
            example.Run();

            Console.ReadKey();
        }
    }
}
