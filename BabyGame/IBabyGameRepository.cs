using BabyGame.Data;

namespace BabyGame;

public interface IBabyGameRepository
{
    Task SaveMarriageAsync(Marriage marriage);
    Task<Marriage> GetMarriageAsync(Player player);
    Task<bool> GetIsMarriedAsync(Player player);
    Task CreateSpouseAsync(Player player);
    Task CreateMarriageAsync(Marriage marriage);
    Task<IAsyncDisposable> BeginTransactionAsync();
}