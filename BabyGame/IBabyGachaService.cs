using BabyGame.Data;

namespace BabyGame;

public interface IBabyGachaService
{
    Baby CreateBaby(Marriage marriage, string? babyName = null);
}