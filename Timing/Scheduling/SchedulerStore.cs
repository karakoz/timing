using C5;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Timing.Models;

namespace Timing.Scheduling
{
    public class SchedulerStore<TKey>
    {
        private readonly ConcurrentDictionary<TKey, IScheduler> _calendarSchedulers = new();

        public IScheduler GetOrCreateScheduler(TKey key)
        {
            return _calendarSchedulers.GetOrAdd(key, x => new SequentialScheduler());
        }

        public bool RemoveScheduler(TKey key)
        {
            return _calendarSchedulers.TryRemove(key, out var _);
        }
    }
}
