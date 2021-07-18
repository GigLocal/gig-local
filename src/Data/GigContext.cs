using GigLocal.Models;
using Microsoft.EntityFrameworkCore;

namespace GigLocal.Data
{
    public class GigContext : DbContext
    {
        public GigContext(DbContextOptions<GigContext> options) : base(options)
        {
        }

        public DbSet<Venue> Venues { get; set; }
        public DbSet<Artist> Artists { get; set; }
        public DbSet<Gig> Gigs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Venue>().ToTable(nameof(Venue))
                .HasMany(v => v.Gigs);
            modelBuilder.Entity<Artist>().ToTable(nameof(Artist))
                .HasMany(a => a.Gigs);
            modelBuilder.Entity<Gig>().ToTable(nameof(Gig));
        }
    }
}
