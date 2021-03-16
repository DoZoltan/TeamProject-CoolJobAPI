using Microsoft.EntityFrameworkCore;

namespace CoolJobAPI.Models
{
    public class JobContext : DbContext
    {
        public JobContext(DbContextOptions<JobContext> options)
            : base(options)
        {
        }
        public DbSet<Job> Jobs { get; set; }
        public DbSet<Favorite> Favorites { get; set; }
    }

}
