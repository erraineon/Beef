using BabyGame.Data;

namespace BabyGame;

public interface IBabyGameRepository
{
    Task AddMarriageAsync(Marriage marriage);
    Task<Marriage?> GetMarriageAsync(ulong userId);
}