using Microsoft.EntityFrameworkCore;

namespace Deloprosit.Data
{
    public class MssqlDeloprositDbContext: DeloprositDbContext
    {
        public MssqlDeloprositDbContext(DbContextOptions<MssqlDeloprositDbContext> options) : base(options)
        {
        }
    }
}
