using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SynchronizationPrimitives
{
    public class LimitedConnectionsExample
    {
        private const int ConcurrentDownloads = 10;
        private SemaphoreSlim _syncSemaphore;
        private List<Thread> _threads;

        public void DownloadFilesAsync(IEnumerable<string> urls)
        {
            _syncSemaphore = new SemaphoreSlim(ConcurrentDownloads);
            _threads = new List<Thread>();
            foreach (var url in urls)
            {
                Thread t = new Thread(DownloadFile);
                _threads.Add(t);
                t.Start(url);
            }
        }

        public void DownloadFile(object url)
        {
            _syncSemaphore.Wait();
            Console.WriteLine($"Downloading {url}");
            Thread.Sleep(1000);
            Console.WriteLine($"finished {url}");
            _syncSemaphore.Release();
        }

        public void DownloadFiles(IEnumerable<string> urls)
        {
            DownloadFilesAsync(urls);
            foreach (var thread in _threads)
            {
                thread.Join();
            }
            _syncSemaphore.Dispose();
        }
    }
}
