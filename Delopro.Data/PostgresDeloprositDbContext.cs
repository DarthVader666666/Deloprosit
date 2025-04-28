using Microsoft.EntityFrameworkCore;

namespace Delopro.Data
{
    public class PostgresDeloprositDbContext : DeloprositDbContext
    {
        public PostgresDeloprositDbContext(DbContextOptions<PostgresDeloprositDbContext> options) : base(options)
        {
        }
    }
}