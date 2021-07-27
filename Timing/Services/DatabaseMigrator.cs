using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Timing.Entities;

namespace Timing.Services
{
    public class DatabaseMigrator : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;

        public DatabaseMigrator(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {

            using var scope = _serviceProvider.CreateScope();

            var timingContext = scope.ServiceProvider.GetRequiredService<TimingContext>();

            await timingContext.Database.MigrateAsync();
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
