using BabyGame.Data;
using Microsoft.EntityFrameworkCore;

namespace BabyGame;

public class BabyGameDbContext : DbContext, IBabyGameRepository
{
    public DbSet<Marriage> Marriages { get; set; }

    public Task<Marriage?> GetMarriageAsync(ulong userId)
    {
        return Marriages
            .FirstOrDefaultAsync(x => x.User1Id == userId || x.User2Id == userId);
    }

    public async Task AddMarriageAsync(Marriage marriage)
    {
        await Marriages.AddAsync(marriage);
        await SaveChangesAsync();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Marriage>()
            .HasIndex(x => new { x.User1Id, x.User2Id })
            .IsUnique();

        base.OnModelCreating(modelBuilder);
    }
}