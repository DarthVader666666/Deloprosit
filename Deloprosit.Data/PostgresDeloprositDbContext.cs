using Microsoft.EntityFrameworkCore;

namespace Deloprosit.Data
{
    public class PostgresDeloprositDbContext : DeloprositDbContext
    {
        public PostgresDeloprositDbContext(DbContextOptions<PostgresDeloprositDbContext> options) : base(options)
        {
        }
    }
}