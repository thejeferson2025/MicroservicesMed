using UserManagementService.Models;
using Microsoft.EntityFrameworkCore;

namespace UserManagementService.Data
{
    public class UserDbContext : DbContext
    {
        public UserDbContext(DbContextOptions<UserDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<WorkItem> WorkItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Mapeo exacto a la base de datos SQL
            modelBuilder.Entity<WorkItem>()
                .HasOne<User>()
                .WithMany(u => u.WorkItems)
                .HasForeignKey(w => w.UserId);
        }
    }
}