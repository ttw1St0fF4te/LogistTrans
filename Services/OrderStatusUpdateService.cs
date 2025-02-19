using LogistTrans.Context;

namespace LogistTrans.Services;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

public class OrderStatusUpdateService : BackgroundService
{
    private readonly IServiceProvider _services;
    private readonly ILogger<OrderStatusUpdateService> _logger;

    public OrderStatusUpdateService(IServiceProvider services, ILogger<OrderStatusUpdateService> logger)
    {
        _services = services;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using (var scope = _services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var now = DateTime.UtcNow;

                var ordersToUpdate = await context.Orders
                    .Where(o => o.Status == "В пути" && o.DeliveryDate <= now)
                    .ToListAsync();

                foreach (var order in ordersToUpdate)
                {
                    order.Status = "Доставлено";
                    _logger.LogInformation($"Order {order.Id} status updated to 'Доставлено'.");
                }

                await context.SaveChangesAsync();
            }

            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken); // Check every minute
        }
    }
}
