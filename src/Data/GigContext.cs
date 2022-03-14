namespace GigLocal.Data;

public class GigContext : DbContext
{
    public DbSet<Venue> Venues { get; set; }
    public DbSet<Gig> Gigs { get; set; }

    public GigContext(DbContextOptions<GigContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Venue>().ToTable(nameof(Venue))
            .HasMany(v => v.Gigs);
        modelBuilder.Entity<Gig>().ToTable(nameof(Gig));
    }
}
