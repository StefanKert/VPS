using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RaceConditionExample
{
    class RaceConditionExample
    {
        private static readonly object LockObject = new object();
        static int result = 0;

        public static void IncreaseResult()
        {
            result++;
        }

        public static void IncreaseResultWithLock()
        {
            lock (LockObject)
            {
                result++;
            }
        }

        public static void Run(int numberOfIncrements, int threadCount, Action method)
        {
            var tasks = new Task[threadCount];
            var raceConditionCount = 0;
            result = 0;
            for (int i = 0; i < numberOfIncrements; i++)
            {
                for (int j = 0; j < threadCount; j++)
                {
                    tasks[j] = new Task(method);
                    tasks[j].Start();
                }

                Task.WaitAll(tasks);

                if (result != i * threadCount)
                    raceConditionCount++;
            }

            PrintResult(numberOfIncrements, threadCount, raceConditionCount);
        }

        private static void PrintResult(int numberOfIncrements, int threadCount, int raceConditionCount)
        {
            if (result != threadCount * numberOfIncrements)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("----------------------");
                Console.Error.WriteLine($"Race Conditions occurred ({raceConditionCount})");
                Console.WriteLine("----------------------");
                Console.ForegroundColor = ConsoleColor.White;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine("----------------------");
                Console.Error.WriteLine($"Successful");
                Console.WriteLine("----------------------");
                Console.ForegroundColor = ConsoleColor.White;
            }
        }
    }
}