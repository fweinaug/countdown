using Microsoft.EntityFrameworkCore;

namespace CountdownApp
{
  public class CountdownContext : DbContext
  {
    public DbSet<Countdown> Countdowns { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
      optionsBuilder.UseSqlite("Filename=Countdowns.db");
    }
  }
}