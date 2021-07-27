using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Timing.Models;

namespace Timing.Validation
{
    public class PeriodValidator : AbstractValidator<Period>
    {
        public PeriodValidator()
        {
            RuleFor(period => period.Begin).NotEmpty();
            RuleFor(period => period.End).NotEmpty();
            RuleFor(period => period.End).GreaterThan(period => period.Begin)
                .WithMessage(period => $"\'{nameof(period.End)}\' must be greater then \'{nameof(period.Begin)}\'");
        }
    }
}
