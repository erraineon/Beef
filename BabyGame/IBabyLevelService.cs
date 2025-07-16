using BabyGame.Data;

namespace BabyGame;

public interface IBabyLevelService
{
    void GainExperience(Baby baby, double chu);
}