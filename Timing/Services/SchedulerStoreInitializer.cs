using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Timing.Entities;
using Timing.Entities.Enums;
using Timing.Scheduling;

namespace Timing.Services
{
    public class SchedulerStoreInitializer : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly SchedulerStore<Guid> _schedulerStore;
        private readonly ILogger<SchedulerStoreInitializer> _logger;

        public SchedulerStoreInitializer(IServiceProvider serviceProvider, SchedulerStore<Guid> schedulerService, ILogger<SchedulerStoreInitializer> logger)
        {
            _serviceProvider = serviceProvider;
            _schedulerStore = schedulerService;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Starting schedulers initialization.");

            using var scope = _serviceProvider.CreateScope();

            var timingContext = scope.ServiceProvider.GetRequiredService<TimingContext>();

            timingContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

            var stopWatch = Stopwatch.StartNew();

            var actions = await timingContext.CalendarActions
                .OrderBy(x => x.ActionDate)
                .ToArrayAsync();

            actions
                .GroupBy(x => x.CalendarId)
                .AsParallel()
                .WithDegreeOfParallelism(4) // todo: recognize best number based on CPUs count ot set from config.
                .ForAll(grouping =>
                {
                    var scheduler = _schedulerStore.GetOrCreateScheduler(grouping.Key);

                    foreach (var action in grouping)
                    {
                        switch (action.ActionType)
                        {
                            case ActionType.Add:
                                if (!scheduler.TryAddPeriod(action.Begin, action.End))
                                {
                                    _logger.LogWarning("Period ({begin}, {end}) is not added to scheduler {key}.", action.Begin, action.End, grouping.Key);
                                }
                                break;
                            case ActionType.Remove:
                                if (!scheduler.TryRemovePeriod(action.Begin, action.End))
                                {
                                    _logger.LogWarning("Period ({begin}, {end}) is not removed from scheduler {key}.", action.Begin, action.End, grouping.Key);
                                }
                                break;
                        }
                    }
                });

            var elapsed = stopWatch.Elapsed;

            _logger.LogInformation($"Calendars schedulers initialized in {elapsed}.");

        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
