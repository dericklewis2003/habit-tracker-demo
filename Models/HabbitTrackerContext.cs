using Microsoft.EntityFrameworkCore;

namespace HabbitTrackerRazor.Models
{
    public class HabbitTrackerContext : DbContext
    {
        public HabbitTrackerContext(DbContextOptions<HabbitTrackerContext> options) : base(options) { }

        public DbSet<MemorableMoment> MemorableMoments { get; set; }
        public DbSet<HabitEntry> HabitEntries { get; set; }
        public DbSet<User> Users { get; set; }
    }
} 