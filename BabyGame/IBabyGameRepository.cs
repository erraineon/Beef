using BabyGame.Data;
using Microsoft.EntityFrameworkCore;

namespace BabyGame;

public interface IBabyGameRepository
{
    Task SaveChangesAsync();
    Task<Marriage> GetMarriageAsync(Player player);
    Task<bool> GetIsMarriedAsync(Player player);
    Task CreateBabyAsync(Baby baby);
    Task CreateSpouseAsync(Player player);
    Task CreateMarriageAsync(Marriage marriage);
    Task<IAsyncDisposable> BeginTransactionAsync();
}