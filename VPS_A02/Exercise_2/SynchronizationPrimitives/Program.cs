using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SynchronizationPrimitives
{
    class Program
    {
        static void Main(string[] args)
        {
            var test = new LimitedConnectionsExample();
            test.DownloadFilesAsync(new List<string>
            {
                "Test",
                "Test",
                "Test",
                "Test",
                "Test",
                "Test",
                "Test",
                "Test",
                "Test",
                "Test",
                "Test",
                "Test"
            });

            Thread.Sleep(10000);
            Console.ReadKey();
        }
    }
}
