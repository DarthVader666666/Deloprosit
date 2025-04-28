using Microsoft.EntityFrameworkCore;

namespace Delopro.Data
{
    public class MssqlDeloproDbContext: DeloproDbContext
    {
        public MssqlDeloproDbContext(DbContextOptions<MssqlDeloproDbContext> options) : base(options)
        {
        }
    }
}
