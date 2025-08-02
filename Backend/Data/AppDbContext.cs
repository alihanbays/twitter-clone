using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TwitterClone.Models;
using static System.Net.Mime.MediaTypeNames;

namespace TwitterClone.Data
{
    public class AppDbContext : IdentityDbContext<User>
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<Tweet> Tweets => Set<Tweet>();
    }
}
