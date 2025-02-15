using Deloprosit.Data.Entities;
using Deloprosit.Data.Enums;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Deloprosit.Data
{
    public class DeloprositDbContext: DbContext
    {
        private readonly IConfiguration _configuration;
        const int maxRoleNameLength = 50;
        const int maxNameLength = 100;
        const int maxInfoLength = 1000;
        const int maxBytesLength = 8000;

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
                user.HasIndex(x => x.Nickname).IsUnique();
                user.Property(x => x.Email).HasMaxLength(maxNameLength).IsRequired();
                user.Property(x => x.Nickname).HasMaxLength(maxNameLength).IsRequired();
                user.Property(x => x.Password).HasMaxLength(maxNameLength).IsRequired();
                user.Property(x => x.FirstName).HasMaxLength(maxNameLength);
                user.Property(x => x.LastName).HasMaxLength(maxNameLength);
                user.Property(x => x.UserTitle).HasMaxLength(maxNameLength);
                user.Property(x => x.Country).HasMaxLength(maxNameLength);
                user.Property(x => x.City).HasMaxLength(maxNameLength);
                user.Property(x => x.Info).HasMaxLength(maxInfoLength);
                user.Property(x => x.Avatar).HasMaxLength(maxBytesLength);
                user.HasData(
                    new User
                    {
                        UserId = 1,
                        Password = _configuration["OwnerPassword"],
                        Nickname = "owner",
                        Email = "owner@owner.com"
                    },
                    new User
                    {
                        UserId = 2,
                        Password = _configuration["AdminPassword"],
                        Nickname = "admin",
                        Email = "admin@admin.com"
                    });
            });
            modelBuilder.Entity<Role>(role =>
            {
                role.HasKey(x => x.RoleId);
                role.HasIndex(x => x.RoleName).IsUnique();
                role.Property(x => x.RoleName).HasMaxLength(maxRoleNameLength).IsRequired();
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
                userRole.HasKey(ur => new { ur.UserId, ur.RoleId });
                userRole.HasOne(ur => ur.User).WithMany(u => u.UserRoles).HasForeignKey(ur => ur.UserId);
                userRole.HasOne(ur => ur.Role).WithMany(r => r.UserRoles).HasForeignKey(ur => ur.RoleId);
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
            modelBuilder.Entity<Chapter>(chapter =>
            {
                chapter.HasKey(x => x.ChapterId);
                chapter.HasMany(x => x.Themes).WithOne(x => x.Chapter).HasForeignKey(x => x.ChapterId);
                chapter.HasOne(x => x.User).WithMany(x => x.Chapters).HasForeignKey(x => x.UserId);
                chapter.Property(x => x.UserId).IsRequired();
                chapter.Property(x => x.DateCreated).IsRequired();
                chapter.Property(x => x.ChapterTitle).HasMaxLength(maxInfoLength).IsRequired();
            });
            modelBuilder.Entity<Theme>(theme => 
            {
                theme.HasKey(x => x.ThemeId);
                theme.Property(x => x.Description).HasMaxLength(maxInfoLength).IsRequired();
                theme.Property(x => x.DateCreated).IsRequired();
                theme.HasOne(x => x.Chapter).WithMany(x => x.Themes).HasForeignKey(x => x.ChapterId);
                theme.HasOne(x => x.User).WithMany(x => x.Themes).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.NoAction);
            });
            modelBuilder.Entity<Comment>(comment =>
            {
                comment.HasKey(x => x.CommentId);
                comment.Property(x => x.Text).HasMaxLength(maxInfoLength).IsRequired();
                comment.Property(x => x.DateCreated).IsRequired();
                comment.HasOne(x => x.Theme).WithMany(x => x.Comments).HasForeignKey(x => x.ThemeId);
                comment.HasOne(x => x.User).WithMany(x => x.Comments).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.NoAction);
            });
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<Chapter> Chapters { get; set; }
        public DbSet<Theme> Themes { get; set; }
        public DbSet<Comment> Comments { get; set; }
    }
}
