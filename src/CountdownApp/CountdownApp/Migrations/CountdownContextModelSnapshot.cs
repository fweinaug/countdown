using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace CountdownApp.Migrations
{
  [DbContext(typeof(CountdownContext))]
  partial class CountdownContextModelSnapshot : ModelSnapshot
  {
    protected override void BuildModel(ModelBuilder modelBuilder)
    {
      modelBuilder
        .HasAnnotation("ProductVersion", "1.1.1");

      modelBuilder.Entity("CountdownApp.Countdown", b =>
      {
        b.Property<int>("Id")
          .ValueGeneratedOnAdd();

        b.Property<DateTime>("Created");

        b.Property<DateTime>("Date");

        b.Property<bool>("FinishedNotification");

        b.Property<string>("Guid");

        b.Property<bool>("HasImage");

        b.Property<byte[]>("ImageData");

        b.Property<bool>("IsRecurrent");

        b.Property<string>("Name");

        b.Property<bool>("PinnedToStart");

        b.HasKey("Id");

        b.ToTable("Countdowns");
      });
    }
  }
}