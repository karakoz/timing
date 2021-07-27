using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Timing.Scheduling;
using Timing.Services;
using Xunit;

namespace Timing.Tests
{
    public class SequentialSchedulerPerformanceTest
    {
        [Fact]
        public void AddMany()
        {
            const int COUNT = 1_000_000;

            var scheduler = new SequentialScheduler();

            var sw = Stopwatch.StartNew();

            foreach (var i in Enumerable.Range(0, COUNT).Reverse())
            {
                scheduler.TryAddPeriod(new DateTime().AddHours(i*2), new DateTime().AddHours(i*2 + 1));
            }

            var elapsed = sw.ElapsedMilliseconds;

            Assert.True(elapsed < 2000);


            var sw2 = Stopwatch.StartNew();

            for (var i = 0; i < COUNT; i++)
            {
                Assert.True(scheduler.TryAddPeriod(new DateTime().AddHours(i * 2 + 1), new DateTime().AddHours((i + 1) * 2)));
            }

            var elapsed2 = sw2.ElapsedMilliseconds;

            Assert.True(elapsed2 < 2000);

        }
    }
}
