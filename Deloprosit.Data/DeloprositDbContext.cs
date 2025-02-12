using Deloprosit.Data.Entities;
using Deloprosit.Data.Enums;

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
            modelBuilder.Entity<User>(user =>
            {
                user.HasKey(x => x.UserId);
                user.HasIndex(x => x.Email).IsUnique();
                user.HasData(
                    new User
                    {
                        UserId = 1,
                        Password = _configuration["OwnerPassword"]
                    },
                    new User
                    {
                        UserId = 2,
                        Password = _configuration["AdminPassword"],
                        Nickname = "vader"
                    });
                user.HasIndex(x => x.Email).IsUnique();
                user.HasIndex(x => x.Nickname).IsUnique();
                user.Property(x => x.Password).IsRequired().HasMaxLength(100);
                user.Property(x => x.Avatar).HasMaxLength(10000);
            });
            modelBuilder.Entity<Role>(role =>
            {
                role.HasKey(x => x.RoleId);
                role.HasData(
                    new Role
                    {
                        RoleId = (int)UserRoleType.Owner,
                        RoleName = nameof(UserRoleType.Owner),
                    },
                    new Role
                    {
                        RoleId = (int)UserRoleType.Admin,
                        RoleName = nameof(UserRoleType.Admin),
                    },
                    new Role
                    {
                        RoleId = (int)UserRoleType.User,
                        RoleName = nameof(UserRoleType.User)
                    });
            });
            modelBuilder.Entity<UserRole>(userRole =>
            {
                userRole.HasKey(x => new { x.UserId, x.RoleId });
                userRole.HasData(
                    new UserRole
                    {
                        UserId = 1,
                        RoleId = (int)UserRoleType.Owner
                    },
                    new UserRole
                    {
                        UserId = 2,
                        RoleId = (int)UserRoleType.Admin
                    });
            });
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
    }
}
