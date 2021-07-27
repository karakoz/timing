using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Timing.Entities
{
    public class TimingContext : DbContext
    {
        public TimingContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Calendar> Calendars { get; set; }

        public DbSet<CalendarAction> CalendarActions { get; set; }
    }
}
