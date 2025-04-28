using Microsoft.EntityFrameworkCore;

namespace Delopro.Data
{
    public class MssqlDeloprositDbContext: DeloprositDbContext
    {
        public MssqlDeloprositDbContext(DbContextOptions<MssqlDeloprositDbContext> options) : base(options)
        {
        }
    }
}
