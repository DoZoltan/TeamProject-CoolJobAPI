using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CoolJobAPI.Models
{
    public class JobContext : IdentityDbContext
    {
        public JobContext(DbContextOptions<JobContext> options)
            : base(options)
        {
        }
        public DbSet<Job> Jobs { get; set; }
        public DbSet<Favorite> Favorites { get; set; }
        //public DbSet<User> Users { get; set; }

    }
   

}
