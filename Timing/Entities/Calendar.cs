using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Timing.Entities
{
    public class Calendar
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public ICollection<CalendarAction> Actions { get; set; }
    }
}
