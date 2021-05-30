using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CoolJobAPI.Models
{
    public class JobContext : IdentityDbContext<User>
    {
        public JobContext(DbContextOptions<JobContext> options)
            : base(options)
        {
        }
        public DbSet<Job> Jobs { get; set; }
        public DbSet<Favorite> Favorites { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
    }
}
