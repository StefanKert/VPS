using System;
using System.Linq;
using System.Threading;

namespace VSS.ToiletSimulation
{

    public class ToiletQueue : FIFOQueue
    {
        protected override IJob DequeueNextJob()
        {
            lock (_queue)
            {
                IJob result = _queue.OrderBy(x => x.DueDate).FirstOrDefault(x => x.DueDate > DateTime.Now) ?? _queue.First();
                _queue.Remove(result);
                return result;
            }
        }
    }
}