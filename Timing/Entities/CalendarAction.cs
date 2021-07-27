using Microsoft.EntityFrameworkCore;
using System;
using Timing.Entities.Enums;

namespace Timing.Entities
{
    [Index(nameof(ActionDate))]
    public class CalendarAction
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Calendar Calendar { get; set; }

        public Guid CalendarId { get; set; }

        public DateTime Begin { get; set; }

        public DateTime End { get; set; }

        public ActionType ActionType { get; set; }

        public DateTime ActionDate { get; set; } = DateTime.Now;

    }
}
