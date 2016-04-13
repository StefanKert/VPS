using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RaceConditions
{
    class FixedRaceConditionExample
    {
        private const int N = 1000;
        private const int BUFFER_SIZE = 10;
        private double[] buffer;
        private AutoResetEvent writerSignal;
        private AutoResetEvent readerSignal;

        public void Run()
        {
            buffer = new double[BUFFER_SIZE];
            writerSignal = new AutoResetEvent(false);
            readerSignal = new AutoResetEvent(true);
            // start threads
            var t1 = new Thread(Reader);
            var t2 = new Thread(Writer);
            t1.Start();
            t2.Start();
            // wait
            t1.Join();
            t2.Join();
        }
        void Reader()
        {
            var readerIndex = 0;
            for (int i = 0; i < N; i++)
            {
                readerSignal.Set();
                writerSignal.WaitOne();
                Console.WriteLine(buffer[readerIndex]);
                readerIndex = (readerIndex + 1) % BUFFER_SIZE;
            }
        }
        void Writer()
        {
            var writerIndex = 0;
            for (int i = 0; i < N; i++)
            {
                readerSignal.WaitOne();
                buffer[writerIndex] = (double)i;
                writerIndex = (writerIndex + 1) % BUFFER_SIZE;
                writerSignal.Set();
            }
        }
    }
}
