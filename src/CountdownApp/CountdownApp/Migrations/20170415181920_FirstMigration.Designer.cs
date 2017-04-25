using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using CountdownApp;

namespace CountdownApp.Migrations
{
    [DbContext(typeof(CountdownContext))]
    [Migration("20170415181920_FirstMigration")]
    partial class FirstMigration
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
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
