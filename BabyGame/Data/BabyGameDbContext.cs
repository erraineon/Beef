using BabyGame.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace BabyGame.Data;

public class BabyGameDbContext : DbContext, IBabyGameRepository
{
    public DbSet<Marriage> Marriages { get; set; }
    public DbSet<Baby> Babies { get; set; }
    public DbSet<Spouse> Spouses { get; set; }

    private Task<Marriage?> GetMarriageOrNullAsync(Spouse spouse)
    {
        return Marriages
            .FirstOrDefaultAsync(x => x.Spouse1.Id == spouse.Id || x.Spouse2.Id == spouse.Id);
    }

    public async Task<bool> GetIsMarriedAsync(Spouse spouse)
    {
        return await GetMarriageOrNullAsync(spouse) != null;
    }

    public Task SaveBabyAsync(Baby baby)
    {
        Babies.Attach(baby);
        return SaveChangesAsync();
    }

    public Task CreateOrUpdateSpouse(Spouse spouse)
    {
        Spouses.Attach(spouse);
        return SaveChangesAsync();
    }

    public async Task<Marriage> GetMarriageAsync(Spouse spouse)
    {
        return await GetMarriageOrNullAsync(spouse) ?? throw new NotMarriedException(spouse);
    }

    public async Task SaveMarriageAsync(Marriage marriage)
    {
        Marriages.Attach(marriage);
        await SaveChangesAsync();
    }
}