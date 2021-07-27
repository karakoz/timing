using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using Timing.Entities;
using Timing.Entities.Enums;
using Timing.Models;
using Timing.Scheduling;

namespace Timing.Controllers
{
    [Route("api/Calendars/{calendarId}")]
    [ApiController]
    public class PeriodsController : Controller
    {
        private readonly TimingContext _timingContext;
        private readonly SchedulerStore<Guid> _schedulerStore;
        private readonly ILogger<PeriodsController> _logger;

        public PeriodsController(TimingContext timingContext, SchedulerStore<Guid> schedulerStore, ILogger<PeriodsController> logger)
        {
            _timingContext = timingContext;
            _schedulerStore = schedulerStore;
            _logger = logger;
        }

        [HttpPost("Periods")]
        public async Task<IActionResult> Add(Guid calendarId, Period period)
        {
            _logger.LogInformation("Adding period {@Period} to calendar {id}.", period, calendarId);

            if (!_timingContext.Calendars.Any(c => c.Id == calendarId))
            {
                return NotFound(new { Error = $"Calendar with id = {calendarId} not found." });
            }

            var scheduler = _schedulerStore.GetOrCreateScheduler(calendarId);

            if (scheduler.TryAddPeriod(period.Begin, period.End))
            {
                var action = await _timingContext.CalendarActions.AddAsync(new CalendarAction
                {
                    CalendarId = calendarId,
                    Begin = period.Begin,
                    End = period.End,
                    ActionType = ActionType.Add
                });

                
                await _timingContext.SaveChangesAsync();
                
                return Ok(new Period(action.Entity.Begin, action.Entity.End));
            }
            else
            {
                return BadRequest(new { Error = "Period overlapping detected." });
            }
        }

        [HttpGet("Periods")]
        public Task<IActionResult> Get(Guid calendarId)
        {
            var scheduler = _schedulerStore.GetOrCreateScheduler(calendarId);
            var periods = scheduler.GetPeriods();

            return Task.FromResult<IActionResult>(Ok(periods));
        }

        [HttpDelete("Periods")]
        public async Task<IActionResult> Delete(Guid calendarId, Period period)
        {
            _logger.LogInformation("Adding period {@Period} to calendar {id}.", period, calendarId);

            if (!_timingContext.Calendars.Any(c => c.Id == calendarId))
            {
                return NotFound(new { Error = $"Calendar with id = {calendarId} not found." });
            }

            var scheduler = _schedulerStore.GetOrCreateScheduler(calendarId);

            if (scheduler.TryRemovePeriod(period.Begin, period.End))
            {
                var action = await _timingContext.CalendarActions.AddAsync(new CalendarAction
                {
                    CalendarId = calendarId,
                    Begin = period.Begin,
                    End = period.End,
                    ActionType = ActionType.Remove,
                });
                
                await _timingContext.SaveChangesAsync();

                return Ok();
            }

            return BadRequest(new { Error = $"Can not remove period ({period.Begin}, {period.End})." });
        }
    }
}
