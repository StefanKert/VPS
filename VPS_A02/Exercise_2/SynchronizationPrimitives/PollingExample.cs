using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SynchronizationPrimitives
{
    class PollingExample
    {
        private const int MAX_RESULTS = 10;
        private volatile string[] results;
        private volatile int resultsFinished;
        private object resultsLocker = new object();

        public void Run() {
            results = new string[MAX_RESULTS];
            resultsFinished = 0;
            // start tasks
            for (int i = 0; i < MAX_RESULTS; i++) {
                var t = new Task((s) => {
                    int _i = (int)s;
                    string m = Magic(_i);
                    results[_i] = m;
                    lock (resultsLocker) {
                        resultsFinished++;
                    }
                }, i);
                t.Start();
            }

            // wait for results
            while (resultsFinished < MAX_RESULTS) {
                Thread.Sleep(10);
            }

            // output results 
            for (int i = 0; i < MAX_RESULTS; i++)
                Console.WriteLine(results[i]);
        }

        private string Magic(int i) {
            return i.ToString();
        }
    }
}