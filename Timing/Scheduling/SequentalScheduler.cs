using C5;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Timing.Scheduling
{
    public class SequentialScheduler : IScheduler
    {
        private ReaderWriterLockSlim _lock = new();

        private TreeDictionary<DateTime, DateTime> _periods = new(MemoryType.Normal);

        public IEnumerable<(DateTime Begin, DateTime End)> GetPeriods()
        {
            _lock.EnterReadLock();

            try
            {
                return _periods.Select(x => (x.Key, x.Value)).ToList();
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        public bool TryAddPeriod(DateTime begin, DateTime end)
        {
            _lock.EnterUpgradeableReadLock();

            try
            {
                if (intersects(begin, end))
                {
                    return false;
                }
                else
                {
                    _lock.EnterWriteLock();
                    try
                    {
                        _periods.Add(begin, end);
                        return true;
                    }
                    finally
                    {
                        _lock.ExitWriteLock();
                    }
                }
            }
            finally
            {
                _lock.ExitUpgradeableReadLock();
            }
        }

        public bool TryRemovePeriod(DateTime begin, DateTime end)
        {
            _lock.EnterWriteLock();
            try
            {
                if (_periods.Find(ref begin, out var existedEnd)
                    && existedEnd == end)
                {
                    return _periods.Remove(begin);
                }

                return false;
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        private bool intersects(DateTime begin, DateTime end)
        {
            if (_periods.TryWeakPredecessor(begin, out var predecessor))
            {
                if (predecessor.Value > begin)
                {
                    return true;
                }
            }

            if (_periods.TryWeakSuccessor(begin, out var successor))
            {
                if (successor.Key < end)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
