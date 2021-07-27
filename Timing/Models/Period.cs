using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Timing.Models
{
    public class Period
    {
        public Period() {}

        public Period(DateTime begin, DateTime end)
        {
            Begin = begin;
            End = end;
        }

        public DateTime Begin { get; set; }

        public DateTime End { get; set; }
    }
}
