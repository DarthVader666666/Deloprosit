using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Deloprosit.Data
{
    public class DeloprositDbContext: DbContext
    {
        private readonly IConfiguration _configuration;

        public DeloprositDbContext(DbContextOptions<DeloprositDbContext> options, IConfiguration configuration) : base(options)
        {
            _configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_configuration["ConnectionStrings:DeloprositDb"]);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

        }
    }
}
