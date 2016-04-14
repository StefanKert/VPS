using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaceConditionExample
{
    class Program
    {
        private const int NumberOfIncrements = 10000;
        private const int ThreadCount = 5;
        private const int Runs = 5;
        static void Main(string[] args)
        {
            MultipleRuns(Runs, NumberOfIncrements, ThreadCount, RaceConditionExample.IncreaseResult);
            MultipleRuns(Runs, NumberOfIncrements, ThreadCount, RaceConditionExample.IncreaseResultWithLock);
            Console.ReadLine();
        }

        public static void MultipleRuns(int runs, int numberOfIncrements, int threadCount, Action method)
        {
            for (int i = 1; i <= runs; i++)
            {
                Console.WriteLine($"{i}. Run");
                RaceConditionExample.Run(numberOfIncrements, threadCount, method);
                Console.WriteLine();
            }
        }

    }
}
