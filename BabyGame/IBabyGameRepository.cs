using BabyGame.Data;

namespace BabyGame;

public interface IBabyGameRepository
{
    Task SaveMarriageAsync(Marriage marriage);
    Task<Marriage> GetMarriageAsync(Spouse spouse);
    Task<bool> GetIsMarriedAsync(Spouse spouse);
    Task SaveBabyAsync(Baby baby);
    Task CreateOrUpdateSpouse(Spouse spouse);
}