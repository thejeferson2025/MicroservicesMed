using WorkItemsService.Models;
using Microsoft.EntityFrameworkCore;

namespace WorkItemsService.Data
{
    public class WorkItemsDbContext : DbContext
    {
        public WorkItemsDbContext(DbContextOptions<WorkItemsDbContext> options) : base(options) { }
        public DbSet<WorkItem> WorkItems { get; set; }
    }
}