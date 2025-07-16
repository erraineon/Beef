using BabyGame.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace BabyGame.Data;

public class BabyGameDbContext : DbContext, IBabyGameRepository
{
    public DbSet<Marriage> Marriages { get; set; }
    public DbSet<Baby> Babies { get; set; }
    public DbSet<Player> Spouses { get; set; }
    public DbSet<Modifier> Modifiers { get; set; }

    public async Task<bool> GetIsMarriedAsync(Player player)
    {
        return await GetMarriageOrNullAsync(player) != null;
    }

    public async Task CreateSpouseAsync(Player player)
    {
        await Spouses.AddAsync(player);
        await SaveChangesAsync();
    }

    public async Task<Marriage> GetMarriageAsync(Player player)
    {
        return await GetMarriageOrNullAsync(player) ?? throw new NotMarriedException(player);
    }

    public async Task CreateMarriageAsync(Marriage marriage)
    {
        await Marriages.AddAsync(marriage);
        await SaveChangesAsync();
    }

    public async Task<IAsyncDisposable> BeginTransactionAsync()
    {
        return new BabyGameTransaction(await Database.BeginTransactionAsync());
    }

    public async Task SaveMarriageAsync(Marriage marriage)
    {
        Marriages.Attach(marriage);
        await base.SaveChangesAsync();
    }

    private Task<Marriage?> GetMarriageOrNullAsync(Player player)
    {
        return Marriages
            .FirstOrDefaultAsync(x => x.Spouse1.Id == player.Id || x.Spouse2.Id == player.Id);
    }

    private class BabyGameTransaction(IDbContextTransaction transaction) : IAsyncDisposable
    {
        public async ValueTask DisposeAsync()
        {
            await transaction.CommitAsync();
            await transaction.DisposeAsync();
        }
    }
}