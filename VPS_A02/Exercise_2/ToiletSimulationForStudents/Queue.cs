using System;
using System.Collections.Generic;
using System.Threading;

namespace VSS.ToiletSimulation
{
    public abstract class Queue : IQueue
    {
        private int countOfCompletedProducers;
        protected IList<IJob> _queue;

        public int Count
        {
            get
            {
                lock (_queue)
                {
                    return _queue.Count;
                }
            }
        }

        protected Queue()
        {
            _queue = new List<IJob>();
        }

        public abstract void Enqueue(IJob job);


        public abstract bool TryDequeue(out IJob job);


        public virtual void CompleteAdding()
        {
            lock (_queue)
            {
                countOfCompletedProducers++;
                if (countOfCompletedProducers == Parameters.Producers)
                    IsCompleted = true;
            }
        }

        public bool IsCompleted
        {
            get; private set;
        }
    }
}