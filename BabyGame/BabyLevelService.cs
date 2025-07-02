using BabyGame.Data;

namespace BabyGame;

public class BabyLevelService(
    IBabyGameRepository babyGameRepository,
    IBabyGameLogger logger
) : IBabyLevelService
{
    public async Task GainExperienceAsync(Baby baby, double chu)
    {
        var currentLevel = baby.Level;
        baby.TotalExperience += chu;
        baby.Level = RecalculateLevel(baby);
        await babyGameRepository.SaveBabyAsync(baby);
        if (currentLevel != baby.Level) logger.Log($"{baby.Name} has become rank {baby.GetRank()}!");
        logger.Log($"{baby.Name} needs {GetExperiencePointsUntilNextLevel(baby)} Chu to rank up.");
    }

    public int RecalculateLevel(Baby baby)
    {
        return (int)Math.Floor(Math.Cbrt(baby.TotalExperience));
    }

    public double GetExperiencePointsUntilNextLevel(Baby baby)
    {
        return Math.Pow(baby.Level, 3) - baby.TotalExperience;
    }
}