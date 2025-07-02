using BabyGame.Data;

namespace BabyGame;

public interface IBabyGameRepository
{
    Task AddMarriageAsync(Marriage marriage);
    Task<Marriage> GetMarriageAsync(ulong userId);
    Task<bool> GetIsMarriedAsync(ulong userId);
    Task SaveBabyAsync(Baby baby);
}