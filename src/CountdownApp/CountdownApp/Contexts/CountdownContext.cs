using System;
using System.IO;
using Microsoft.EntityFrameworkCore;

namespace CountdownApp
{
  public class CountdownContext : DbContext
  {
    public DbSet<Countdown> Countdowns { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
      var dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Countdowns.db");

      optionsBuilder.UseSqlite($"Data Source={dbPath}");
    }
  }
}
