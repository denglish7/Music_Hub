using Microsoft.EntityFrameworkCore;

namespace BBApp.Models
{
    public class BBContext : DbContext
    {
        public BBContext(DbContextOptions<BBContext> options) : base(options)
        { }
        public DbSet<User> users { get; set; }
        public DbSet<Song> songs { get; set; }
        public DbSet<Join> Joins { get; set; } 
    }
}