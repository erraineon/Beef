using BabyGame.Exceptions;
using BabyGame.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Numerics;

namespace BabyGame.Data;

public class BabyGameDbContext : DbContext, IBabyGameRepository
{
    public DbSet<Marriage> Marriages { get; set; }
    public DbSet<Baby> Babies { get; set; }
    public DbSet<Player> Spouses { get; set; }
    public DbSet<Modifier> Modifiers { get; set; }
    public DbSet<Proposal> Proposals { get; set; }

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

    public Task<Proposal?> GetProposalOrNullAsync(Player proposer, Player fiance)
    {
        return Proposals
            .AsNoTracking()
            .FirstOrDefaultAsync(x =>
                (x.Proposer.Id == proposer.Id && x.Fiance.Id == fiance.Id) ||
                (x.Fiance.Id == proposer.Id && x.Proposer.Id == fiance.Id)
            );
    }

    public IQueryable<Proposal> GetProposals(Player proposer)
    {
        return Proposals
            .AsNoTracking()
            .Where(x => x.Proposer.Id == proposer.Id || x.Fiance.Id == proposer.Id);
    }

    public async Task SaveProposalAsync(Proposal proposal)
    {
        Proposals.Attach(proposal);
        await SaveChangesAsync();
    }

    public async Task<IAsyncDisposable> BeginTransactionAsync()
    {
        return new BabyGameTransaction(await Database.BeginTransactionAsync());
    }

    public async Task SaveMarriageAsync(Marriage marriage)
    {
        Marriages.Attach(marriage);
        await SaveChangesAsync();
    }

    private Task<Marriage?> GetMarriageOrNullAsync(Player player)
    {
        return Marriages
            .AsNoTracking()
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