using BabyGame.Data;

namespace BabyGame;

public interface IBabyGachaService
{
    Task<Baby> CreateBabyAsync(Marriage marriage, string? babyName = null);
}