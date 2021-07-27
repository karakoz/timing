using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using Timing.Scheduling;
using Xunit;

namespace Timing.Tests
{
    public class SequentialSchedulerTests
    {
        [Fact]
        public void Adding()
        {
            var scheduler = new SequentialScheduler();

            // add 10:00 - 11:00
            Assert.True(scheduler.TryAddPeriod(new DateTime(2020, 10, 20, 10, 0, 0), new DateTime(2020, 10, 20, 11, 0, 0)));
            Assert.False(scheduler.TryAddPeriod(new DateTime(2020, 10, 20, 10, 0, 0), new DateTime(2020, 10, 20, 11, 0, 0)));
            Assert.False(scheduler.TryAddPeriod(new DateTime(2020, 10, 20, 9, 0, 0), new DateTime(2020, 10, 20, 11, 0, 0)));
            Assert.False(scheduler.TryAddPeriod(new DateTime(2020, 10, 20, 9, 0, 0), new DateTime(2020, 10, 20, 10, 30, 0)));
            Assert.False(scheduler.TryAddPeriod(new DateTime(2020, 10, 20, 9, 0, 0), new DateTime(2020, 10, 20, 12, 0, 0)));
            Assert.False(scheduler.TryAddPeriod(new DateTime(2020, 10, 20, 10, 0, 0), new DateTime(2020, 10, 20, 12, 0, 0)));
            Assert.False(scheduler.TryAddPeriod(new DateTime(2020, 10, 20, 10, 59, 0), new DateTime(2020, 10, 20, 12, 0, 0)));

            // add 12:00 - 14:00
            Assert.True(scheduler.TryAddPeriod(new DateTime(2020, 10, 20, 12, 0, 0), new DateTime(2020, 10, 20, 14, 0, 0)));

            // try add 10:59 - 11:30
            Assert.False(scheduler.TryAddPeriod(new DateTime(2020, 10, 20, 10, 59, 0), new DateTime(2020, 10, 20, 11, 30, 0)));
            // try add 11:10 - 12:01
            Assert.False(scheduler.TryAddPeriod(new DateTime(2020, 10, 20, 11, 10, 0), new DateTime(2020, 10, 20, 12, 1, 0)));

            // add 11:00 - 12:00
            Assert.True(scheduler.TryAddPeriod(new DateTime(2020, 10, 20, 11, 0, 0), new DateTime(2020, 10, 20, 12, 0, 0)));

            var periods = scheduler.GetPeriods();

            Assert.Equal(periods, new[]
            {
                (new DateTime(2020, 10, 20, 10, 0, 0), new DateTime(2020, 10, 20, 11, 0, 0)),
                (new DateTime(2020, 10, 20, 11, 0, 0), new DateTime(2020, 10, 20, 12, 0, 0)),
                (new DateTime(2020, 10, 20, 12, 0, 0), new DateTime(2020, 10, 20, 14, 0, 0)),
            });

        }

        [Fact]
        public void Removing()
        {
            var scheduler = new SequentialScheduler();

            // add 10:00 - 11:00
            scheduler.TryAddPeriod(new DateTime(2020, 10, 20, 10, 0, 0), new DateTime(2020, 10, 20, 11, 0, 0));

            // add 11:00 - 12:00
            scheduler.TryAddPeriod(new DateTime(2020, 10, 20, 11, 0, 0), new DateTime(2020, 10, 20, 12, 0, 0));

            // add 12:00 - 14:00
            scheduler.TryAddPeriod(new DateTime(2020, 10, 20, 12, 0, 0), new DateTime(2020, 10, 20, 14, 0, 0));

            Assert.False(scheduler.TryRemovePeriod(new DateTime(2020, 10, 20, 11, 0, 0), new DateTime(2020, 10, 20, 12, 1, 0)));
            Assert.False(scheduler.TryRemovePeriod(new DateTime(2020, 10, 20, 10, 0, 0), new DateTime(2020, 10, 20, 12, 0, 0)));
            Assert.False(scheduler.TryRemovePeriod(new DateTime(2020, 10, 20, 13, 0, 0), new DateTime(2020, 10, 20, 14, 0, 0)));
            Assert.False(scheduler.TryRemovePeriod(new DateTime(2020, 10, 20, 13, 0, 0), new DateTime(2020, 10, 20, 15, 0, 0)));

            Assert.True(scheduler.TryRemovePeriod(new DateTime(2020, 10, 20, 11, 0, 0), new DateTime(2020, 10, 20, 12, 0, 0)));

            var periods = scheduler.GetPeriods();

            Assert.Equal(periods, new[]
            {
                (new DateTime(2020, 10, 20, 10, 0, 0), new DateTime(2020, 10, 20, 11, 0, 0)),
                (new DateTime(2020, 10, 20, 12, 0, 0), new DateTime(2020, 10, 20, 14, 0, 0)),
            });

        }

    }
}
