using System;
using System.Collections.Generic;

namespace Timing.Scheduling
{
    public interface IScheduler
    {
        IEnumerable<(DateTime Begin, DateTime End)> GetPeriods();

        bool TryAddPeriod(DateTime begin, DateTime end);

        bool TryRemovePeriod(DateTime begin, DateTime end);
    }
}
