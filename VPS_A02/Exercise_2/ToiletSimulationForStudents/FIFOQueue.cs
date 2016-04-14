using System;
using System.Linq;
using System.Threading;

namespace VSS.ToiletSimulation
{

    public class FIFOQueue : Queue
    {
        private readonly SemaphoreSlim _syncSem;

        public FIFOQueue()
        {
            _syncSem = new SemaphoreSlim(0);
        }

        public override void Enqueue(IJob job)
        {
            if (IsCompleted)
                throw new InvalidOperationException("The queue already is completed.");

            lock (_queue)
            {
                _queue.Add(job);
            }
            _syncSem.Release();
        }

        public override bool TryDequeue(out IJob job)
        {

            job = null;
            if (IsCompleted)
                return false;

            _syncSem.Wait();

            lock (_queue)
            {
                if (Count == 0)
                    return false;

                job = DequeueNextJob();
                return true;
            }
        }

        protected virtual IJob DequeueNextJob()
        {
            lock (_queue)
            {
                var job = _queue.First();
                _queue.Remove(job);
                return job;
            }
        }
    }
}