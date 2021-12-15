using Microsoft.Extensions.Logging;
using Ordering.Domain.Entities;

namespace Ordering.Infrastructure.Persistence;

public class OrderContextSeed
{
    public static async Task SeedAsync(OrderContext context, ILogger<OrderContextSeed> logger)
    {
        if (context.Orders.Any() is false)
        {
            context.Orders.AddRange(GetPreconfiguredOrders());
            await context.SaveChangesAsync();
            logger.LogInformation("Seed database associated with context {DbContextName}",
                nameof(OrderContext));
        }
    }

    private static IEnumerable<Order> GetPreconfiguredOrders()
    {
        return new List<Order>
        {
            new()
            {
                UserName = "didacks",
                TotalPrice = 100,
                FirstName = "Didac",
                LastName = "Rius",
                EmailAddress = "didacriuscom@gmail.com",
                AddressLine = "Barcelona",
                Country = "Spain",
                ZipCode = "08"
            }
        };
    }
}