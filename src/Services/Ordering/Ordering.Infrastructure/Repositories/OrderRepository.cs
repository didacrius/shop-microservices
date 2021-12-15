using Microsoft.EntityFrameworkCore;
using Ordering.Application.Contracts.Persistence;
using Ordering.Domain.Entities;

namespace Ordering.Infrastructure.Persistence.Repositories;

public class OrderRepository : RepositoryBase<Order>, IOrderRepository
{
    public OrderRepository(OrderContext context)
        : base(context)
    {
    }

    public async Task<IEnumerable<Order>> GetByUsername(string username)
    {
        return await _dbContext.Orders
            .Where(o => o.UserName == username)
            .ToListAsync();
    }
}