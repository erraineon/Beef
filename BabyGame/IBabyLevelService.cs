using BabyGame.Data;

namespace BabyGame;

public interface IBabyLevelService
{
    Task GainExperienceAsync(Baby baby, double chu);
}