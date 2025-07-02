using BabyGame.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace BabyGame.Data;

public class BabyGameDbContext : DbContext, IBabyGameRepository
{
    public DbSet<Marriage> Marriages { get; set; }
    public DbSet<Baby> Babies { get; set; }

    private Task<Marriage?> GetMarriageOrNullAsync(ulong userId)
    {
        return Marriages
            .FirstOrDefaultAsync(x => x.User1Id == userId || x.User2Id == userId);
    }

    public async Task<bool> GetIsMarriedAsync(ulong userId)
    {
        return await GetMarriageOrNullAsync(userId) != null;
    }

    public Task SaveBabyAsync(Baby baby)
    {
        Babies.Attach(baby);
        return SaveChangesAsync();
    }

    public async Task<Marriage> GetMarriageAsync(ulong userId)
    {
        return await GetMarriageOrNullAsync(userId) ?? throw new NotMarriedException(userId);
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