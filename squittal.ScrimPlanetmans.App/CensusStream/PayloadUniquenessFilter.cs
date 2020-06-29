using System;
using System.Linq;
using System.Collections.Concurrent;
using squittal.ScrimPlanetmans.CensusStream.Models;

namespace squittal.ScrimPlanetmans.CensusStream
{
    //public class PayloadUniquenessFilter<IEquitablePayload> //<T> where T : IEquitablePayload<T>
    public class PayloadUniquenessFilter<T> where T : PayloadBase, IEquitablePayload<T>
    {
        private ConcurrentQueue<T> PayloadQueue { get; set; } = new ConcurrentQueue<T>();

        private int MaxQueueItems { get; set; } = 10;

        public PayloadUniquenessFilter(int maxQueueItems = 10)
        {
            MaxQueueItems = maxQueueItems;
        }

        public bool TryFilterNewPayload(T payload)
        {
            if (PayloadQueue.Contains(payload))
            {
                return false;
            }
            else if  (PayloadQueue.Count < MaxQueueItems)
            {
                PayloadQueue.Enqueue(payload);
                return true;
            }
            else
            {
                PayloadQueue.TryDequeue(out _);
                PayloadQueue.Enqueue(payload);
                return true;
            }
        }
    }
}
