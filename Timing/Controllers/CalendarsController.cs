using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using Timing.Entities;
using Timing.Scheduling;

namespace Timing.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CalendarsController : Controller
    {
        private readonly TimingContext _timingContext;
        private readonly SchedulerStore<Guid> _schedulerStore;
        private readonly ILogger<CalendarsController> _logger;

        public CalendarsController(TimingContext timingContext, SchedulerStore<Guid> schedulerService, ILogger<CalendarsController> logger)
        {
            _timingContext = timingContext;
            _schedulerStore = schedulerService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Create()
        {
            _logger.LogInformation("Creating new calendar.");

            var calendar = await _timingContext.Calendars.AddAsync(new Calendar());

            await _timingContext.SaveChangesAsync();

            _schedulerStore.GetOrCreateScheduler(calendar.Entity.Id);

            _logger.LogInformation("Calendar {id} created.", calendar.Entity.Id);

            return Ok(calendar.Entity.Id);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            _logger.LogInformation("Removing calendar {id}.", id);

            _timingContext.Calendars.Remove(new Calendar { Id = id });

            await _timingContext.SaveChangesAsync();

            _schedulerStore.RemoveScheduler(id);

            _logger.LogInformation("Calendar {id} removed.", id);

            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _timingContext.Calendars.Select(x => x.Id).ToArrayAsync());
        }
    }
}
