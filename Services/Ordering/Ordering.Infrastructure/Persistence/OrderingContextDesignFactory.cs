using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Ordering.Infrastructure.Persistence
{
    public class OrderingContextDesignFactory : IDesignTimeDbContextFactory<OrderContext>
    {
        private readonly IConfiguration _configuration;

        public OrderingContextDesignFactory(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public OrderContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<OrderContext>()
                .UseSqlServer(_configuration.GetConnectionString("OrderingConnectionString"));

            return new OrderContext(optionsBuilder.Options);
        }
    }
}