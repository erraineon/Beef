﻿using Microsoft.EntityFrameworkCore;

namespace Beef.Core.Data;

public class BeefDbContext : DbContext
{
    public required DbSet<Trigger> Triggers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TimeTrigger>();
        modelBuilder.Entity<OneTimeTrigger>();
        modelBuilder.Entity<RecurringTrigger>();
        modelBuilder.Entity<CronTrigger>();
        base.OnModelCreating(modelBuilder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=database.db");
    }
}