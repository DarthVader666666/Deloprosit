using Microsoft.EntityFrameworkCore;

namespace Delopro.Data
{
    public class PostgresDeloproDbContext : DeloproDbContext
    {
        public PostgresDeloproDbContext(DbContextOptions<PostgresDeloproDbContext> options) : base(options)
        {
        }
    }
}